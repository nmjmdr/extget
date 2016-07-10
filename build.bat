echo "Extget build..."

cd .\Output
del /q *.*
mkdir plugins

cd ..

MsBuild.exe Extget.sln /t:Clean;Rebuild /p:Configuration=Release
if %ERRORLEVEL% GEQ 1 EXIT /B 1

copy .\Extget\bin\Release\*.exe .\Output\
copy .\Extget\bin\Release\*.dll .\Output\
copy .\Extget\bin\Release\*.config .\Output\


copy .\HttpHandlerPlugin\bin\Release\*.dll .\Output\plugins
copy .\FileHandlerPlugin\bin\Release\*.dll .\Output\plugins

copy .\Ouput .\System_Test

echo "Done"


