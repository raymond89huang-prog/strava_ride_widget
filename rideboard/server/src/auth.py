import json
import os
import time
import requests
import webbrowser
from urllib.parse import urlencode

CONFIG_PATH = os.path.join(os.path.dirname(__file__), '../config.json')
TOKENS_PATH = os.path.join(os.path.dirname(__file__), '../data/tokens.json')

class StravaAuth:
    def __init__(self):
        self.config = self._load_config()
        self.tokens = self._load_tokens()

    def _load_config(self):
        if not os.path.exists(CONFIG_PATH):
            return None
        with open(CONFIG_PATH, 'r') as f:
            return json.load(f)

    def _load_tokens(self):
        if not os.path.exists(TOKENS_PATH):
            return None
        with open(TOKENS_PATH, 'r') as f:
            return json.load(f)

    def save_tokens(self, tokens):
        # Ensure data directory exists
        os.makedirs(os.path.dirname(TOKENS_PATH), exist_ok=True)
        # Add expiry time if not present (expires_in is usually returned)
        if 'expires_at' not in tokens and 'expires_in' in tokens:
            tokens['expires_at'] = time.time() + tokens['expires_in']
            
        with open(TOKENS_PATH, 'w') as f:
            json.dump(tokens, f, indent=2)
        self.tokens = tokens

    def is_logged_in(self):
        return self.tokens is not None and 'access_token' in self.tokens

    def get_valid_token(self):
        if not self.is_logged_in():
            return None
        
        if time.time() > self.tokens.get('expires_at', 0):
            return self.refresh_token()
        
        return self.tokens['access_token']

    def refresh_token(self):
        print("Refreshing token...")
        payload = {
            'client_id': self.config['client_id'],
            'client_secret': self.config['client_secret'],
            'refresh_token': self.tokens['refresh_token'],
            'grant_type': 'refresh_token'
        }
        try:
            response = requests.post('https://www.strava.com/oauth/token', data=payload)
            response.raise_for_status()
            new_tokens = response.json()
            self.save_tokens(new_tokens)
            return new_tokens['access_token']
        except Exception as e:
            print(f"Failed to refresh token: {e}")
            return None

    def get_login_url(self):
        if not self.config or self.config['client_id'] == "YOUR_CLIENT_ID":
            return None
            
        params = {
            'client_id': self.config['client_id'],
            'redirect_uri': self.config['redirect_uri'],
            'response_type': 'code',
            'scope': 'activity:read_all'
        }
        return f"https://www.strava.com/oauth/authorize?{urlencode(params)}"

    def exchange_code(self, code):
        payload = {
            'client_id': self.config['client_id'],
            'client_secret': self.config['client_secret'],
            'code': code,
            'grant_type': 'authorization_code'
        }
        response = requests.post('https://www.strava.com/oauth/token', data=payload)
        response.raise_for_status()
        self.save_tokens(response.json())
