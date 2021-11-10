@echo off
xcopy /s/e/y %0\..\Saucisse_bot.bots\Sources %0\..\Saucisse_bot.bots\bin\Release\netcoreapp3.1\Sources
devenv Saucisse_bot.sln /build Release

pause