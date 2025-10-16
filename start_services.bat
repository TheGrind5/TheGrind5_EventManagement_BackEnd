@echo off
echo Starting Backend API...
start "Backend API" cmd /k "cd /d %~dp0\src && dotnet run"

timeout /t 5

echo Starting Frontend React...
start "Frontend React" cmd /k "cd /d %~dp0..\TheGrind5_EventManagement_FrontEnd && npm start"

echo Both services are starting...
echo Backend: http://localhost:5000
echo Frontend: http://localhost:3000
echo.
echo Press any key to exit...
pause
