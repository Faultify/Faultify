using System;
using System.Collections.Generic;
using System.Text;

namespace Faultify.Analyze
{
    public interface IReportable
    {
        /// <summary>
        /// Description to be used in reports
        /// </summary>
        string Description { get; }
        /// <summary>
        ///  Name to be used in reports
        /// </summary>
        string Name { get; }
    }
}
