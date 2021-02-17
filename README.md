<h1 align="center"><img width="500" src="docs/full-logo.png" /></h1>

[![Nuget](https://img.shields.io/nuget/v/faultify.svg?color=blue&label=faultify&style=flat-square)](https://www.nuget.org/packages/faultify/)
[![Nuget](https://img.shields.io/nuget/dt/faultify.svg?style=flat-square)](https://www.nuget.org/packages/faultify/)
![Tester](https://github.com/Faultify/Faultify/workflows/Tester/badge.svg?branch=main)
[![Join us on Discord](https://img.shields.io/discord/801802378721493044.svg?logo=discord)](https://discord.gg/8aKeQFtcnT) 
[![Donate](https://img.shields.io/badge/Donate-PayPal-green.svg)](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=Z8QK6XU749JB2)

## Byte Code Dotnet Mutation Utility
Faultify provides a quick simple way to realize dotnet mutation testing at the byte code level. 
It imitates the bad programmer by deliberately introducing errors. 
A test is assumed to fail after an introduced mutation, the test is likely to be error-prone if it instead succeeds.

*disclaimer: faultify is just released and bugs can be expected, please open a issue if you get any.*

### Getting Started

- Read the [Technical Book](https://faultify.github.io/Faultify/index.html)
- Benchmark [Statistics](https://github.com/Faultify/Faultify/blob/main/Benchmark/README.md)

**Commandline Options**

```
  -t, --testProjectName    Required. The path pointing to the test project project file.
  -r, --reportPath         The path were the report will be saved.
  -f, --reportFormat       (Default: json) Type of report to be generated, options: 'pdf', 'html', 'json'
  -p, --parallel           (Default: 1) Defines how many test sessions are ran at the same time.
  -l, --mutationLevel      (Default: Detailed) The mutation level indicating the test depth.
  
  --help                   Display this help screen.
  --version                Display version information.
```

**Install / Run**

```
dotnet tool install --global faultify --version 0.2.0
faultify -t YourTestProject.csproj -f html
```

This generates a 'HTML' report for the project 'YourTestProject.csproj' at the default executable location in the folder '/FaultifyOutput'.

### Features

**Functional Features**
- [X] Mutate Arithmetic (+, -, /, *, %) Operators.
- [X] Mutate Assignment Expressions (+=, -=, /=, *=, %=, --, ++).
- [X] Mutate Equivalence Operators (==, !=).
- [X] Mutate Logical Operators (&&, ||).
- [X] Change bitwise operators (^,|, &).
- [X] Mutate Branching statements (if(condition), if(!condition))
- [X] Mutate Variable Literals (true, false).
     - [X] Support for other types.
- [X] Mutate Constant Fields (string, number, boolean).
- [X] Mutate Array initializations (Only arrays larger than 2 elements, all C# types)
- [X] Build mutation report (HTML/PDF).
- [x] Mutation test algorithm.

**Non-Functional Features**
- [X] Cross-platform .net core compatibility. 
- [X] Nunit/Xunit/Msunit support via `dotnet test`.
- [X] Runnable from console. 
- [X] All dotnet languages (F#, C#, visualbaisc) support.

**Todo**
- Implement [member exclusion](https://github.com/Faultify/Faultify/issues/11).
- Implement line number display
- Improve reporting functionality
- Implement assembly testing in memory

## Application Preview
<img src="https://github.com/Faultify/Faultify/blob/main/docs/application-overview.PNG" alt="drawing" width="600"/>

### Solution Projects

| Solution Item | Description |
| :----- | :----- |
| `Faultify.Cli` | `Contains the faultify executable mutation dotnet utility.`|
| `Faultify.Analyze` | `Contains the code that analyzes and searches for mutations`|
| `Faultify.TestRunner` | `Contains the code that runs the mutation test process`|
| `Faultify.Report` | `Contains the code that generates a report`|
| `/Bechmarks` | `Contains a benchmark project and tests that can be used for mutation testing with faultify/stryker.`|
