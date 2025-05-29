@echo off
REM Save the current directory
set "current_dir=%cd%"

REM Navigate to the target directory
cd /d "%current_dir%\Samples\Avalonia.PropertyGrid.Samples.Browser"

REM Check for the --clean argument
if "%1"=="--clean" (
    echo Cleaning bin and obj directories...
    rd /s /q "bin"
    rd /s /q "obj"
)

REM Execute the dotnet publish command
dotnet publish

REM Check if the publish was successful
if %errorlevel% neq 0 (
    echo Publish failed, exiting script.
    exit /b %errorlevel%
)

REM Navigate to the publish directory
cd /d "%current_dir%\Samples\Avalonia.PropertyGrid.Samples.Browser\bin\Release\net9.0-browser\publish\wwwroot"

REM Recursively delete all .br and .gz files
for /r %%i in (*.br *.gz) do del "%%i"

REM Open the directory in Windows Explorer
explorer .

echo Operation completed.