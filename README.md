# Faultify
Faultify is a dotnet mutation tool that performs mutations in the byte code to test the test quality of a project. 
Basicly, it imitates the bad programmer by introducing mistakes on purpose. 
A test is supposed to fail after a mutation, if that is not the case the test is probable error-prone.

## How to use

**Commandline Options**

```
  -p, --testProjectName    Required. The path pointing to the test project project file.
  -r, --reportPath         The path were the report will be saved.
  -t, --reportType         Type of report to be generated, options: 'pdf', 'html', 'json'
  --help                   Display this help screen.
  --version                Display version information.
```

**Example**

```
dotnet Faultify.Cli.dll -p TestProject.csproj -t pdf
```

Report will be writen to executable location '/FaultifyOutput'.

## Features

**Funnctional Features**
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

**Non-Funnctional Features**
- [X] Crossplatform .net core compitability. 
- [X] Nunit/Xunit/Msunit support via `dotnet test`.
- [X] Runnable from console. 

## Operation
Faultify mutates IL code with `Mono.Cecil`. For arithmetic, assigment, equivalence, logical, bitwise, branching mutations, this is relatively easy. In these cases, only the opcode needs to be changed. With array mutations there is more complexity involved since defining arrays is not done one way in IL code but there are many edge cases.

## Alternative
- Source code mutation with mutation switching to speed up compile speed (stryker). 
