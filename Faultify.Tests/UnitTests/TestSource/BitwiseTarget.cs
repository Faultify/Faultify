using System;
using System.Collections.Generic;
using System.Text;

namespace Faultify.Tests.UnitTests.TestSource
{
    public class BitwiseTarget
    {
        public uint AndOperator(uint lhs, uint rhs)
        {
            return lhs & rhs;
        }

        public uint OrOperator(uint lhs, uint rhs)
        {
            return lhs | rhs;
        }

        public uint XorOperator(uint lhs, uint rhs)
        {
            return lhs ^ rhs;
        }
    }
}
