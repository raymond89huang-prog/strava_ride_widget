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
echo Cleaning up unnecessary files...
del "dist\*.pdb" 2>nul

echo.
echo ========================================================
echo  SUCCESS! Release package created in 'dist' folder.
echo  You can now zip the 'dist' folder and share it.
echo ========================================================
echo.
pause
