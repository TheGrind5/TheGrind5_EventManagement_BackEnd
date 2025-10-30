@echo off
echo ========================================
echo KIEM TRA HE THONG THEGRIND5
echo ========================================
echo.

echo [1] Kiem tra Backend dang chay...
curl -s http://localhost:5000/swagger/index.html >nul 2>&1
if %ERRORLEVEL% == 0 (
    echo [OK] Backend dang chay tai http://localhost:5000
) else (
    echo [ERROR] Backend KHONG chay. Vui long khoi dong backend!
)
echo.

echo [2] Kiem tra Frontend dang chay...
curl -s http://localhost:3000 >nul 2>&1
if %ERRORLEVEL% == 0 (
    echo [OK] Frontend dang chay tai http://localhost:3000
) else (
    echo [ERROR] Frontend KHONG chay. Vui long khoi dong frontend!
)
echo.

echo [3] Kiem tra API Event...
curl -s http://localhost:5000/api/Event >nul 2>&1
if %ERRORLEVEL% == 0 (
    echo [OK] API Event hoat dong binh thuong
) else (
    echo [ERROR] API Event KHONG hoat dong. Co the:
    echo   - Backend chua khoi dong xong
    echo   - Database chua duoc tao
    echo   - Loi ket noi database
)
echo.

echo ========================================
echo HUONG DAN SUA LOI:
echo ========================================
echo.
echo Neu Backend/Frontend chua chay:
echo   1. Chay: run.bat
echo   2. Doi 30-60 giay
echo   3. Chay lai script nay
echo.
echo Neu Database chua duoc tao:
echo   1. Mo SQL Server Management Studio
echo   2. Chay file: TheGrind5_Query.sql
echo   3. Chay file: SampleData_Insert.sql
echo   4. Khoi dong lai Backend
echo.
pause

