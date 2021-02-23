$scriptPath =  $PSScriptRoot
cd $scriptPath
Remove-Item -Path $scriptPath\TestResults\* -include *.trx
Remove-Item -Path $scriptPath\TestResults\* -include *.opencover.xml


dotnet sonarscanner begin  /k:"PQC" `
    /d:sonar.cs.opencover.reportsPaths="./TestResults/coverage.opencover.xml" `
    /d:sonar.cs.vstest.reportsPaths="./TestResults/*.trx" `
    /d:sonar.host.url="http://localhost:8000" `
    /d:sonar.coverage.exclusions="**/Test*/**" `
    /d:sonar.exclusions="**/wwwroot/**, **/obj/**, **/bin/**, **/Migrations/**,**/Resources/**"  `
    /d:sonar.login="a8d1772a9d4768709d40563a9ff0d5eb0d51b0b0"

dotnet build 

dotnet test --no-build `
    /p:CollectCoverage=true `
    /p:CoverletOutputFormat="opencover"  `
    /p:CoverletOutput="./../TestResults/" `
    /p:Exclude="[xunit.runner.*]*" `
    --logger="trx" --results-directory="./TestResults/"

dotnet sonarscanner end /d:sonar.login="a8d1772a9d4768709d40563a9ff0d5eb0d51b0b0"

Write-Host "Press any key to exit..."
$Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")