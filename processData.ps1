$targetDir = dir -File products.xml/
$outputDir = ".\output\"
foreach ($file in  $targetDir) {
    $isFile = $file.Mode.Contains("a")
    if ($isFile) {
        $fileName = $file.FullName
        [System.Console]::ForegroundColor = "Green"
        write "Trying to process $fileName"
        [System.Console]::ResetColor()
        .\BestBuyProductDataToCSV\bin\Debug\BestBuyProductDataToCSV.exe $fileName $outputDir
    }
}

# Concatenating data for Arjun 
[System.Console]::ForegroundColor = "Green"
write "Concatenating data for Arjun..."
[System.Console]::ResetColor()
cd .\output\arjun\
Get-Content *.csv | Out-File ..\ArjunData.csv
cd ..\..
[System.Console]::ForegroundColor = "Green"
write "Done."
[System.Console]::ResetColor()

# Concatenating data for upload to db
[System.Console]::ForegroundColor = "Green"
write "Concatenating data for upload to db..."
[System.Console]::ResetColor()
cd .\output\outputcsv\
Get-Content *.csv | Out-File ..\ProductData.csv
cd ..\..
[System.Console]::ForegroundColor = "Green"
write "Done."
[System.Console]::ResetColor()

