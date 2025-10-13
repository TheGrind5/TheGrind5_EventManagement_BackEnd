@echo off
echo Starting TheGrind5 Event Management System...
echo.
echo Backend: Starting on http://localhost:5000
echo Frontend: Starting on http://localhost:3000
echo.
start "Backend" cmd /k "cd /d %~dp0\src && dotnet run"
ping 127.0.0.1 -n 4 >nul
start "Frontend" cmd /k "cd /d %~dp0\..\FrontEnd && npm start"
echo.
echo Both services are starting...
echo Backend will be available at: http://localhost:5000
echo Frontend will be available at: http://localhost:3000
echo Swagger UI will be available at: http://localhost:5000/swagger
echo.
echo Press any key to close this window...
pause >nul
