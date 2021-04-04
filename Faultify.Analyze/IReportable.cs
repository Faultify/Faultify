using System;
using System.Collections.Generic;
using System.Text;

namespace Faultify.Analyze
{
    public interface IReportable
    {
        string Description { get; }
        string Name { get; }
    }
}
