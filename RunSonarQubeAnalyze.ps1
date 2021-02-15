cd C:\Users\Jarek\Documents\Projects\Magisterka\ProjectQualityChecker
dotnet sonarscanner begin  /k:"pqc" `
    /d:sonar.cs.opencover.reportsPaths="./TestResults/coverage.opencover.xml" `
    /d:sonar.cs.vstest.reportsPaths="./TestResults/*.trx" `
    /d:sonar.host.url="http://localhost:8000" `
    /d:sonar.coverage.exclusions="**/Test*/**" `
    /d:sonar.exclusions="**/wwwroot/**, **/obj/**, **/bin/**, **/Migrations/**,**/Resources/**"  `
    /d:sonar.login="9dc7d10cd01534a4b2551ffacb8db57e1fd8ae48"

dotnet build 

dotnet test --no-build `
    /p:CollectCoverage=true `
    /p:CoverletOutputFormat="opencover"  `
    /p:CoverletOutput="./../TestResults/" `
    /p:Exclude="[xunit.runner.*]*" `
    --logger="trx" --results-directory="./TestResults/"

dotnet sonarscanner end /d:sonar.login="9dc7d10cd01534a4b2551ffacb8db57e1fd8ae48"