using System;
using System.Collections.Generic;
using System.Text;

namespace Faultify.Tests.UnitTests.TestSource
{
    public class DecompilerTestTarget
    {
        private const bool _testBool = true;

        public int TestReturnInt(int test)
        {
            return test;
        }

        public int TestReturnInt(int test, int test2)
        {
            return test + test2;
        }
    }
}
