
cd downloaded
del /q *.*

cd ..

echo "Large and small files download"
"..\Output\Extget.exe" -i .\large_and_small.txt -o downloaded -c 100
if %ERRORLEVEL% GEQ 1 EXIT /B 1

echo "Passed"



