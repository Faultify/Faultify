Analyzers can be used separately from Faultify. 
They are able to provide the user mutations. 
The reader can look at `AssemblyMutator` for inspiration.
This is the abstraction that Faultify uses to analyze for mutations. 

## Implement Custom Analyzer

To implement your own Analyzer there are a couple of steps:
- Implement (or use existing) `IMutation`.
- Implement instance of `IMutationAnalyzer`.

### Implement IMutation

The mutation, as shown in the previous chapter, is responsible for performing the mutation.
Implement this to your use-case. 

```csharp
public class CustomMutation : IMutation
{
    public OpCode OriginalValue { get; set; }
    public OpCode NewValue { get; set; }

    public Instruction InstructionReference { get; set; }

    public void Mutate()
    {
        InstructionReference.OpCode = NewValue;
    }

    public void Reset()
    {
        InstructionReference.OpCode = NewValue;
    }
}
```

_Faultify provides a set of `IMutation` implementations that serve some general goals._ 

### Implement Analyzer

`IMutationAnalyzer` has two generic parameters. 
- The first one indicates the mutation type that is returned if the analyzer detects this mutation kind. 
- The second one indicates in which scope the mutation can be found. 
The scope should be a type from `Mono.Cecil` such as: `MethodDefinion`, `Instruction`, `FieldDefinition`, `TypeDefinition`, `AssemblyDefinition`.

The general idea is that the analyzer inspects this scope for some possible mutations and returns the generic mutation. 

In the following example, an analyzer that searches for `add` to `sub` mutations in the `Instruction` scope is demonstrated: 

```csharp
public class CustomMutationAnalyzer : IMutationAnalyzer<CustomMutation, Instruction>
{
    public string Description => "Addition to subtraction analyzer.";

    public string Name => "Addition to Subtraction";

    public IEnumerable<CustomMutation> AnalyzeMutations(Instruction field,
        MutationLevel mutationLevel)
    {
        if (field.OpCode.Code == Code.Add)
        {
            yield return new CustomMutation() {OriginalValue = field.OpCode, NewValue = OpCodes.Sub};
        }
    }
}
```

### Use Analyzer

```csharp
var analyzer =  new CustomMutationAnalyzer();
using var assembly = ModuleDefinition.ReadModule("assembly.dll");
var mutations = assembly.Types
    .SelectMany(x => x.Methods)
    .SelectMany(x => x.Body.Instructions)
    .SelectMany(instruction => analyzer.AnalyzeMutations(instruction, MutationLevel.Detailed).ToList());

foreach (var mutation in mutations)
{
    mutation.Mutate();
    // do some stuff
    mutation.Reset();
}
```

After mutation or resetting a mutation back to original 
one has to write the `ModuleDefinition` back to the file system. 
Otherwise, the mutations are just purely performed in memory.   

### Assembly Mutator

The `AssemblyMutator` can be used to analyze all kinds of mutations in a target assembly.
It can be extended with custom analyzers. 
Tho an extension must correspond to one of the following collections in `AssemblyMutator`:  

- ArrayMutationAnalyzers (`IMutationAnalyzer<ArrayMutation, MethodDefinition>`)
- ConstantAnalyzers (`IMutationAnalyzer<ConstantMutation, FieldDefinition>`)
- VariableMutationAnalyzer (`IMutationAnalyzer<VariableMutation, MethodDefinition>`)
- OpCodeMutationAnalyzer (`IMutationAnalyzer<OpCodeMutation, Instruction>`)

If you add your analyzer to one of those collections then it will be used in the process of analyzing.
Unfortunately, if your analyzer does not fit the interfaces, it can not be used with the `AssemblyMutator`.
Then you would have to create some kind of `AssemblyMutator` for yourself.
