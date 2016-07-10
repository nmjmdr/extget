
cd downloaded
del /q *.*

cd ..

echo "Huge large and small files download"
"..\Output\Extget.exe" -i .\huge_large_small.txt -o downloaded -c 100
if %ERRORLEVEL% GEQ 1 EXIT /B 1

echo "Passed"



