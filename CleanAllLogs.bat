@echo off
echo Cleaning all log files in the project...
echo.

cd /d "C:\UI_Modulraztion\rst-erp-backend\src"

echo Deleting from Svc.HRM...
del /s /q "Svc.HRM\*\Logs\*.txt" 2>nul

echo Deleting from Svc.Finance...
del /s /q "Svc.Finance\*\Logs\*.txt" 2>nul

echo.
echo All log files deleted!
pause