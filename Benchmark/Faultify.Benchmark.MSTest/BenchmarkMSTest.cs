using Faultify.Benchmark_0;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Faultify.Benchmark.MSTest
{
    [TestClass]
    public class BenchmarkXUnit
    {
        [TestMethod]
        public void TestArray()
        {
            var targets = new BenchmarkTarget();
            var actual = targets.ConstructArray();
            int[] expected = { 1, 2, 3, 6, 4, 2, 5, 3, 2, 6, 7 };

            Assert.AreEqual(expected, expected);
        }

        [TestMethod]
        public void TestAddition()
        {
            var targets = new BenchmarkTarget();
            var actual = targets.Addition(5, 4);
            var expected = 9;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSubtraction()
        {
            var targets = new BenchmarkTarget();
            var actual = targets.Subtraction(5, 4);
            var expected = 1;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestDivision()
        {
            var targets = new BenchmarkTarget();
            var actual = targets.Division(2, 1);
            var expected = 2;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestMultiplication()
        {
            var targets = new BenchmarkTarget();
            var actual = targets.Multiplication(2, 2);
            var expected = 4;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestModulo()
        {
            var targets = new BenchmarkTarget();
            var actual = targets.Modulo(3, 2);
            var expected = 1;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestForLoop()
        {
            var targets = new BenchmarkTarget();
            targets.ForLoop(10);
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TestWhileLoop()
        {
            var targets = new BenchmarkTarget();
            var actual = targets.WhileLoop(10);
            var expected = 10;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestIf()
        {
            var targets = new BenchmarkTarget();
            var actual = targets.If(true);
            var expected = true;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestLessThan()
        {
            var targets = new BenchmarkTarget();
            var actual = false;

            actual = targets.LessThan(5, 10);

            var expected = true;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestMoreThan()
        {
            var targets = new BenchmarkTarget();
            var actual = targets.MoreThan(10, 5);
            var expected = true;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestLogicalAnd()
        {
            var targets = new BenchmarkTarget();
            var actual = targets.LogicalAnd(0, 0);
            var expected = true;
            Assert.AreEqual(expected, actual);
        }


        [TestMethod]
        public void TestLogicalOr()
        {
            var targets = new BenchmarkTarget();
            var actual = targets.LogicalOr(0, 1);
            var expected = true;
            Assert.AreEqual(expected, actual);
        }
    }

    [TestClass]
    public class BenchmarkXUnit1
    {
        [TestMethod]
        public void TestArray()
        {
            var targets = new BenchmarkTarget1();
            var actual = targets.ConstructArray();
            int[] expected = { 1, 2, 3, 6, 4, 2, 5, 3, 2, 6, 7 };

            Assert.AreEqual(expected, expected);
        }

        [TestMethod]
        public void TestAddition()
        {
            var targets = new BenchmarkTarget1();
            var actual = targets.Addition(5, 4);
            var expected = 9;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSubtraction()
        {
            var targets = new BenchmarkTarget1();
            var actual = targets.Subtraction(5, 4);
            var expected = 1;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestDivision()
        {
            var targets = new BenchmarkTarget1();
            var actual = targets.Division(2, 1);
            var expected = 2;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestMultiplication()
        {
            var targets = new BenchmarkTarget1();
            var actual = targets.Multiplication(2, 2);
            var expected = 4;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestModulo()
        {
            var targets = new BenchmarkTarget1();
            var actual = targets.Modulo(3, 2);
            var expected = 1;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestForLoop()
        {
            var targets = new BenchmarkTarget1();
            targets.ForLoop(10);
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TestWhileLoop()
        {
            var targets = new BenchmarkTarget1();
            var actual = targets.WhileLoop(10);
            var expected = 10;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestIf()
        {
            var targets = new BenchmarkTarget1();
            var actual = targets.If(true);
            var expected = true;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestLessThan()
        {
            var targets = new BenchmarkTarget1();
            var actual = false;

            actual = targets.LessThan(5, 10);

            var expected = true;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestMoreThan()
        {
            var targets = new BenchmarkTarget1();
            var actual = targets.MoreThan(10, 5);
            var expected = true;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestLogicalAnd()
        {
            var targets = new BenchmarkTarget1();
            var actual = targets.LogicalAnd(0, 0);
            var expected = true;
            Assert.AreEqual(expected, actual);
        }


        [TestMethod]
        public void TestLogicalOr()
        {
            var targets = new BenchmarkTarget1();
            var actual = targets.LogicalOr(0, 1);
            var expected = true;
            Assert.AreEqual(expected, actual);
        }
    }

    [TestClass]
    public class BenchmarkXUnit2
    {
        [TestMethod]
        public void TestArray()
        {
            var targets = new BenchmarkTarget2();
            var actual = targets.ConstructArray();
            int[] expected = { 1, 2, 3, 6, 4, 2, 5, 3, 2, 6, 7 };

            Assert.AreEqual(expected, expected);
        }

        [TestMethod]
        public void TestAddition()
        {
            var targets = new BenchmarkTarget2();
            var actual = targets.Addition(5, 4);
            var expected = 9;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSubtraction()
        {
            var targets = new BenchmarkTarget2();
            var actual = targets.Subtraction(5, 4);
            var expected = 1;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestDivision()
        {
            var targets = new BenchmarkTarget2();
            var actual = targets.Division(2, 1);
            var expected = 2;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestMultiplication()
        {
            var targets = new BenchmarkTarget2();
            var actual = targets.Multiplication(2, 2);
            var expected = 4;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestModulo()
        {
            var targets = new BenchmarkTarget2();
            var actual = targets.Modulo(3, 2);
            var expected = 1;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestForLoop()
        {
            var targets = new BenchmarkTarget2();
            targets.ForLoop(10);
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TestWhileLoop()
        {
            var targets = new BenchmarkTarget2();
            var actual = targets.WhileLoop(10);
            var expected = 10;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestIf()
        {
            var targets = new BenchmarkTarget2();
            var actual = targets.If(true);
            var expected = true;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestLessThan()
        {
            var targets = new BenchmarkTarget2();
            var actual = false;

            actual = targets.LessThan(5, 10);

            var expected = true;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestMoreThan()
        {
            var targets = new BenchmarkTarget2();
            var actual = targets.MoreThan(10, 5);
            var expected = true;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestLogicalAnd()
        {
            var targets = new BenchmarkTarget2();
            var actual = targets.LogicalAnd(0, 0);
            var expected = true;
            Assert.AreEqual(expected, actual);
        }


        [TestMethod]
        public void TestLogicalOr()
        {
            var targets = new BenchmarkTarget2();
            var actual = targets.LogicalOr(0, 1);
            var expected = true;
            Assert.AreEqual(expected, actual);
        }
    }

    [TestClass]
    public class BenchmarkXUnit3
    {
        [TestMethod]
        public void TestArray()
        {
            var targets = new BenchmarkTarget3();
            var actual = targets.ConstructArray();
            int[] expected = { 1, 2, 3, 6, 4, 2, 5, 3, 2, 6, 7 };

            Assert.AreEqual(expected, expected);
        }

        [TestMethod]
        public void TestAddition()
        {
            var targets = new BenchmarkTarget3();
            var actual = targets.Addition(5, 4);
            var expected = 9;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestSubtraction()
        {
            var targets = new BenchmarkTarget3();
            var actual = targets.Subtraction(5, 4);
            var expected = 1;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestDivision()
        {
            var targets = new BenchmarkTarget3();
            var actual = targets.Division(2, 1);
            var expected = 2;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestMultiplication()
        {
            var targets = new BenchmarkTarget3();
            var actual = targets.Multiplication(2, 2);
            var expected = 4;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestModulo()
        {
            var targets = new BenchmarkTarget3();
            var actual = targets.Modulo(3, 2);
            var expected = 1;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestForLoop()
        {
            var targets = new BenchmarkTarget3();
            targets.ForLoop(10);
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TestWhileLoop()
        {
            var targets = new BenchmarkTarget3();
            var actual = targets.WhileLoop(10);
            var expected = 10;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestIf()
        {
            var targets = new BenchmarkTarget3();
            var actual = targets.If(true);
            var expected = true;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestLessThan()
        {
            var targets = new BenchmarkTarget3();
            var actual = false;

            actual = targets.LessThan(5, 10);

            var expected = true;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestMoreThan()
        {
            var targets = new BenchmarkTarget3();
            var actual = targets.MoreThan(10, 5);
            var expected = true;
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestLogicalAnd()
        {
            var targets = new BenchmarkTarget3();
            var actual = targets.LogicalAnd(0, 0);
            var expected = true;
            Assert.AreEqual(expected, actual);
        }


        [TestMethod]
        public void TestLogicalOr()
        {
            var targets = new BenchmarkTarget3();
            var actual = targets.LogicalOr(0, 1);
            var expected = true;
            Assert.AreEqual(expected, actual);
        }
    }
}
