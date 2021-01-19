using System.Collections.Generic;
using System.Linq;
using Faultify.Analyze.Mutation;

namespace Faultify.Analyze.Groupings
{
    /// <summary>
    ///     Groups the subject that is mutated and the possible mutations together.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IMutationGrouping<out T> : IGrouping<string, T> where T : IMutation
    {
        /// <summary>
        ///     Description of the analyzer that analyzed for the mutation.
        /// </summary>
        string AnalyzerDescription { get; set; }

        /// <summary>
        ///     Name of the analyzer that analyzed for the mutation.
        /// </summary>
        string AnalyzerName { get; set; }

        public IEnumerable<T> Mutations { get; }
    }
}