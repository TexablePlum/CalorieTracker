@echo off
title Serwer dokumentacji CalorieTracker
cd /d "%~dp0"

echo Uruchamianie serwera dokumentacji...
echo.
echo Za chwile otworze przegladarke...
echo Adres: http://localhost:8080
echo.

REM Uruchom serwer
start /B docfx serve _site

REM Poczekaj i otwórz przegladarke
timeout /t 3 /nobreak >nul
start http://localhost:8080

echo Uruchomiono serwer dokumentacji
echo Aby zamknac, nacisnij dowolny klawisz...
pause >nul