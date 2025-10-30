@echo off
echo ========================================
echo TAO DATABASE THEGRIND5
echo ========================================
echo.

echo [1] Tao database tu TheGrind5_Query.sql...
sqlcmd -S "DESKTOP-21QAC9D\SQLEXPRESS" -E -i "TheGrind5_Query.sql" -C
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] Khong the tao database!
    echo Vui long:
    echo   1. Mo SQL Server Management Studio (SSMS)
    echo   2. Ket noi toi server: DESKTOP-21QAC9D\SQLEXPRESS
    echo   3. Chay file: TheGrind5_Query.sql
    echo   4. Chay file: SampleData_Insert.sql
    pause
    exit /b 1
)
echo [OK] Da tao database thanh cong!
echo.

echo [2] Insert sample data tu SampleData_Insert.sql...
sqlcmd -S "DESKTOP-21QAC9D\SQLEXPRESS" -E -i "SampleData_Insert.sql" -C
if %ERRORLEVEL% NEQ 0 (
    echo [WARNING] Co loi khi insert data, nhung database da duoc tao
)
echo [OK] Da insert sample data thanh cong!
echo.

echo ========================================
echo HOAN TAT!
echo ========================================
echo Database EventDB da san sang.
echo Bay gio co the chay: run.bat
echo.
pause

