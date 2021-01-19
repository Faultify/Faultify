## Run Stryker

```
dotnet tool install --global dotnet-stryker --version 0.20.0
dotnet stryker -tp "['Faultify.Benchmark.NUnit\\Faultify.Benchmark.NUnit.csproj']"
```

Mutations Found: 35
Duration:		 23 seconds
Mutations a second: 0,65 per second
Mutation score is 60.98 %

Stryker is running single threaded becasue faultify does not support multithreading yet.

## Run Faultify
```
dotnet tool install --global faultify --version 0.0.3
faultify -p .\Faultify.Benchmark.NUnit\Faultify.Benchmark.NUnit.csproj -r .\  -t html
```

Mutations Found: 23
Duration:		 15
Mutations a second: 0,65 per second
Mutation Score: 65%

## Results
Faultify seems to be slightly faster while it does have a higher score. 