using System;

namespace Faultify.Benchmark_1
{
    public class BenchmarkTarget1
    {
        public int WhileLoop(int loops)
        {
            var i = 0;
            while (i < loops) i++;

            return i;
        }

        public void ForLoop(int loops)
        {
            for (var i = 0; i < loops; i++) Console.WriteLine();
        }

        public bool MoreThan(int lhs, int rhs)
        {
            if (lhs > rhs) return true;

            return false;
        }

        public bool LessThan(int lhs, int rhs)
        {
            if (lhs < rhs) return true;

            return false;
        }

        public bool LogicalAnd(int lhs, int rhs)
        {
            if (lhs == 0 && rhs == 0) return true;

            return false;
        }

        public bool LogicalOr(int lhs, int rhs)
        {
            if (lhs == 0 || rhs == 0) return true;

            return false;
        }
    }
}