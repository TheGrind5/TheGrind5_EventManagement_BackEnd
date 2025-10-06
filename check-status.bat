@echo off
echo Checking TheGrind5 Event Management System Status...
echo.

echo Checking Backend Server (Port 5000):
netstat -an | findstr ":5000" >nul
if %errorlevel% == 0 (
    echo [OK] Backend is running on port 5000
    netstat -an | findstr ":5000"
) else (
    echo [STOPPED] Backend is not running
)

echo.
echo Checking Frontend Server (Port 3000):
netstat -an | findstr ":3000" >nul
if %errorlevel% == 0 (
    echo [OK] Frontend is running on port 3000
    netstat -an | findstr ":3000"
) else (
    echo [STOPPED] Frontend is not running
)

echo.
echo Checking Processes:
echo.
echo Dotnet processes (Backend):
tasklist /fi "imagename eq dotnet.exe" 2>nul | findstr dotnet.exe
if %errorlevel% neq 0 (
    echo No dotnet processes found
)

echo.
echo Node processes (Frontend):
tasklist /fi "imagename eq node.exe" 2>nul | findstr node.exe
if %errorlevel% neq 0 (
    echo No node processes found
)

echo.
echo URLs:
echo Backend API: http://localhost:5000
echo Backend Swagger: http://localhost:5000/swagger
echo Frontend: http://localhost:3000
echo.
pause
