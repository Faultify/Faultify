using Faultify.Benchmark_0;
using Faultify.Benchmark_1;
using Xunit;

namespace Faultify.Benchmark.XUnit
{
    public class BenchmarkXUnit
    {
        [Fact]
        public void TestArray()
        {
            var targets = new BenchmarkTarget0();
            var actual = targets.ConstructArray();
            int[] expected = {1, 2, 3, 6, 4, 2, 5, 3, 2, 6, 7};

            Assert.Equal(expected, expected);
        }

        [Fact]
        public void TestAddition()
        {
            var targets = new BenchmarkTarget0();
            var actual = targets.Addition(5, 4);
            var expected = 9;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestSubtraction()
        {
            var targets = new BenchmarkTarget0();
            var actual = targets.Subtraction(5, 4);
            var expected = 1;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestDivision()
        {
            var targets = new BenchmarkTarget0();
            var actual = targets.Division(2, 1);
            var expected = 2;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestMultiplication()
        {
            var targets = new BenchmarkTarget0();
            var actual = targets.Multiplication(2, 2);
            var expected = 4;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestModulo()
        {
            var targets = new BenchmarkTarget0();
            var actual = targets.Modulo(3, 2);
            var expected = 1;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestForLoop()
        {
            var targets = new BenchmarkTarget1();
            targets.ForLoop(10);
            Assert.True(true);
        }

        [Fact]
        public void TestWhileLoop()
        {
            var targets = new BenchmarkTarget1();
            var actual = targets.WhileLoop(10);
            var expected = 10;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestIf()
        {
            var targets = new BenchmarkTarget0();
            var actual = targets.If(true);
            var expected = true;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestLessThan()
        {
            var targets = new BenchmarkTarget1();
            var actual = false;

            actual = targets.LessThan(5, 10);

            var expected = true;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestMoreThan()
        {
            var targets = new BenchmarkTarget1();
            var actual = targets.MoreThan(10, 5);
            var expected = true;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestLogicalAnd()
        {
            var targets = new BenchmarkTarget1();
            var actual = targets.LogicalAnd(0, 0);
            var expected = true;
            Assert.Equal(expected, actual);
        }


        [Fact]
        public void TestLogicalOr()
        {
            var targets = new BenchmarkTarget1();
            var actual = targets.LogicalOr(0, 1);
            var expected = true;
            Assert.Equal(expected, actual);
        }
    }
}