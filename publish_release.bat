@echo off
setlocal

echo ========================================================
echo  RideBoard Release Packager
echo ========================================================
echo.

REM 1. Clean previous build
if exist "dist" rmdir /s /q "dist"
mkdir "dist"

echo Publishing WPF App (Single File)...
dotnet publish rideboard\widget\RideBoard.Widget.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o "dist"

if %errorlevel% neq 0 (
    echo [ERROR] Publish failed.
    pause
    exit /b
)

echo.
echo Copying Server Files...
mkdir "dist\server"
xcopy "rideboard\server" "dist\server" /E /I /Y

echo.
echo Cleaning up sensitive/unnecessary files...
del "dist\*.pdb" 2>nul
REM Remove local config with real keys
if exist "dist\server\config.json" del "dist\server\config.json"
REM Remove local cache/tokens
if exist "dist\server\data\*.json" del "dist\server\data\*.json"

echo.
echo Creating default config.json...
(
echo {
echo     "client_id": "YOUR_CLIENT_ID_HERE",
echo     "client_secret": "YOUR_CLIENT_SECRET_HERE",
echo     "redirect_uri": "http://localhost:8787/callback"
echo }
) > "dist\server\config.json"

echo.
echo Copying README...
copy "使用说明_README.txt" "dist\使用说明_README.txt"

echo.
echo ========================================================
echo  SUCCESS! Release package created in 'dist' folder.
echo  You can now zip the 'dist' folder and share it.
echo ========================================================
echo.
pause
