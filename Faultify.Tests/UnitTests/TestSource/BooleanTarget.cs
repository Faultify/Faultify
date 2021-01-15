using System;
using System.Collections.Generic;
using System.Text;

namespace Faultify.Tests.UnitTests.TestSource
{
    public class BooleanTarget
    {
        public bool BrTrue(bool condition)
        {
            while (condition)
            {
                return true;
            }

            return false;
        }

        public bool BrFalse(bool condition)
        {
            if (condition)
                return true;

            return false;
        }
    }
}
