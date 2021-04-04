using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using Faultify.Analyze.MutationGroups;
using Faultify.Analyze.Mutation;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using FieldDefinition = Mono.Cecil.FieldDefinition;
using MethodDefinition = Mono.Cecil.MethodDefinition;

namespace Faultify.Analyze.AssemblyMutator
{
    /// <summary>
    ///     Contains all of the instructions and mutations within the scope of a method definition.
    /// </summary>
    public class FaultifyMethodDefinition : IMutationProvider, IFaultifyMemberDefinition
    {
        private readonly HashSet<IMutationAnalyzer<ArrayMutation, MethodDefinition>> _arrayMutationAnalyzers;
        private readonly HashSet<IMutationAnalyzer<ConstantMutation, FieldDefinition>> _constantReferenceMutationAnalyers;
        private readonly HashSet<IMutationAnalyzer<OpCodeMutation, Instruction>> _opCodeMethodAnalyzers;
        private readonly HashSet<IMutationAnalyzer<VariableMutation, MethodDefinition>> _variableMutationAnalyzers;


        /// <summary>
        ///     Underlying Mono.Cecil TypeDefinition.
        /// </summary>
        public readonly MethodDefinition MethodDefinition;

        public FaultifyMethodDefinition(
            MethodDefinition methodDefinition,
            HashSet<IMutationAnalyzer<ConstantMutation, FieldDefinition>> constantReferenceMutationAnalyers,
            HashSet<IMutationAnalyzer<OpCodeMutation, Instruction>> opcodeMethodAnalyzers,
            HashSet<IMutationAnalyzer<VariableMutation, MethodDefinition>> variableMutationAnalyzers,
            HashSet<IMutationAnalyzer<ArrayMutation, MethodDefinition>> arrayMutationAnalyzers)
        {
            MethodDefinition = methodDefinition;
            _constantReferenceMutationAnalyers = constantReferenceMutationAnalyers;
            _opCodeMethodAnalyzers = opcodeMethodAnalyzers;
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

        /// <summary>
        ///     Returns all available mutations within the scope of this method.
        /// </summary>
        public IEnumerable<IMutationGroup<IMutation>> AllMutations(MutationLevel mutationLevel)
        {
            if (MethodDefinition.Body == null)
                return Enumerable.Empty<IMutationGroup<IMutation>>();

            MethodDefinition.Body.SimplifyMacros();

            IEnumerable<IMutationGroup<IMutation>> opcodeMutations = OpCodeMutations(mutationLevel);
            IEnumerable<IMutationGroup<IMutation>> constantMutations = ConstantReferenceMutations(mutationLevel);
            IEnumerable<IMutationGroup<IMutation>> variableMutations = VariableMutations(mutationLevel);
            IEnumerable<IMutationGroup<IMutation>> arrayMutations = ArrayMutations(mutationLevel);

            return opcodeMutations
                // .Concat(constantMutations) // TODO: Why was this nopt used in the original?
                .Concat(variableMutations)
                .Concat(arrayMutations);
        }

        /// <summary>
        ///     Returns all operator mutations within the scope of this method.
        /// </summary>
        public IEnumerable<OpCodeGroup> OpCodeMutations(MutationLevel mutationLevel)
        {
            foreach (var analyzer in _opCodeMethodAnalyzers)
                if (MethodDefinition.Body?.Instructions != null)
                    foreach (var instruction in MethodDefinition.Body?.Instructions)
                    {
                        OpCodeGroup mutations = (OpCodeGroup) analyzer.GenerateMutations(instruction, mutationLevel);

                        if (mutations.Any())
                            yield return mutations;
                    }
        }

        /// <summary>
        ///     Returns all literal value mutations within the scope of this method.
        /// </summary>
        public IEnumerable<ConstGroup> ConstantReferenceMutations(MutationLevel mutationLevel)
        {
            var fieldReferences = MethodDefinition.Body.Instructions
                .OfType<FieldReference>();

            foreach (var field in fieldReferences)
            foreach (var analyzer in _constantReferenceMutationAnalyers)
            {
                ConstGroup mutations = (ConstGroup) analyzer.GenerateMutations(field.Resolve(), mutationLevel);

                if (mutations.Any())
                {
                    yield return mutations;
                }
            }
        }

        /// <summary>
        ///     Returns all variable mutations within the scope of this method.
        /// </summary>
        public IEnumerable<VarMutationGroup> VariableMutations(MutationLevel mutationLevel)
        {
            return
                from analyzer
                in _variableMutationAnalyzers
                select (VarMutationGroup) analyzer.GenerateMutations(MethodDefinition, mutationLevel);
        }

        /// <summary>
        ///     Returns all array mutations within the scope of this method.
        /// </summary>
        public IEnumerable<ArrayMutationGroup> ArrayMutations(MutationLevel mutationLevel)
        {
            return
                from analyzer
                in _arrayMutationAnalyzers
                select (ArrayMutationGroup) analyzer.GenerateMutations(MethodDefinition, mutationLevel);
        }
    }
}