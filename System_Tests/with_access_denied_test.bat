
cd downloaded
del /q *.*

cd ..

echo "One with access denied download"
"..\Output\Extget.exe" -i .\with_access_denied.txt -o downloaded -c 100
if %ERRORLEVEL% GEQ 1 EXIT /B 1

echo "Passed"



