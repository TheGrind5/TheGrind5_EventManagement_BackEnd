@echo off
echo ========================================
echo TheGrind5 Event Management System
echo Starting Full Stack Application...
echo ========================================

echo.
echo [1/2] Starting Backend (ASP.NET Core)...
start "Backend" cmd /k "cd /d C:\Users\PHOENIX\Desktop\TheGrind5\src && dotnet run --urls http://localhost:5000"

echo [2/2] Starting Frontend (React)...
start "Frontend" cmd /k "cd /d C:\Users\PHOENIX\Desktop\5GrindThe\TheGrind5_EventManagement_FrontEnd && npm start"

echo.
echo Waiting for services to start...
timeout /t 15 /nobreak > nul

echo.
echo [3/3] Opening browsers...
start http://localhost:3000
start http://localhost:5000/swagger

echo.
echo ========================================
echo ✅ All services started successfully!
echo Frontend: http://localhost:3000
echo Backend API: http://localhost:5000
echo Swagger UI: http://localhost:5000/swagger
echo ========================================
echo.
echo Press any key to exit...
pause > nul
