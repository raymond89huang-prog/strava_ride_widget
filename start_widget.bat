@echo off
setlocal

echo Checking .NET SDK availability...

REM Check if dotnet is installed
dotnet --version >nul 2>&1
if %errorlevel% neq 0 (
    echo [ERROR] .NET SDK is not installed.
    goto :InstallDotNet
)

echo Building RideBoard...
dotnet build rideboard\widget\RideBoard.Widget.csproj -c Debug
if %errorlevel% neq 0 (
    echo Build failed.
    pause
    exit /b
)

echo Starting RideBoard Widget...
REM Start the executable directly so we don't keep a console window open
start "" "rideboard\widget\bin\Debug\net10.0-windows\RideBoard.Widget.exe"

goto :End

:InstallDotNet
echo.
echo ========================================================
echo  RideBoard requires .NET 10.0 SDK
echo ========================================================
echo.
pause
winget install Microsoft.DotNet.SDK.10
pause

:End
exit
