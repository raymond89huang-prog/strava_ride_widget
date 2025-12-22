【RideBoard 使用说明】

1. 注册 Strava API
   - 登录 Strava 开发者官网：https://www.strava.com/settings/api
   - 创建一个应用 (Create an Application)。
   - "Authorization Callback Domain" 请填写：localhost
   - 创建成功后，您将获得 "Client ID" 和 "Client Secret"。

2. 配置 API 信息
   - 进入本目录下的 server 文件夹。
   - 找到 config.json 文件。
   - 使用记事本打开该文件，填入您申请到的 client_id 和 client_secret。
     示例：
     {
         "client_id": "12345",
         "client_secret": "your_secret_string_here",
         "redirect_uri": "http://localhost:8787/callback"
     }
   - 保存文件。

3. 启动程序
   - 双击运行 RideBoard.Widget.exe。
   - 在设置中点击 "Connect Strava" 进行登录授权。
