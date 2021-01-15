using System;
using System.Collections.Generic;
using System.Text;

namespace Faultify.Tests.UnitTests.TestSource
{
    public class ArithmeticTarget
    {
        public int Addition(int lhs, int rhs)
        {
            return lhs + rhs;
        }

        public int Substraction(int lhs, int rhs)
        {
            return lhs - rhs;
        }

        public int Multiplication(int lhs, int rhs)
        {
            return lhs * rhs;
        }

        public int Division(int lhs, int rhs)
        {
            return lhs / rhs;
        }

        public int Modulo(int lhs, int rhs)
        {
            return lhs % rhs;
        }
    }
}
