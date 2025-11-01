@echo off
setlocal enabledelayedexpansion

REM Lấy đường dẫn thư mục hiện tại (thư mục chứa run.bat - Backend)
set "BACKEND_DIR=%~dp0"
set "BACKEND_SRC=%BACKEND_DIR%src"

REM Tìm thư mục Frontend - tìm trong thư mục cha (cùng cấp với Backend)
set "FRONTEND_DIR="
set "PARENT_DIR=%BACKEND_DIR%.."

REM Trước tiên thử tìm chính xác tên TheGrind5_EventManagement_FrontEnd
if exist "%PARENT_DIR%\TheGrind5_EventManagement_FrontEnd\package.json" (
    set "FRONTEND_DIR=%PARENT_DIR%\TheGrind5_EventManagement_FrontEnd"
    goto :found_frontend
)

REM Tìm các thư mục cùng cấp với Backend có tên chứa "TheGrind5" và "FrontEnd"
for /d %%d in ("%PARENT_DIR%\*TheGrind5*FrontEnd*") do (
    if exist "%%d\package.json" (
        set "FRONTEND_DIR=%%d"
        goto :found_frontend
    )
)

REM Tìm các thư mục cùng cấp có tên chứa "FrontEnd" (fallback)
for /d %%d in ("%PARENT_DIR%\*FrontEnd*") do (
    if exist "%%d\package.json" (
        set "FRONTEND_DIR=%%d"
        goto :found_frontend
    )
)

:found_frontend

REM Hiển thị thông tin nếu không tìm thấy Frontend
if "%FRONTEND_DIR%"=="" (
    echo Warning: Frontend directory not found. Only Backend will be started.
    echo Looking for: %PARENT_DIR%\TheGrind5_EventManagement_FrontEnd
)

REM Kiểm tra Backend
if not exist "%BACKEND_SRC%\Program.cs" (
    echo Error: Backend source directory not found at %BACKEND_SRC%
    exit /b 1
)

REM Chạy Backend (không hiển thị giao diện)
start "TheGrind5 Backend" cmd /k "cd /d %BACKEND_SRC% && dotnet run --urls http://localhost:5000"

REM Chỉ chạy Frontend nếu tìm thấy (không hiển thị giao diện)
if not "%FRONTEND_DIR%"=="" (
    if exist "%FRONTEND_DIR%\package.json" (
        REM Kiểm tra node_modules và các package quan trọng
        set "NEED_INSTALL=0"
        if not exist "%FRONTEND_DIR%\node_modules" (
            set "NEED_INSTALL=1"
        ) else (
            REM Kiểm tra một số package quan trọng có tồn tại không
            if not exist "%FRONTEND_DIR%\node_modules\react-easy-crop" set "NEED_INSTALL=1"
            if not exist "%FRONTEND_DIR%\node_modules\swiper" set "NEED_INSTALL=1"
            if not exist "%FRONTEND_DIR%\node_modules\framer-motion" set "NEED_INSTALL=1"
        )
        
        REM Nếu cần cài đặt thì chạy npm install
        if "!NEED_INSTALL!"=="1" (
            REM Cài đặt dependencies với --legacy-peer-deps để tránh conflict React 19
            cd /d "%FRONTEND_DIR%"
            call npm install --legacy-peer-deps >nul 2>&1
        )
        
        REM Đợi một chút để backend khởi động
        timeout /t 3 /nobreak >nul 2>nul
        
        REM Chạy Frontend
        start "TheGrind5 Frontend" cmd /k "cd /d %FRONTEND_DIR% && npm start"
    )
)

REM Tự động đóng cửa sổ này
exit /b 0
