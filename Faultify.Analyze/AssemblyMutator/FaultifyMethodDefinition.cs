﻿using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using Faultify.Analyze.Groupings;
using Faultify.Analyze.Mutation;
using Faultify.Analyze.OpcodeAnalyzer;
using Mono.Cecil.Cil;
using MethodDefinition = Mono.Cecil.MethodDefinition;

namespace Faultify.Analyze.AssemblyMutator
{
    /// <summary>
    ///     Represents a raw method definition.
    /// </summary>
    public class FaultifyMethodDefinition : IMutationProvider, IFaultifyMemberDefinition
    {
        private readonly HashSet<IMutationAnalyzer<ArrayMutation, MethodDefinition>> _arrayMutationAnalyzers;

        /// <summary>
        ///     Underlying Mono.Cecil TypeDefinition.
        /// </summary>
        private readonly MethodDefinition _methodDefinition;

        private readonly HashSet<IMutationAnalyzer<OpCodeMutation, Instruction>> _opcodeMethodAnalyzers;

        private readonly HashSet<IMutationAnalyzer<VariableMutation, MethodDefinition>> _variableMutationAnalyzers;

        public FaultifyMethodDefinition(MethodDefinition methodDefinition,
            HashSet<IMutationAnalyzer<OpCodeMutation, Instruction>> opcodeMethodAnalyzers,
            HashSet<IMutationAnalyzer<VariableMutation, MethodDefinition>> variableMutationAnalyzers,
            HashSet<IMutationAnalyzer<ArrayMutation, MethodDefinition>> arrayMutationAnalyzers)
        {
            _methodDefinition = methodDefinition;
            _opcodeMethodAnalyzers = opcodeMethodAnalyzers;
            _variableMutationAnalyzers = variableMutationAnalyzers;
            _arrayMutationAnalyzers = arrayMutationAnalyzers;
        }

        public int IntHandle => _methodDefinition.MetadataToken.ToInt32();

        /// <summary>
        ///     Full assembly name of this method.
        /// </summary>
        public string AssemblyQualifiedName => _methodDefinition.FullName;

        public string Name => _methodDefinition.Name;

        public EntityHandle Handle => MetadataTokens.EntityHandle(IntHandle);

        public IEnumerable<IMutationGrouping<IMutation>> AllMutations(MutationLevel mutationLevel)
        {
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
                if (_methodDefinition.Body?.Instructions != null)
                    foreach (var instruction in _methodDefinition.Body?.Instructions)
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

        public IEnumerable<VariableMutationGrouping> VariableMutations(MutationLevel mutationLevel)
        {
            return _variableMutationAnalyzers.Select(analyzer => new VariableMutationGrouping
            {
                Mutations = analyzer.AnalyzeMutations(_methodDefinition, mutationLevel),
                Key = _methodDefinition.Name,
                AnalyzerName = analyzer.Name,
                AnalyzerDescription = analyzer.Description
            });
        }

        public IEnumerable<ArrayMutationGrouping> ArrayMutations(MutationLevel mutationLevel)
        {
            return _arrayMutationAnalyzers.Select(analyzer => new ArrayMutationGrouping
            {
                Mutations = analyzer.AnalyzeMutations(_methodDefinition, mutationLevel),
                Key = _methodDefinition.Name,
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