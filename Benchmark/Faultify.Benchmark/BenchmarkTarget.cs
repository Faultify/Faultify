using System;

namespace Faultify.Benchmark
{
    public class BenchmarkTarget
    {
        private const bool Constant = 1 < 2;
        // public static bool TestStaticField = 1 < 2;
        // public bool TestLocalField = 1 < 2;

        public int[] ConstructArray()
        {
            return new[] {1, 2, 3, 6, 4, 2, 5, 3, 2, 6, 7};
        }

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
            Console.WriteLine(Constant);

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
            if (condition) return true;

            return false;
        }
    }

    public class BenchmarkTarget1
    {
        // private const bool Constant = 1 < 2;
        // public static bool TestStaticField = 1 < 2;
        // public bool TestLocalField = 1 < 2;

        public int[] ConstructArray()
        {
            return new[] {1, 2, 3, 6, 4, 2, 5, 3, 2, 6, 7};
        }

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
            if (condition) return true;

            return false;
        }
    }

    public class BenchmarkTarget2
    {
        // private const bool Constant = 1 < 2;
        // public static bool TestStaticField = 1 < 2;
        // public bool TestLocalField = 1 < 2;

        public int[] ConstructArray()
        {
            return new[] {1, 2, 3, 6, 4, 2, 5, 3, 2, 6, 7};
        }

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
            if (condition) return true;

            return false;
        }
    }

    public class BenchmarkTarget3
    {
        // private const bool Constant = 1 < 2;
        // public static bool TestStaticField = 1 < 2;
        // public bool TestLocalField = 1 < 2;

        public int[] ConstructArray()
        {
            return new[] {1, 2, 3, 6, 4, 2, 5, 3, 2, 6, 7};
        }

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
            if (condition) return true;

            return false;
        }
    }
}