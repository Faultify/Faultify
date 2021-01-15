using System;
using System.Collections.Generic;
using System.Text;

namespace Faultify.Tests.UnitTests.TestSource
{
    public class TestAssemblyTarget1
    {
        private const bool Constant = false;
        private static bool Static = false;

        public int TestMethod1(int a)
        {
            var b = a < 10;
            return a + 1;
        }

        public void TestMethod2()
        {
        }
    }

    public class TestAssemblyTarget2
    {
        private const string Constant = "";
        private static string Static = "";

        public void TestMethod1()
        {
        }

        public void TestMethod2()
        {
        }
    }
}
