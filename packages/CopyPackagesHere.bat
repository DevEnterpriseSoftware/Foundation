@echo off

copy /y ..\src\Core\bin\Debug\*.nupkg .\
copy /y ..\src\Core\bin\Release\*.nupkg .\

copy /y ..\src\Caching\bin\Debug\*.nupkg .\
copy /y ..\src\Caching\bin\Release\*.nupkg .\

pause
exit
