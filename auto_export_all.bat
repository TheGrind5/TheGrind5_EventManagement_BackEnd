@echo off
REM ========================================
REM AUTO EXPORT ALL - BATCH FILE WRAPPER
REM ========================================
REM Double-click file này để chạy auto export
REM ========================================

echo.
echo ========================================
echo  AUTO EXPORT SAMPLE DATA
echo ========================================
echo.
echo Starting PowerShell script...
echo.

REM Chạy PowerShell script với quyền Execution Policy bypass
powershell.exe -ExecutionPolicy Bypass -NoProfile -File "%~dp0auto_export_all_simple.ps1"

REM Giữ cửa sổ mở để xem kết quả
echo.
echo.
echo Press any key to close...
pause > nul

