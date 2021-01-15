using System;

namespace Faultify.Tests.UnitTests.TestSource
{
    public class LogicalTarget
    {
        public bool AndOperator(bool t, bool f)
        {
            if (t && f)
            {
                return true;
            }

            return false;
        }

        public bool OrOperator(bool t, bool f)
        {
            if (t || f)
            {
                return true;
            }

            return false;
        }

    }
}
