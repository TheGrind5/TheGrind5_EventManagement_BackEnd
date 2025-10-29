@echo off
echo Running TheGrind5 Unit Tests...
echo.

cd /d "%~dp0"

echo Building test project...
dotnet build
if %ERRORLEVEL% neq 0 (
    echo Build failed!
    pause
    exit /b 1
)

echo.
echo Running tests...
dotnet test --verbosity normal

echo.
echo Tests completed!
pause
