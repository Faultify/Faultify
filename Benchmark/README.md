This document contains a benchmark, and a comperison with stryker. 

**Mutation per Second**: (mutations / seconds)
**Benchmark Application**: Faultify.Benchmark.Runner**
**Benchmark System**: i7-7700K CPU @ 4.20GHz, 16GB DDR-4

## Run Stryker

```
dotnet tool install --global dotnet-stryker --version 0.21.0
dotnet stryker -tp "['Faultify.Benchmark.NUnit\\Faultify.Benchmark.NUnit.csproj']"
```

**Benchmark from Faultify.Benchmark.Runner**

Mutations *Tested*: 124
Score:     64%

| Runners | Duration | Mutation per Second | 
|---------|----------|----------------------|
| 1       |  34653   |       3,65           | 
| 2       |  30263   |       4,13           |
| 3       |  31334   |       4,00           |
| 4       |  31268   |       4,00           |
| 5       |  35250   |       3,54           |
| 6       |  43799   |       2,88           |

## Run Faultify

```
dotnet tool install --global faultify --version 0.1.0
faultify -t .\Faultify.Benchmark.NUnit\Faultify.Benchmark.NUnit.csproj  -f html -p 4

// Run (main) local executable
..\Faultify.Cli\bin\Debug\netcoreapp3.1\Faultify.Cli.exe -t .\Faultify.Benchmark.NUnit\Faultify.Benchmark.NUnit.csproj  -f html
```

**Benchmark from Faultify.Benchmark.Runner**
Mutations *Tested*: 168
Score:     ~60%

| Runners | Duration | Mutation per Second | 
|---------|----------|----------------------|
| 1       |  48616   |       3,50           | 
| 2       |  52657   |       3,23           |
| 3       |  49840   |       3,43           |
| 4       |  34949   |       4,94           |
| 5       |  35427   |       4,80           |
| 6       |  40383   |       4,20           |


## Results

- Both stryker and faultify generate about he same mutation score.
- The mutations per seconds do not differ that much, 
stryker is slightly faster with 2-3 test runners wereas faultify is faster with 1 or > 3 test runners.
- Stryker becomes slower, as they acknowlege as well, after 3 testrunners. 
- Faultify becomes faster with every test runner added. Only when more test runners are specified than test runs, speed is not influenced

Faultify becomes significantly faster when more then 2 test runners are configured. 
- On a larger project with `259 mutations,` it took Faultify 55 seconds, which is `0,21` seconds for a mutation. Whereas for stryker it ook about `150` seconds which is `0,58` seconds permutation. This is a speed increasement of about `58%`.
