#Health check endpoint
```
https://localhost:5225/health
```
# API Docs 
```
https://localhost:5225/docs/index.html
```

# vs extension 
Open the Extension menu, then search and install the following extensions:
```
formulahendry.dotnet-test-explorer
ryanluker.vscode-coverage-gutters
```
## XUnit Test Coverage 
```
https://www.hanselman.com/blog/automatic-unit-testing-in-net-core-plus-code-coverage-in-visual-studio-code
```

``` 

cd src
dotnet test /p:CollectCoverage=true

dotnet test src/mysb-forms.Tests/mysb-forms.Tests.csproj /p:CollectCoverage=true /p:CoverletOutputFormat=lcov /p:CoverletOutput=./lcov.info
```