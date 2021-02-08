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
| 1       |  19443   |       1,63           | 
| 2       |  17650   |       1,82           |
| 3       |  17703   |       1,82           |
| 4       |  21046   |       1,48           |
| 5       |  25285   |       1,24           |
| 6       |  25566   |       1,24           |


## Run Faultify

```
dotnet tool install --global faultify --version 0.1.0
faultify -t .\Faultify.Benchmark.NUnit\Faultify.Benchmark.NUnit.csproj  -f html -p 4

// Run at local executable
..\Faultify.Cli\bin\Debug\netcoreapp3.1\Faultify.Cli.exe -t .\Faultify.Benchmark.NUnit\Faultify.Benchmark.NUnit.csproj  -f html
```

**Benchmark from Faultify.Benchmark.Runner**
Mutations: 29
Score:     62%

| Runners | Duration | Seconds per mutation | 
|---------|----------|----------------------|
| 1       |  22529   |       1,32           | 
| 2       |  17510   |       1,71           |
| 3       |  19905   |       1,53           |
| 4       |  16187   |       1,81           |
| 5       |  15547   |       1,93           |
| 6       |  14885   |       2,07           |

## Results
- Both stryker and faultify have about the same amount of mutations and the same score. 
- With this particular project, stryker is faster when 1–2 test runners are configured. 
Faultify becomes significantly faster when more then 2 test runners are configured. 
- On a larger project with `259 mutations,` it took Faultify 55 seconds, which is `0,21` seconds for a mutation. Whereas for stryker it ook about `150` seconds which is `0,58` seconds permutation. This is a speed increasement of about `58%`.
