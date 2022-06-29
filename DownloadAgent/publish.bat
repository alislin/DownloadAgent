call web.bat
cd ..
del  obj\**,bin\** -recurse
7z a ./Resources/wwwroot.7z wwwroot
dotnet clean -c release
dotnet publish -c release -r win-x64 /p:publishsinglefile=true /p:publishtrimmed=true