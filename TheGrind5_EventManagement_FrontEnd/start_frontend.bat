@echo off
echo ========================================
echo TheGrind5 Event Management Frontend
echo Starting React Application...
echo ========================================

cd /d "C:\Users\Lenovo\OneDrive\Desktop\TheGrind5\TheGrind5_EventManagement_FrontEnd"
if not exist "package.json" (
    echo ERROR: Frontend project not found!
    pause
    exit /b 1
)

echo.
echo Starting frontend on http://localhost:3000...
npm start
echo.
echo ========================================
echo ✅ Frontend process finished.
echo ========================================
pause > nul
