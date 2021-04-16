using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using Faultify.Analyze.MutationGroups;
using Faultify.Analyze.Mutation;
using Mono.Cecil.Cil;
using FieldDefinition = Mono.Cecil.FieldDefinition;
using MethodDefinition = Mono.Cecil.MethodDefinition;
using TypeDefinition = Mono.Cecil.TypeDefinition;

namespace Faultify.Analyze.AssemblyMutator
{
    /// <summary>
    ///     Represents a raw type definition and provides access to its fields and methods..
    /// </summary>
    public class TypeScope : IMemberScope, IMutationProvider
    {
        private readonly HashSet<IAnalyzer<ConstantMutation, FieldDefinition>> _constantAnalyzers;

        public TypeScope(
            TypeDefinition typeDefinition,
            HashSet<IAnalyzer<OpCodeMutation, Instruction>> opcodeAnalyzers,
            HashSet<IAnalyzer<ConstantMutation, FieldDefinition>> fieldAnalyzers,
            HashSet<IAnalyzer<VariableMutation, MethodDefinition>> variableMutationAnalyzers,
            HashSet<IAnalyzer<ArrayMutation, MethodDefinition>> arrayMutationAnalyzers
        )
        {
            _constantAnalyzers = fieldAnalyzers;

            TypeDefinition = typeDefinition;

            Fields = TypeDefinition.Fields.Select(x =>
                new FieldScope(x)
            ).ToList();

            Methods = TypeDefinition.Methods.Select(x =>
                new MethodScope(x)
            ).ToList();
        }

        /// <summary>
        ///     The fields in this type.
        ///     For example: const, static, non-static fields.
        /// </summary>
        public List<FieldScope> Fields { get; }

        /// <summary>
        ///     The methods in this type.
        /// </summary>
        public List<MethodScope> Methods { get; }

        public TypeDefinition TypeDefinition { get; }
        public string Name => TypeDefinition.Name;
        public EntityHandle Handle => MetadataTokens.EntityHandle(TypeDefinition.MetadataToken.ToInt32());
        public string AssemblyQualifiedName => TypeDefinition.FullName;

        public IEnumerable<IMutationGroup<IMutation>> AllMutations(MutationLevel mutationLevel)
        {
            foreach (var analyzer in _constantAnalyzers)
            {
                foreach (var field in TypeDefinition.Fields)
                {
                    ConstGroup mutations = (ConstGroup)analyzer.GenerateMutations(field, mutationLevel);

                    if (mutations.Any())
                    {
                        yield return mutations;
                    }
                }
            }
        }
    }
}