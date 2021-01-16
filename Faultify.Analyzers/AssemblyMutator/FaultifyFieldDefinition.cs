using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using Faultify.Analyzers.ConstantAnalyzer;
using Faultify.Analyzers.Groupings;
using Faultify.Analyzers.Mutation;
using FieldDefinition = Mono.Cecil.FieldDefinition;

namespace Faultify.Analyzers.AssemblyMutator
{
    /// <summary>
    ///     Represents a raw field definition.
    /// </summary>
    public class FaultifyFieldDefinition : IMutationProvider, IFaultifyMemberDefinition
    {
        private readonly HashSet<ConstantMutationAnalyzer> _fieldAnalyzers;

        /// <summary>
        ///     Underlying Mono.Cecil FieldDefinition.
        /// </summary>
        private readonly FieldDefinition _fieldDefinition;

        public FaultifyFieldDefinition(FieldDefinition fieldDefinition,
            HashSet<ConstantMutationAnalyzer> fieldAnalyzers)
        {
            _fieldDefinition = fieldDefinition;
            _fieldAnalyzers = fieldAnalyzers;
        }

        public string AssemblyQualifiedName => _fieldDefinition.FullName;
        public string Name => _fieldDefinition.Name;
        public EntityHandle Handle => MetadataTokens.EntityHandle(_fieldDefinition.MetadataToken.ToInt32());

        public IEnumerable<IMutationGrouping<IMutation>> AllMutations(MutationLevel mutationLevel)
        {
            return ConstantFieldMutations(mutationLevel);
        }

        /// <summary>
        ///     Returns possible constant field mutations.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ConstGrouping> ConstantFieldMutations(MutationLevel mutationLevel)
        {
            foreach (var analyzer in _fieldAnalyzers)
            {
                var mutations = analyzer.AnalyzeMutations(_fieldDefinition, mutationLevel);

                if (mutations.Any())
                    yield return new ConstGrouping
                    {
                        Mutations = mutations,
                        Key = analyzer.Name,
                        AnalyzerName = analyzer.Name,
                        AnalyzerDescription = analyzer.Description
                    };
            }
        }
    }
}