## Run Stryker

```
dotnet tool install --global dotnet-stryker --version 0.20.0
dotnet stryker -tp "['Faultify.Benchmark.NUnit\\Faultify.Benchmark.NUnit.csproj']"
```

Mutations Found: 35
Duration:		 23 seconds
Mutations a second: 0,92 per second
Mutation score is 60.98 %

Stryker is running single threaded becasue faultify is not able to have multiple threads yet.

## Run Faultify
```
dotnet ..\Faultify.Cli\bin\Debug\netcoreapp3.1\Faultify.Cli.dll -p .\Faultify.Benchmark.NUnit\Faultify.Benchmark.NUnit.csproj -r .\  -t html
```

Mutations Found: 29
Duration:		 17.91
Mutations a second: 0,61 per second
Mutation Score: 72%

## Results
Faultify seems to be slightly faster while it does have a higher score. 