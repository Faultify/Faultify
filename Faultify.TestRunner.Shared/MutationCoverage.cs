using System;
using System.Collections.Generic;
using System.IO;

namespace Faultify.TestRunner.Shared
{
    // TODO: Uses custom format because Json requires external package.
    // External packages are somehow not working with test data collectors.
    public class MutationCoverage
    {
        /// <summary>
        /// Collection with test names as key and the covered method entity handles as value.
        /// </summary>
        public Dictionary<string, List<int>> Coverage { get; set; } = new Dictionary<string, List<int>>();

        public byte[] Serialize()
        {
            var memoryStream = new MemoryStream();
            var binaryWriter = new BinaryWriter(memoryStream);
            binaryWriter.Write(Coverage.Count);
            foreach (var (key, value) in Coverage)
            {
                binaryWriter.Write(key);
                binaryWriter.Write(value.Count);
                foreach (var entityHandle in value) binaryWriter.Write(entityHandle);
            }

            return memoryStream.ToArray();
        }

        public static MutationCoverage Deserialize(byte[] data)
        {
            var mutationCoverage = new MutationCoverage();
            var memoryStream = new MemoryStream(data);
            var binaryReader = new BinaryReader(memoryStream);

            var count = binaryReader.ReadInt32();
            for (var i = 0; i < count; i++)
            {
                var key = binaryReader.ReadString();
                var listCount = binaryReader.ReadInt32();
                var entityHandles = new List<int>(listCount);
                for (var j = 0; j < listCount; j++) entityHandles.Add(binaryReader.ReadInt32());
                mutationCoverage.Coverage.Add(key, entityHandles);
            }

            return mutationCoverage;
        }
    }
}