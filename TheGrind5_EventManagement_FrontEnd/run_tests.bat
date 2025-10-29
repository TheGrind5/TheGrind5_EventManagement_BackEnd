@echo off
echo ========================================
echo 🧪 Running Jest Unit Tests for Shopping Cart
echo ========================================
echo.

echo 📦 Installing dependencies...
call npm install

echo.
echo 🚀 Running tests...
call npm test -- --watchAll=false --verbose

echo.
echo ========================================
echo ✅ Test execution completed!
echo ========================================
pause
