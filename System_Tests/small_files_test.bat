
cd downloaded
del /q *.*

cd ..

echo "Small files download"
"..\Output\Extget.exe" -i .\small_files.txt -o downloaded -c 100
if %ERRORLEVEL% GEQ 1 EXIT /B 1

echo "Passed"



