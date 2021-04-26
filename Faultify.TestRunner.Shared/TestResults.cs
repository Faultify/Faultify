using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace Faultify.TestRunner.Shared
{
    /// <summary>
    ///     Test results from a test session.
    /// </summary>
    // TODO: Uses custom format because Json requires external package.
    // External packages are somehow not working with test data collectors.
    public class TestResults
    {
        /// <summary>
        ///     A list of the test result from each test in the session.
        /// </summary>
        public List<TestResult> Tests { get; set; } = new List<TestResult>();

        public byte[] Serialize()
        {
            MemoryStream memoryStream = new MemoryStream();
            BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
            binaryWriter.Write(Tests.Count);
            foreach (TestResult testResult in Tests)
            {
                binaryWriter.Write(testResult.Name);
                binaryWriter.Write((int) testResult.Outcome);
            }

            return memoryStream.ToArray();
        }

        public static TestResults Deserialize(byte[] data)
        {
            TestResults testResults = new TestResults();
            MemoryStream memoryStream = new MemoryStream(data);
            BinaryReader binaryReader = new BinaryReader(memoryStream);
            int count = binaryReader.ReadInt32();
            for (var i = 0; i < count; i++)
            {
                TestResult testResult = new TestResult();
                string name = binaryReader.ReadString();
                TestOutcome testOutcome = (TestOutcome) binaryReader.ReadInt32();
                testResult.Name = name;
                testResult.Outcome = testOutcome;
                testResults.Tests.Add(testResult);
            }

            return testResults;
        }
    }
}
