using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using Faultify.Analyze.Groupings;
using Faultify.Analyze.Mutation;
using Faultify.Analyze.OpcodeAnalyzer;
using Microsoft.Build.Framework;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using FieldDefinition = Mono.Cecil.FieldDefinition;
using MethodDefinition = Mono.Cecil.MethodDefinition;

namespace Faultify.Analyze.AssemblyMutator
{
    /// <summary>
    ///     Represents a raw method definition.
    /// </summary>
    public class FaultifyMethodDefinition : IMutationProvider, IFaultifyMemberDefinition
    {
        

        /// <summary>
        ///     Underlying Mono.Cecil TypeDefinition.
        /// </summary>
        public readonly MethodDefinition MethodDefinition;

        private readonly HashSet<IMutationAnalyzer<ArrayMutation, MethodDefinition>> _arrayMutationAnalyzers;

        private readonly HashSet<IMutationAnalyzer<ConstantMutation, FieldDefinition>> _constantReferenceMutationAnalyers;

        private readonly HashSet<IMutationAnalyzer<OpCodeMutation, Instruction>> _opcodeMethodAnalyzers;

        private readonly HashSet<IMutationAnalyzer<VariableMutation, MethodDefinition>> _variableMutationAnalyzers;

        public FaultifyMethodDefinition(MethodDefinition methodDefinition,
            HashSet<IMutationAnalyzer<ConstantMutation, FieldDefinition>> constantReferenceMutationAnalyers,
            HashSet<IMutationAnalyzer<OpCodeMutation, Instruction>> opcodeMethodAnalyzers,
            HashSet<IMutationAnalyzer<VariableMutation, MethodDefinition>> variableMutationAnalyzers,
            HashSet<IMutationAnalyzer<ArrayMutation, MethodDefinition>> arrayMutationAnalyzers)
        {
            MethodDefinition = methodDefinition;
            _constantReferenceMutationAnalyers = constantReferenceMutationAnalyers;
            _opcodeMethodAnalyzers = opcodeMethodAnalyzers;
            _variableMutationAnalyzers = variableMutationAnalyzers;
            _arrayMutationAnalyzers = arrayMutationAnalyzers;
        }

        public int IntHandle => MethodDefinition.MetadataToken.ToInt32();

        /// <summary>
        ///     Full assembly name of this method.
        /// </summary>
        public string AssemblyQualifiedName => MethodDefinition.FullName;

        public string Name => MethodDefinition.Name;

        public EntityHandle Handle => MetadataTokens.EntityHandle(IntHandle);

        public IEnumerable<IMutationGrouping<IMutation>> AllMutations(MutationLevel mutationLevel)
        {
            MethodDefinition.Body.SimplifyMacros();

            IEnumerable<IMutationGrouping<IMutation>> opcodeMutations = OpCodeMutations(mutationLevel);
            IEnumerable<IMutationGrouping<IMutation>> variableMutations = VariableMutations(mutationLevel);
            IEnumerable<IMutationGrouping<IMutation>> arrayMutations = ArrayMutations(mutationLevel);

            return opcodeMutations.Concat(variableMutations).Concat(arrayMutations);
        }

        /// <summary>
        ///     Returns all possible mutations from the method its instructions.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<OpCodeGrouping> OpCodeMutations(MutationLevel mutationLevel)
        {
            foreach (var analyzer in _opcodeMethodAnalyzers)
                if (MethodDefinition.Body?.Instructions != null)
                    foreach (var instruction in MethodDefinition.Body?.Instructions)
                    {
                        var mutations = analyzer.AnalyzeMutations(instruction, mutationLevel).ToList();

                        if (mutations.Any())
                            yield return new OpCodeGrouping
                            {
                                Key = instruction.OpCode.ToString(),
                                Mutations = mutations,
                                AnalyzerName = analyzer.Name,
                                AnalyzerDescription = analyzer.Description
                            };
                    }
        }

        public IEnumerable<ConstGrouping> ConstantReferenceMutations(MutationLevel mutationLevel)
        {
            if (MethodDefinition.Name == "LessThan")
            {
                var fieldReferences = MethodDefinition.Body.Instructions
                    .OfType<FieldReference>();

                foreach (var field in fieldReferences)
                {
                    foreach (var analyzer in _constantReferenceMutationAnalyers)
                    {
                        var mutations = analyzer.AnalyzeMutations(field.Resolve(), mutationLevel);

                        yield return new ConstGrouping()
                        {
                            AnalyzerName = analyzer.Name,
                            AnalyzerDescription = analyzer.Description,
                            Key = field.Name,
                            Mutations = mutations
                        };
                    }
                }
            }
        }

        public IEnumerable<VariableMutationGrouping> VariableMutations(MutationLevel mutationLevel)
        {
            return _variableMutationAnalyzers.Select(analyzer => new VariableMutationGrouping
            {
                Mutations = analyzer.AnalyzeMutations(MethodDefinition, mutationLevel),
                Key = MethodDefinition.Name,
                AnalyzerName = analyzer.Name,
                AnalyzerDescription = analyzer.Description
            });
        }

        public IEnumerable<ArrayMutationGrouping> ArrayMutations(MutationLevel mutationLevel)
        {
            return _arrayMutationAnalyzers.Select(analyzer => new ArrayMutationGrouping
            {
                Mutations = analyzer.AnalyzeMutations(MethodDefinition, mutationLevel),
                Key = MethodDefinition.Name,
                AnalyzerName = analyzer.Name,
                AnalyzerDescription = analyzer.Description
            });
        }

        public class VariableMutationGrouping : BaseGrouping<VariableMutation>
        {
        }

        public class ArrayMutationGrouping : BaseGrouping<ArrayMutation>
        {
        }
    }
}