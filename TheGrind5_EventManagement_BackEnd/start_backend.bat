@echo off
echo ========================================
echo TheGrind5 Event Management System
echo Starting Backend (ASP.NET Core)...
echo ========================================

cd /d "C:\Users\Lenovo\OneDrive\Desktop\TheGrind5\TheGrind5_EventManagement_BackEnd\src"
if not exist "TheGrind5_EventManagement.csproj" (
    echo ERROR: Backend project not found!
    pause
    exit /b 1
)

echo.
echo Starting backend on http://localhost:5000...
dotnet run --urls "http://localhost:5000"
echo.
echo ========================================
echo ✅ Backend process finished.
echo ========================================
pause > nul
