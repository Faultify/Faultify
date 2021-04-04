using System;
using System.Collections.Generic;
using System.Linq;
using Faultify.Analyze.Mutation;
using Faultify.Analyze.MutationGroups;
using Faultify.Core.Extensions;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Faultify.Analyze
{
    /// <summary>
    ///     Analyzer that searches for possible variable mutations.
    ///     Mutations such as 'true' to 'false'
    /// </summary>
    public class VariableMutationAnalyzer : IMutationAnalyzer<VariableMutation, MethodDefinition>
    {
        private readonly RandomValueGenerator _valueGenerator;

        public VariableMutationAnalyzer()
        {
            _valueGenerator = new RandomValueGenerator();
            Mapped = new TypeCollection();
            Mapped.AddBooleanTypes();
        }

        public TypeCollection Mapped { get; }

        public string Description =>
            "Analyzer that searches for possible variable mutations such as 'true' to 'false'.";

        public string Name => "Variable Mutation Analyzer";

        public IMutationGroup<VariableMutation> GenerateMutations(MethodDefinition method, MutationLevel mutationLevel)
        {
            List<VariableMutation> mutations = new List<VariableMutation>();

            // NOTE: Commenting this out, may cause errors
            // if (method?.Body == null)
            //     mutations = Enumerable.Empty<VariableMutation>();

            foreach (var instruction in method.Body.Instructions)
            {
                // Booleans (0,1) or number literals are loaded on the evaluation stack with 'ldc_...' and popped of with 'stloc'.
                // Therefore if there is an 'ldc' instruction followed by 'stdloc' we can assert there is a literal of some type. 
                // 'ldc' does not specify the variable type. 
                // In order to know the type cast the 'Operand' to 'VariableDefinition'.

                if (instruction.OpCode != OpCodes.Stloc) continue;

                var variableDefinition = instruction.Operand as VariableDefinition;

                if (variableDefinition == null) continue;

                try
                {
                    // Get variable type.
                    var variableType = Type.GetType(variableDefinition.VariableType.ToString());

                    // Get previous instruction.
                    var variableInstruction = instruction.Previous;

                    // If the previous instruction is 'ldc' its loading a boolean or integer on the stack. 
                    if (!variableInstruction.IsLdc()) continue;

                    // If the value is mapped then mutate it.
                    if (Mapped.Types.TryGetValue(variableType, out var type))
                        mutations.Add(new VariableMutation
                        {
                            Original = variableInstruction.Operand,
                            Replacement = _valueGenerator.GenerateValueForField(type, instruction.Previous.Operand),
                            Variable = variableInstruction
                        });
                }
                catch
                {
                    Console.Error.WriteLine($"GetType() failed to get the type of {variableDefinition.VariableType}"); // TODO: Use proper logging
                }
            }

            return new MutationGroup<VariableMutation> {
                Name = Name,
                Description = Description,
                Mutations = mutations
            };
        }
    }
}