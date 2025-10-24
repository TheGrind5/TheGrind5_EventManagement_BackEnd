@echo off
echo 🚀 Starting Unit Tests for Buy Ticket Flow...
echo.

echo 📁 Running Backend (.NET) tests...
cd TheGrind5_EventManagement.Tests
dotnet test --collect:"XPlat Code Coverage"
if %errorlevel% neq 0 (
    echo ❌ Backend tests failed!
    pause
    exit /b 1
)
echo ✅ Backend tests completed successfully!
echo.

echo 📁 Running Frontend (React) tests...
cd ..\TheGrind5_EventManagement_FrontEnd
npm run test:coverage
if %errorlevel% neq 0 (
    echo ❌ Frontend tests failed!
    pause
    exit /b 1
)
echo ✅ Frontend tests completed successfully!
echo.

echo 🎉 All tests completed!
echo.
echo 📊 Test Results Summary:
echo - Backend: Check TestResults folder for coverage
echo - Frontend: Check src/__tests__/coverage/lcov-report/index.html
echo.
echo 💡 To run tests individually:
echo   Backend:  dotnet test
echo   Frontend: npm test
echo.
pause
