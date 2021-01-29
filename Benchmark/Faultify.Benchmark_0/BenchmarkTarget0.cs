using System;
using System.Threading;

namespace Faultify.Benchmark_0
{
    public class BenchmarkTarget0
    {
        // private const bool Constant = 1 < 2;
        // public static bool TestStaticField = 1 < 2;
        // public bool TestLocalField = 1 < 2;

        public int[] ConstructArray()
        {
            return new[] { 1, 2, 3, 6, 4, 2, 5, 3, 2, 6, 7 };
        }

        public int Addition(int lhs, int rhs)
        {
            return lhs + rhs;
        }

        public int Subtraction(int lhs, int rhs)
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
            return lhs / rhs;
        }

        public bool If(bool condition)
        {
            if (condition)
            {
                return true;
            }

            return false;
        }
    }
}
