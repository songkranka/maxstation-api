call dotnet tool install --global dotnet-sonarscanner --version 4.7.1
rem call dotnet add package coverlet.msbuild
call dotnet sonarscanner begin /k:"test" /d:sonar.host.url="http://localhost:9000"  /d:sonar.login="d9050888068e1a491e9c526d078e059c869d19ff"
call dotnet build
call dotnet sonarscanner end /d:sonar.login="d9050888068e1a491e9c526d078e059c869d19ff"
