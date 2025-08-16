@echo off
set AUTH_BASE=%1
if "%AUTH_BASE%"=="" set AUTH_BASE=https://localhost:1213
echo Rotating keys at %AUTH_BASE% ...
curl -s -X POST "%AUTH_BASE%/api/keys/rotate"
echo.
