using System;
using System.Collections.Generic;
using System.Linq;
using Faultify.Analyze.Mutation;
using Faultify.Analyze.MutationGroups;
using Faultify.Core.Extensions;
using Mono.Cecil;
using Mono.Cecil.Cil;
using NLog;

namespace Faultify.Analyze.Analyzers
{
    /// <summary>
    ///     Analyzer that searches for possible variable mutations.
    ///     Mutations such as 'true' to 'false'
    /// </summary>
    public class VariableAnalyzer : IAnalyzer<VariableMutation, MethodDefinition>
    {
        private readonly RandomValueGenerator _valueGenerator;
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public VariableAnalyzer()
        {
            _valueGenerator = new RandomValueGenerator();
        }

        public string Description =>
            "Analyzer that searches for possible variable mutations such as 'true' to 'false'.";

        public string Name => "Variable Mutation Analyzer";

        public IMutationGroup<VariableMutation> GenerateMutations(MethodDefinition method, MutationLevel mutationLevel, IDictionary<Instruction, SequencePoint> debug = null)
        {
            List<VariableMutation> mutations = new List<VariableMutation>();

            // NOTE: Commenting this out, may cause errors
            // if (method?.Body == null)
            //     mutations = Enumerable.Empty<VariableMutation>();
            int lineNumber = -1;

            foreach (var instruction in method.Body.Instructions)
            {
                // Booleans (0,1) or number literals are loaded on the evaluation stack with 'ldc_...' and popped of with 'stloc'.
                // Therefore if there is an 'ldc' instruction followed by 'stdloc' we can assert there is a literal of some type. 
                // 'ldc' does not specify the variable type. 
                // In order to know the type cast the 'Operand' to 'VariableDefinition'.

                if (instruction.OpCode != OpCodes.Stloc) continue;

                try
                {
                    if (debug != null)
                    {
                        debug.TryGetValue(instruction, out var tempSeqPoint);

                        if (tempSeqPoint != null)
                        {
                            lineNumber = tempSeqPoint.StartLine;
                        }
                    }

                    // Get variable type. Might throw InvalidCastException
                    Type type = ((TypeReference)instruction.Operand).ToSystemType();

                    // Get previous instruction.
                    Instruction variableInstruction = instruction.Previous;

                    // If the previous instruction is 'ldc' its loading a boolean or integer on the stack. 
                    if (!variableInstruction.IsLdc()) continue;

                    // If the value is mapped then mutate it.
                    if (TypeChecker.IsVariableType(type))
                    {
                        mutations.Add(
                            new VariableMutation
                            {
                                Original = variableInstruction.Operand,
                                Replacement = _valueGenerator.GenerateValueForField(type, instruction.Previous.Operand),
                                Variable = variableInstruction,
                                LineNumber = lineNumber
                            }) ;
                    }
                }
                catch (InvalidCastException e)
                {
                    _logger.Warn(e, $"Failed to get the type of {instruction.Operand}");
                }
            }

            return new MutationGroup<VariableMutation>
            {
                Name = Name,
                Description = Description,
                Mutations = mutations
            };
        }
    }
}