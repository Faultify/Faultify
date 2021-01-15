using System;
using System.Collections.Generic;
using System.Text;

namespace Faultify.Tests.UnitTests.TestSource
{
    public class LoopTarget
    {
        public int ForLoop(int loops)
        {
            int j = 0;
            for (int i = 0; i < loops; i++)
            {
                j++;
            }
            return j;
        }

        public int WhileLoop(int loops)
        {
            int i = 0;
            while (i < loops)
            {
                i++;
            }
            return i;
        }
    }
}
