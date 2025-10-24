@echo off
chcp 65001 >nul
echo Starting TheGrind5 Event Management System...

echo.
echo [1/4] Starting Backend API...
start "Backend API" cmd /k "cd /d C:\Users\Lenovo\OneDrive\Desktop\TheGrind5\TheGrind5_EventManagement_BackEnd\src ^&^& dotnet run"

echo.
echo [2/4] Waiting for Backend to start...
ping 127.0.0.1 -n 15 >nul

echo.
echo [3/4] Starting Frontend React...
start "Frontend React" cmd /k "cd /d C:\Users\Lenovo\OneDrive\Desktop\TheGrind5\TheGrind5_EventManagement_FrontEnd ^&^& npm start"

echo.
echo [4/4] Waiting for Frontend to start...
ping 127.0.0.1 -n 20 >nul

echo.
echo Opening website...
start http://localhost:3000

echo.
echo ==========================================
echo Both services are starting!
echo Frontend: http://localhost:3000
echo Backend: http://localhost:5000
echo ==========================================
echo.
echo Press any key to exit this launcher...
pause >nul
