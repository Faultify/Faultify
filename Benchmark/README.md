This document contains a benchmark, and a comperison with stryker. 

**Seconds per mutation**: Time in (milliseconds / mutations) / 1000
**Benchmark Application**: Faultify.Benchmark.Runner**

## Run Stryker

```
dotnet tool install --global dotnet-stryker --version 0.20.0
dotnet stryker -tp "['Faultify.Benchmark.NUnit\\Faultify.Benchmark.NUnit.csproj']"
```

**Benchmark from Faultify.Benchmark.Runner**

Mutations: 31
Score:     64.86%

| Runners | Duration | Seconds per mutation | 
|---------|----------|----------------------|
| 1       |  19526   |       0,62           | 
| 2       |  16572   |       0,53           |
| 3       |  16131   |       0,52           |
| 4       |  16067   |       0,51           |
| 5       |  20505   |       0,66           |
| 6       |  25881   |       0,83           |


## Run Faultify


```
dotnet tool install --global faultify --version 0.1.0
faultify -t .\Faultify.Benchmark.NUnit\Faultify.Benchmark.NUnit.csproj  -f html -p 4

// Run at local executable
..\Faultify.Cli\bin\Debug\netcoreapp3.1\Faultify.Cli.exe -t .\Faultify.Benchmark.NUnit\Faultify.Benchmark.NUnit.csproj  -f html
```

**Benchmark from Faultify.Benchmark.Runner**
Mutations: 34
Score:     62%

| Runners | Duration | Seconds per mutation | 
|---------|----------|----------------------|
| 1       |  33028   |       0,97           | 
| 2       |  20533   |       0,60           |
| 3       |  13373   |       0,39           |
| 4       |  13473   |       0,40           |
| 5       |  12685   |       0,37           |
| 6       |  13059   |       0,38           |

## Results
- Both stryker and faultify have about the same amount of mutations and the same score. 
- With this particular project, stryker is faster when 1–2 test runners are configured. 
Faultify becomes significantly faster when more then 2 test runners are configured. 
- On a larger project with `259 mutations,` it took Faultify 55 seconds, which is `0,21` seconds for a mutation. Whereas for stryker it ook about `150` seconds which is `0,58` seconds permutation. This is a speed increasement of about `58%`.
