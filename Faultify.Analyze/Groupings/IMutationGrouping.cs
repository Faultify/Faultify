using System.Collections.Generic;
using System.Linq;
using Faultify.Analyze.Mutation;

namespace Faultify.Analyze.Groupings
{
    /// <summary>
    ///     Groups the subject that is mutated and the possible mutations together.
    ///     This interface is required for the covariance <code>out</code> parameter
    ///     which cannot be added directly to a class.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IMutationGrouping<out T> : IReportable, IEnumerable<T> where T : IMutation
    {
        public IEnumerable<T> Mutations { get; }
    }
}