namespace Faultify.Analyze
{
    public interface IReportable
    {
        /// <summary>
        ///     Description to be used in reports
        /// </summary>
        string Description { get; }

        /// <summary>
        ///     Name to be used in reports
        /// </summary>
        string Name { get; }
    }
}
