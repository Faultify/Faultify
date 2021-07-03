using System;

namespace Faultify.Analyze
{
    [Flags]
    public enum MutationLevel
    {
        /// <summary>
        ///     All mutations that are less likely to produce a result will be excluded from the test process.
        ///     This will likely speed up the test duration but will likely give a less accurate score.
        ///     This level might be useful for large projects.
        /// </summary>
        Simple = 0,

        /// <summary>
        ///     Not the simple level nor detailed but in between.
        /// </summary>
        Medium = 1 | Simple,

        /// <summary>
        ///     All mutations that are found will be included in the test process.
        ///     This will likely increase the test process duration however it will give a more accurate score.
        /// </summary>
        Detailed = 2 | Medium,
    }
}
