@echo off
setlocal enabledelayedexpansion

REM ========================================
REM TheGrind5 Event Management - Universal Manager
REM All-in-one script for setup and development
REM ========================================

:MAIN_MENU
cls
echo.
echo ========================================
echo   TheGrind5 Event Management System
echo   Universal Manager
echo ========================================
echo.
echo 1. Quick Start (Auto-detect everything)
echo 2. Setup Project (First time only)
echo 3. Start Development
echo 4. Stop All Services
echo 5. Check Status
echo 6. Exit
echo.
set /p choice="Please select an option (1-6): "

if "%choice%"=="1" goto QUICK_START
if "%choice%"=="2" goto SETUP_MENU
if "%choice%"=="3" goto DEV_MENU
if "%choice%"=="4" goto STOP_SERVICES
if "%choice%"=="5" goto CHECK_STATUS
if "%choice%"=="6" goto EXIT
echo Invalid choice. Please try again.
pause
goto MAIN_MENU

REM ========================================
REM QUICK START
REM ========================================

:QUICK_START
cls
echo.
echo ========================================
echo   Quick Start - Auto-detect Everything
echo ========================================
echo.

REM Get the current directory (backend directory)
set BACKEND_DIR=%~dp0
set FRONTEND_DIR=%BACKEND_DIR%..\TheGrind5_EventManagement_FrontEnd

echo [INFO] Backend Directory: %BACKEND_DIR%
echo [INFO] Frontend Directory: %FRONTEND_DIR%
echo.

REM Check if frontend directory exists
if not exist "%FRONTEND_DIR%" (
    echo [ERROR] Frontend directory not found at %FRONTEND_DIR%
    echo Please make sure the frontend project is in the correct location.
    pause
    goto MAIN_MENU
)

REM Auto-detect database setup
echo [STEP 1] Auto-detecting database configuration...

REM Check if Docker is available and running
docker --version >nul 2>&1
if %errorlevel% == 0 (
    echo [DETECTED] Docker is available - using Docker database
    goto DOCKER_START
) else (
    echo [DETECTED] Docker not available - using local database
    goto LOCAL_START
)

REM ========================================
REM SETUP MENU
REM ========================================

:SETUP_MENU
cls
echo.
echo ========================================
echo   Setup Project (First time only)
echo ========================================
echo.
echo 1. Setup with Docker (Recommended)
echo 2. Setup with Local SQL Server
echo 3. Setup with SQL Server Express
echo 4. Check Prerequisites
echo 5. Back to Main Menu
echo.
set /p setup_choice="Please select setup option (1-5): "

if "%setup_choice%"=="1" goto DOCKER_SETUP
if "%setup_choice%"=="2" goto LOCAL_SQL_SETUP
if "%setup_choice%"=="3" goto EXPRESS_SQL_SETUP
if "%setup_choice%"=="4" goto CHECK_PREREQUISITES
if "%setup_choice%"=="5" goto MAIN_MENU
echo Invalid choice. Please try again.
pause
goto SETUP_MENU

REM ========================================
REM DEVELOPMENT MENU
REM ========================================

:DEV_MENU
cls
echo.
echo ========================================
echo   Start Development
echo ========================================
echo.
echo 1. Auto-detect Database
echo 2. Use Docker Database
echo 3. Use Local Database
echo 4. Back to Main Menu
echo.
set /p dev_choice="Please select development option (1-4): "

if "%dev_choice%"=="1" goto AUTO_START
if "%dev_choice%"=="2" goto DOCKER_START
if "%dev_choice%"=="3" goto LOCAL_START
if "%dev_choice%"=="4" goto MAIN_MENU
echo Invalid choice. Please try again.
pause
goto DEV_MENU

REM ========================================
REM SETUP SECTION
REM ========================================

:DOCKER_SETUP
cls
echo.
echo ========================================
echo   Docker Setup (Recommended)
echo ========================================
echo.

echo [STEP 1] Checking Docker installation...
docker --version >nul 2>&1
if %errorlevel% neq 0 (
    echo [ERROR] Docker is not installed or not running.
    echo Please install Docker Desktop from: https://www.docker.com/products/docker-desktop
    pause
    goto MAIN_MENU
)
echo [SUCCESS] Docker is installed and running.

echo.
echo [STEP 2] Starting database with Docker...
cd /d "%~dp0.."
docker-compose -f Docker/docker-compose.dev.yml up -d db

echo.
echo [STEP 3] Waiting for database to initialize...
timeout /t 10 /nobreak > nul

echo.
echo [STEP 4] Running database migrations...
cd /d "%~dp0..\src"
dotnet ef database update

echo.
echo [SUCCESS] Docker setup completed!
echo.
echo Database: localhost:1433
echo Username: sa
echo Password: YourStrong@Passw0rd
echo Database: EventDB
echo.
echo You can now run the application with: dotnet run
echo.
pause
goto MAIN_MENU

:LOCAL_SQL_SETUP
cls
echo.
echo ========================================
echo   Local SQL Server Setup
echo ========================================
echo.

echo [STEP 1] Please provide your SQL Server details:
echo.
set /p DB_SERVER="SQL Server name (e.g., localhost, LAPTOP-XXXXX): "
set /p DB_USER="Username (e.g., sa): "
set /p DB_PASSWORD="Password: "
set /p DB_NAME="Database name (default: EventDB): "

if "%DB_NAME%"=="" set DB_NAME=EventDB

echo.
echo [STEP 2] Updating configuration files...

REM Update appsettings.json
cd /d "%~dp0.."
powershell -Command "(Get-Content 'appsettings.json') -replace 'Server=.*?;', 'Server=%DB_SERVER%;User Id=%DB_USER%;Password=%DB_PASSWORD%;Database=%DB_NAME%;TrustServerCertificate=true;' | Set-Content 'appsettings.json'"

echo [SUCCESS] Configuration updated!
echo.
echo [STEP 3] Testing database connection...
cd /d "%~dp0..\src"
dotnet ef database update

if %errorlevel% == 0 (
    echo [SUCCESS] Database connection successful!
) else (
    echo [ERROR] Database connection failed. Please check your credentials.
)

echo.
pause
goto MAIN_MENU

:EXPRESS_SQL_SETUP
cls
echo.
echo ========================================
echo   SQL Server Express Setup
echo ========================================
echo.

echo [STEP 1] Detecting SQL Server Express instances...
sqlcmd -L >nul 2>&1
if %errorlevel% neq 0 (
    echo [ERROR] SQL Server Express is not installed or not accessible.
    echo Please install SQL Server Express from: https://www.microsoft.com/en-us/sql-server/sql-server-downloads
    pause
    goto MAIN_MENU
)

echo [INFO] Available SQL Server instances:
sqlcmd -L

echo.
echo [STEP 2] Please provide your SQL Server Express details:
echo.
set /p DB_SERVER="SQL Server instance (e.g., localhost\SQLEXPRESS): "
set /p DB_USER="Username (default: sa): "
set /p DB_PASSWORD="Password: "

if "%DB_USER%"=="" set DB_USER=sa

echo.
echo [STEP 3] Updating configuration...
cd /d "%~dp0.."
powershell -Command "(Get-Content 'appsettings.json') -replace 'Server=.*?;', 'Server=%DB_SERVER%;User Id=%DB_USER%;Password=%DB_PASSWORD%;Database=EventDB;TrustServerCertificate=true;' | Set-Content 'appsettings.json'"

echo [SUCCESS] Configuration updated!
echo.
echo [STEP 4] Testing connection and running migrations...
cd /d "%~dp0..\src"
dotnet ef database update

if %errorlevel% == 0 (
    echo [SUCCESS] Setup completed successfully!
) else (
    echo [ERROR] Setup failed. Please check your SQL Server configuration.
)

echo.
pause
goto MAIN_MENU

:CHECK_PREREQUISITES
cls
echo.
echo ========================================
echo   Prerequisites Check
echo ========================================
echo.

echo [CHECKING] .NET 8 SDK...
dotnet --version >nul 2>&1
if %errorlevel% == 0 (
    for /f "tokens=1" %%i in ('dotnet --version') do echo [SUCCESS] .NET %%i is installed
) else (
    echo [ERROR] .NET 8 SDK is not installed
    echo Please install from: https://dotnet.microsoft.com/download/dotnet/8.0
)

echo.
echo [CHECKING] Docker...
docker --version >nul 2>&1
if %errorlevel% == 0 (
    for /f "tokens=3" %%i in ('docker --version') do echo [SUCCESS] Docker %%i is installed
) else (
    echo [WARNING] Docker is not installed (optional for Docker setup)
)

echo.
echo [CHECKING] SQL Server tools...
sqlcmd -? >nul 2>&1
if %errorlevel% == 0 (
    echo [SUCCESS] SQL Server tools are available
) else (
    echo [WARNING] SQL Server tools not found (optional for local setup)
)

echo.
echo [CHECKING] Node.js (for frontend)...
node --version >nul 2>&1
if %errorlevel% == 0 (
    for /f "tokens=1" %%i in ('node --version') do echo [SUCCESS] Node.js %%i is installed
) else (
    echo [WARNING] Node.js is not installed (required for frontend)
)

echo.
echo [CHECKING] Git...
git --version >nul 2>&1
if %errorlevel% == 0 (
    for /f "tokens=3" %%i in ('git --version') do echo [SUCCESS] Git %%i is installed
) else (
    echo [WARNING] Git is not installed
)

echo.
pause
goto MAIN_MENU

REM ========================================
REM DEVELOPMENT SECTION
REM ========================================

:AUTO_START
cls
echo.
echo ========================================
echo   Auto-Detection Mode
echo ========================================
echo.

REM Get the current directory (backend directory)
set BACKEND_DIR=%~dp0
set FRONTEND_DIR=%BACKEND_DIR%..\TheGrind5_EventManagement_FrontEnd

echo [INFO] Backend Directory: %BACKEND_DIR%
echo [INFO] Frontend Directory: %FRONTEND_DIR%
echo.

REM Check if frontend directory exists
if not exist "%FRONTEND_DIR%" (
    echo [ERROR] Frontend directory not found at %FRONTEND_DIR%
    echo Please make sure the frontend project is in the correct location.
    pause
    goto MAIN_MENU
)

REM Auto-detect database setup
echo [STEP 1] Auto-detecting database configuration...

REM Check if Docker is available and running
docker --version >nul 2>&1
if %errorlevel% == 0 (
    echo [DETECTED] Docker is available - using Docker database
    goto DOCKER_START
) else (
    echo [DETECTED] Docker not available - using local database
    goto LOCAL_START
)

:DOCKER_START
cls
echo.
echo ========================================
echo   Starting with Docker Database
echo ========================================
echo.

set BACKEND_DIR=%~dp0
set FRONTEND_DIR=%BACKEND_DIR%..\TheGrind5_EventManagement_FrontEnd

echo [STEP 1] Starting Docker database...
cd /d "%~dp0.."
docker-compose -f Docker/docker-compose.dev.yml up -d db

echo [STEP 2] Waiting for database to initialize...
timeout /t 10 /nobreak > nul

echo [STEP 3] Running database migrations...
cd /d "%~dp0..\src"
dotnet ef database update

echo [STEP 4] Starting Backend Server...
cd /d "%~dp0..\src"
start "TheGrind5-Backend" cmd /k "cd /d %BACKEND_DIR%\src && echo Starting Backend Server with Docker DB... && dotnet run --environment Docker"

echo [STEP 5] Waiting for backend to initialize...
timeout /t 5 /nobreak > nul

echo [STEP 6] Starting Frontend Server...
start "TheGrind5-Frontend" cmd /k "cd /d %FRONTEND_DIR% && echo Starting Frontend Server... && npm start"

echo.
echo [SUCCESS] Development environment started with Docker!
echo.
echo Backend API: http://localhost:5000/api/Event
echo Backend Swagger: http://localhost:5000/swagger
echo Frontend: http://localhost:3000
echo Database: localhost:1433 (Docker)
echo.
pause
goto MAIN_MENU

:LOCAL_START
cls
echo.
echo ========================================
echo   Starting with Local Database
echo ========================================
echo.

set BACKEND_DIR=%~dp0
set FRONTEND_DIR=%BACKEND_DIR%..\TheGrind5_EventManagement_FrontEnd

echo [STEP 1] Checking local database connection...
cd /d "%~dp0..\src"
dotnet ef database update >nul 2>&1
if %errorlevel% neq 0 (
    echo [ERROR] Database connection failed!
    echo Please run setup first (options 1-3) to configure your database.
    pause
    goto MAIN_MENU
)

echo [SUCCESS] Database connection verified!

echo [STEP 2] Starting Backend Server...
cd /d "%~dp0..\src"
start "TheGrind5-Backend" cmd /k "cd /d %BACKEND_DIR%\src && echo Starting Backend Server with Local DB... && dotnet run"

echo [STEP 3] Waiting for backend to initialize...
timeout /t 5 /nobreak > nul

echo [STEP 4] Starting Frontend Server...
start "TheGrind5-Frontend" cmd /k "cd /d %FRONTEND_DIR% && echo Starting Frontend Server... && npm start"

echo.
echo [SUCCESS] Development environment started with Local Database!
echo.
echo Backend API: http://localhost:5000/api/Event
echo Backend Swagger: http://localhost:5000/swagger
echo Frontend: http://localhost:3000
echo.
pause
goto MAIN_MENU

:STOP_SERVICES
cls
echo.
echo ========================================
echo   Stopping All Services
echo ========================================
echo.

call :KILL_PROCESSES

echo.
echo [SUCCESS] All services have been stopped.
echo.
pause
goto MAIN_MENU

:CHECK_STATUS
cls
echo.
echo ========================================
echo   System Status Check
echo ========================================
echo.

echo [CHECKING] Backend Server (Port 5000):
netstat -an | findstr ":5000" >nul
if %errorlevel% == 0 (
    echo [RUNNING] Backend is active on port 5000
) else (
    echo [STOPPED] Backend is not running
)

echo.
echo [CHECKING] Frontend Server (Port 3000):
netstat -an | findstr ":3000" >nul
if %errorlevel% == 0 (
    echo [RUNNING] Frontend is active on port 3000
) else (
    echo [STOPPED] Frontend is not running
)

echo.
echo [CHECKING] Database (Port 1433):
netstat -an | findstr ":1433" >nul
if %errorlevel% == 0 (
    echo [RUNNING] Database is active on port 1433
) else (
    echo [STOPPED] Database is not running
)

echo.
echo [CHECKING] Docker containers:
docker ps --filter "name=thegrind5" --format "table {{.Names}}\t{{.Status}}" 2>nul

echo.
echo [URLS]
echo Backend API: http://localhost:5000/api/Event
echo Backend Swagger: http://localhost:5000/swagger
echo Frontend: http://localhost:3000
echo.
pause
goto MAIN_MENU

:KILL_PROCESSES
echo   - Force killing all development processes...

REM Kill by process name
echo   - Killing dotnet processes (Backend)...
taskkill /f /im dotnet.exe >nul 2>&1
taskkill /f /im TheGrind5_EventManagement.exe >nul 2>&1

echo   - Killing node processes (Frontend)...
taskkill /f /im node.exe >nul 2>&1
taskkill /f /im npm.exe >nul 2>&1

echo   - Closing development windows...
taskkill /f /fi "WINDOWTITLE eq TheGrind5-Backend*" >nul 2>&1
taskkill /f /fi "WINDOWTITLE eq TheGrind5-Frontend*" >nul 2>&1

REM Force kill by port
echo   - Force cleaning up ports...
for /f "tokens=5" %%a in ('netstat -ano ^| findstr ":5000"') do (
    if not "%%a"=="0" (
        taskkill /f /pid %%a >nul 2>&1
    )
)

for /f "tokens=5" %%a in ('netstat -ano ^| findstr ":3000"') do (
    if not "%%a"=="0" (
        taskkill /f /pid %%a >nul 2>&1
    )
)

echo   - Force cleanup completed.
goto :eof

:EXIT
cls
echo.
echo ========================================
echo   Goodbye!
echo ========================================
echo.
echo Thank you for using TheGrind5 Universal Manager.
echo.
timeout /t 2 /nobreak > nul
exit /b 0
