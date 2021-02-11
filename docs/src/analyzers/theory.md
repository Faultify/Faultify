# Analyzers

An `analyzer` looks at the bytecode, checks what kind of mutations can take place, 
and returns the possible mutations (see next section) to the caller.

![Analyzers](../img/analyzers.PNG)
 
An analyzer has two generics: `TMutation` and `TInput`. 
`TMutation` is the mutation type which contains metadata about the possible mutation and can execute and/or undo itself. 
`TInput` is the scope where the mutation check will be made. 

The scope is a type of `Mono.Cecil` and can for example be a `Method/FieldDefinition` or `Instruction`.

- In the case of opcode mutations, the scope is an 'Instruction' (e.g. return 1 + 1) , 
- in the case of an array mutation, the scope is a 'Method Definition',
- in the case of a constant field mutation, the scope is a 'FieldDefinition'.

**Mutation Level**: This level determines what mutations are executed during a test run.

## OpCode Mutation Analyzer
This mutation analyzer looks at what possible mutations there are in the opcode of an instruction. 
An instruction is a statement like 'a = 1 + 2'. 
On this instruction the '+' can be changed to '-, /, *, %'. 
There are 4 different opcode mutation analyzers. 
Namely an analyzer for arithmetic operations (/, *), bitwise operations (^, |), branch (if) operations and comparison operations (<, >). 
For many opcodes, one or more mutations are possible, therefore an 'IEnumerable<TMutation>' is returned.

## Constant Mutation Analyzer
This mutation analyzer looks at what possible mutations can be performed on a constant field. 
There are different types of constant fields with different ways of mutation. 
An integer can be mutated with a random number, a string with a random string and a boolean should be inverted. 

## Array Mutation Analyzer
This mutation analyzer looks at the possible mutations on arrays in a method body. 
There are different arrays, different ways to declare and modify them. 
An array can be:

- Modified
- Emptied
- Resized

There are:
- Arrays passed as parameters
- Inline initialized arrays (`new {1, 2, 3 }`)
- Index initialized arrays 
- Index changes

## Variable Analyzer
The variable analyzer is able to mutate literal variables such as `false` to `true`, `1` to `2` etc.. 

