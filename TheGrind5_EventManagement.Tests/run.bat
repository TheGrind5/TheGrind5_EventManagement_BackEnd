@echo off
setlocal enabledelayedexpansion

cd /d "%~dp0"

:menu
echo.
echo ========================================
echo TheGrind5 Event Management - Test Suite
echo ========================================
echo 1. Run ALL tests
echo 2. Run Thiên tests
echo 3. Run A Duy tests
echo 4. Run Khanh tests
echo 5. Run Minh tests
echo 6. Run Tân tests
echo 7. Show Summary
echo 8. Generate Buy Ticket Coverage Report
echo 9. Exit
echo ========================================
set /p choice="Enter choice (1-9): "

if "%choice%"=="1" goto run_all
if "%choice%"=="2" goto run_thien
if "%choice%"=="3" goto run_a_duy
if "%choice%"=="4" goto run_khanh
if "%choice%"=="5" goto run_minh
if "%choice%"=="6" goto run_tan
if "%choice%"=="7" goto show_summary
if "%choice%"=="8" goto generate_coverage
if "%choice%"=="9" goto exit
echo Invalid choice!
goto menu

:run_all
call :clean_results
call :build_project
if !ERRORLEVEL! neq 0 goto :eof
dotnet test --verbosity minimal --logger "trx;LogFileName=TestResults_All.trx" --results-directory TestResults --collect:"XPlat Code Coverage" --settings coverlet.runsettings
echo.
echo Test results saved to: TestResults\TestResults_All.trx
echo Buy Ticket Flow coverage: TestResults folder
goto menu

:run_thien
call :clean_results
call :build_project
if !ERRORLEVEL! neq 0 goto :eof
dotnet test --filter "Thien" --verbosity minimal --logger "trx;LogFileName=TestResults_Thien.trx" --results-directory TestResults --collect:"XPlat Code Coverage" --settings coverlet.runsettings
echo.
echo Test results saved to: TestResults\TestResults_Thien.trx
echo Buy Ticket Flow coverage: TestResults folder
goto menu

:run_a_duy
call :clean_results
call :build_project
if !ERRORLEVEL! neq 0 goto :eof
dotnet test --filter "A_Duy" --verbosity minimal --logger "trx;LogFileName=TestResults_A_Duy.trx" --results-directory TestResults --collect:"XPlat Code Coverage" --settings coverlet.runsettings
echo.
echo Test results saved to: TestResults\TestResults_A_Duy.trx
echo Buy Ticket Flow coverage: TestResults folder
goto menu

:run_khanh
call :clean_results
call :build_project
if !ERRORLEVEL! neq 0 goto :eof
dotnet test --filter "Khanh" --verbosity minimal --logger "trx;LogFileName=TestResults_Khanh.trx" --results-directory TestResults --collect:"XPlat Code Coverage" --settings coverlet.runsettings
echo.
echo Test results saved to: TestResults\TestResults_Khanh.trx
echo Buy Ticket Flow coverage: TestResults folder
goto menu

:run_minh
call :clean_results
call :build_project
if !ERRORLEVEL! neq 0 goto :eof
dotnet test --filter "Minh" --verbosity minimal --logger "trx;LogFileName=TestResults_Minh.trx" --results-directory TestResults --collect:"XPlat Code Coverage" --settings coverlet.runsettings
echo.
echo Test results saved to: TestResults\TestResults_Minh.trx
echo Buy Ticket Flow coverage: TestResults folder
goto menu

:run_tan
call :clean_results
call :build_project
if !ERRORLEVEL! neq 0 goto :eof
dotnet test --filter "Tan" --verbosity minimal --logger "trx;LogFileName=TestResults_Tan.trx" --results-directory TestResults --collect:"XPlat Code Coverage" --settings coverlet.runsettings
echo.
echo Test results saved to: TestResults\TestResults_Tan.trx
echo Buy Ticket Flow coverage: TestResults folder
goto menu

:show_summary
call :clean_results
call :build_project
if !ERRORLEVEL! neq 0 goto :eof
dotnet test --verbosity minimal --logger "trx;LogFileName=TestResults_Summary.trx" --results-directory TestResults --collect:"XPlat Code Coverage" --settings coverlet.runsettings
echo.
echo Test results saved to: TestResults\TestResults_Summary.trx
echo Buy Ticket Flow coverage: TestResults folder
call :show_summary_info
goto menu

:clean_results
echo Cleaning old test results and coverage data...
if exist TestResults rmdir /s /q TestResults
if exist TestResults*.trx del TestResults*.trx
if exist *.trx del *.trx
if exist coverage rmdir /s /q coverage
if exist CoverageReport rmdir /s /q CoverageReport
if exist *.coverage del *.coverage
echo Old results and coverage data cleaned!
goto :eof

:build_project
echo Building project...
cd /d "%~dp0\.."
dotnet build --verbosity minimal
if !ERRORLEVEL! neq 0 (
    echo Build failed!
    pause
    exit /b 1
)
cd /d "%~dp0"
goto :eof

:show_summary_info
echo.
echo ========================================
echo Test Summary Report
echo ========================================
echo.
echo ✅ Thiên (OrderService Core): 10/10 COMPLETED
echo ⚠️ A Duy (OrderService Extended): 6/10 (thiếu 4)
echo 🎉 Khanh (TicketService Core): 14/10 VƯỢT MỤC TIÊU!
echo 🎉 Minh (TicketService + Controller): 20/10 VƯỢT MỤC TIÊU!
echo ⚠️ Tân (Controller + Wallet + Repo): 9/10 (thiếu 1)
echo.
echo ========================================
echo Overall: 59/50 test cases (118% - VƯỢT MỤC TIÊU!)
echo ========================================
echo.
goto :eof

:generate_coverage
echo.
echo ========================================
echo Generating Buy Ticket Flow Coverage Report
echo ========================================
echo.

REM Check if TestResults folder exists
if not exist TestResults (
    echo No test results found! Please run tests first.
    echo.
    pause
    goto menu
)

REM Check if coverage data exists
set coverage_found=0
for /d %%i in (TestResults\*) do (
    if exist "%%i\coverage.cobertura.xml" set coverage_found=1
)

if %coverage_found%==0 (
    echo No coverage data found! Please run tests with coverage first.
    echo.
    pause
    goto menu
)

echo Found coverage data! Generating Buy Ticket Flow coverage report...
echo.

REM Install reportgenerator if not exists
where reportgenerator >nul 2>nul
if %ERRORLEVEL% neq 0 (
    echo Installing ReportGenerator tool...
    dotnet tool install -g dotnet-reportgenerator-globaltool --version 5.2.0
    if !ERRORLEVEL! neq 0 (
        echo Failed to install ReportGenerator!
        pause
        goto menu
    )
)

REM Create coverage report directory
if exist CoverageReport rmdir /s /q CoverageReport
mkdir CoverageReport

REM Generate HTML report
echo Generating HTML coverage report...
reportgenerator -reports:"TestResults\*\coverage.cobertura.xml" -targetdir:"CoverageReport" -reporttypes:"Html"

if !ERRORLEVEL! neq 0 (
    echo Failed to generate coverage report!
    pause
    goto menu
)

echo.
echo ========================================
echo Buy Ticket Flow Coverage Report Generated!
echo ========================================
echo.
echo 📊 HTML Report: CoverageReport\index.html
echo 📁 Coverage Data: TestResults folder
echo 🎯 Coverage Scope: Order, Ticket, Payment, Wallet services only
echo.
echo Opening coverage report in browser...
start CoverageReport\index.html
echo.
pause
goto menu

:exit
exit /b 0