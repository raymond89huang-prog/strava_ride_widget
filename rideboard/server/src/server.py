from http.server import HTTPServer, BaseHTTPRequestHandler
import json
import os
import time
import threading
from urllib.parse import urlparse, parse_qs
from auth import StravaAuth
from strava_api import StravaAPI

# Paths
CACHE_PATH = os.path.join(os.path.dirname(__file__), '../data/cache.json')

# Global instances
auth = StravaAuth()
api = StravaAPI(auth)
login_event = threading.Event()

class SimpleHTTPRequestHandler(BaseHTTPRequestHandler):
    def do_GET(self):
        parsed_url = urlparse(self.path)
        
        # 1. Main Data Endpoint
        if parsed_url.path == '/strava':
            self.send_response(200)
            self.send_header('Content-type', 'application/json')
            self.end_headers()
            
            # If not logged in, return cached or offline status
            if not auth.is_logged_in():
                data = self.load_cache()
                if not data:
                    data = {"status": "offline", "updated_at": "N/A"}
                else:
                    data["status"] = "cached (offline)"
                self.wfile.write(json.dumps(data).encode())
                return

            # If logged in, return cache (background thread updates it)
            data = self.load_cache()
            if not data:
                data = {"status": "loading", "updated_at": "..."}
            self.wfile.write(json.dumps(data).encode())

        # 2. Trigger Login Endpoint
        elif parsed_url.path == '/login':
            # Check config first
            if not auth.config or auth.config.get('client_id') == "YOUR_CLIENT_ID":
                self.send_response(200)
                self.send_header('Content-type', 'text/html; charset=utf-8')
                self.end_headers()
                html = """
                <html>
                <head>
                    <title>RideBoard Configuration</title>
                    <style>
                        body { font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, Helvetica, Arial, sans-serif; max-width: 600px; margin: 40px auto; line-height: 1.6; color: #333; }
                        code { background: #f4f4f4; padding: 2px 5px; border-radius: 3px; font-family: Consolas, monospace; }
                        .box { border: 1px solid #ddd; padding: 20px; border-radius: 8px; background: #fff; box-shadow: 0 2px 4px rgba(0,0,0,0.05); }
                        h2 { margin-top: 0; color: #fc4c02; }
                        a { color: #fc4c02; text-decoration: none; font-weight: bold; }
                        a:hover { text-decoration: underline; }
                        li { margin-bottom: 10px; }
                    </style>
                </head>
                <body>
                    <div class="box">
                        <h2>Configuration Required / 需要配置</h2>
                        <p>无法启动登录流程，因为尚未配置 Strava API 信息。</p>
                        <p>Cannot start login flow because Strava API info is missing.</p>
                        <hr>
                        <ol>
                            <li>访问 <a href="https://www.strava.com/settings/api" target="_blank">Strava API Settings</a> 并登录。</li>
                            <li>创建一个应用（Application），回调域名（Authorization Callback Domain）填: <code>localhost</code></li>
                            <li>复制 <strong>Client ID</strong> 和 <strong>Client Secret</strong>。</li>
                            <li>打开项目文件 <code>rideboard/server/config.json</code> 并填入。</li>
                            <li>重启服务 (关闭窗口并重新运行 start_server.bat)。</li>
                        </ol>
                    </div>
                </body>
                </html>
                """
                self.wfile.write(html.encode('utf-8'))
                return

            # Redirect user to Strava OAuth page
            url = auth.get_login_url()
            if url:
                print(f"Redirecting to Strava login: {url}")
                self.send_response(302)
                self.send_header('Location', url)
                self.end_headers()
            else:
                self.send_response(500)
                self.wfile.write(b"Error generating login URL")

        # 3. OAuth Callback Endpoint
        elif parsed_url.path == '/callback':
            query = parse_qs(parsed_url.query)
            code = query.get('code', [None])[0]
            if code:
                try:
                    auth.exchange_code(code)
                    self.send_response(200)
                    self.send_header('Content-type', 'text/html')
                    self.end_headers()
                    self.wfile.write(b"Login successful! You can close this window.")
                    # Trigger immediate update
                    update_data()
                except Exception as e:
                    self.send_response(500)
                    self.wfile.write(f"Login failed: {e}".encode())
            else:
                self.send_response(400)
                self.wfile.write(b"Missing code")

        else:
            self.send_response(404)
            self.end_headers()

    def load_cache(self):
        if os.path.exists(CACHE_PATH):
            with open(CACHE_PATH, 'r') as f:
                return json.load(f)
        return None

    def start_login_flow(self):
        url = auth.get_login_url()
        if url:
            import webbrowser
            webbrowser.open(url)
        else:
            print("Cannot start login: Missing config.")

def update_data():
    if auth.is_logged_in():
        print("Fetching new data from Strava...")
        data = api.get_summary()
        if data:
            os.makedirs(os.path.dirname(CACHE_PATH), exist_ok=True)
            with open(CACHE_PATH, 'w') as f:
                json.dump(data, f)
            print("Data updated.")
        else:
            print("No data returned.")

def background_loop():
    while True:
        try:
            update_data()
        except Exception as e:
            print(f"Update error: {e}")
        # Update every 15 minutes
        time.sleep(900)

def run(server_class=HTTPServer, handler_class=SimpleHTTPRequestHandler, port=8787):
    server_address = ('127.0.0.1', port)
    httpd = server_class(server_address, handler_class)
    print(f'Server running at http://127.0.0.1:{port}/strava')
    
    # Start background data fetcher
    threading.Thread(target=background_loop, daemon=True).start()
    
    # Auto-open login if needed
    if not auth.is_logged_in():
        print("Not logged in. Navigate to http://127.0.0.1:8787/login to authenticate.")
    
    httpd.serve_forever()

if __name__ == '__main__':
    run()
