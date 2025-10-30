@echo off

echo Starting Backend Server...
start "Backend" cmd /k "cd /d %~dp0src && dotnet run --urls http://localhost:5000"
echo.
echo Starting Frontend Server...
start "Frontend" cmd /k "cd /d %~dp0TheGrind5_EventManagement_FrontEnd && npm start"
echo.
echo Backend: http://localhost:5000
echo Frontend: http://localhost:3000
echo.
pause
