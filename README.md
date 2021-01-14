# Faultify
Faultify is a dotnet mutation tool that performs mutations in the byte code to test the test quality of a project. 

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
- [X] Runnable from console. 

## How to use

```
dotnet faultify --testProjectPath --reportPath ...
```

## Operation
Faultify mutates IL code with `Mono.Cecil`. For arithmetic, assigment, equivalence, logical, bitwise, branching mutations, this is relatively easy. In these cases, only the opcode needs to be changed. With array mutations there is more complexity involved since defining arrays is not done one way in IL code but there are many edge cases.

## Alternative
- Source code mutation with mutation switching to speed up compile speed (stryker). 
