
cd downloaded
del /q *.*

cd ..

echo "Throttled concuurency - one at a time"
"..\Output\Extget.exe" -i .\small_files.txt -o downloaded -c 1
if %ERRORLEVEL% GEQ 1 EXIT /B 1

echo "Passed"



