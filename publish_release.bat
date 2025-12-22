@echo off
setlocal
chcp 65001 >nul

echo ========================================================
echo  RideBoard Release Packager
echo ========================================================
echo.

REM 1. Clean previous build
echo Killing running instances...
taskkill /F /IM RideBoard.Widget.exe 2>nul
timeout /t 1 /nobreak >nul

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
echo Cleaning sensitive user data from distribution...
if exist "dist\server\data\tokens.json" del "dist\server\data\tokens.json"
if exist "dist\server\data\cache.json" del "dist\server\data\cache.json"

echo Creating template config.json...
echo {> "dist\server\config.json"
echo     "client_id": "YOUR_CLIENT_ID_HERE",>> "dist\server\config.json"
echo     "client_secret": "YOUR_CLIENT_SECRET_HERE",>> "dist\server\config.json"
echo     "redirect_uri": "http://localhost:8787/callback">> "dist\server\config.json"
echo }>> "dist\server\config.json"

echo.
echo Copying Documentation...
copy "使用说明_README.txt" "dist\"

echo.
echo Cleaning up unnecessary files...
del "dist\*.pdb" 2>nul

echo.
echo ========================================================
echo  SUCCESS! Release package created in 'dist' folder.
echo  You can now zip the 'dist' folder and share it.
echo ========================================================
echo.
pause
