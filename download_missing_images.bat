@echo off
REM ========================================
REM Download Missing Images - Batch Wrapper
REM ========================================

echo.
echo ========================================
echo  Downloading Placeholder Images
echo ========================================
echo.
echo Starting PowerShell script...
echo.

REM Chạy PowerShell script với quyền Execution Policy bypass
powershell.exe -ExecutionPolicy Bypass -NoProfile -File "%~dp0download_missing_images.ps1"

REM Giữ cửa sổ mở để xem kết quả
echo.
echo.
echo Press any key to close...
pause > nul

