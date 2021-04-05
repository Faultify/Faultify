using System.Collections.Generic;
using System.IO;

namespace Faultify.TestRunner.Shared
{
    public class RegisteredCoverage
    {
        public RegisteredCoverage(string assemblyName, int entityHandle)
        {
            AssemblyName = assemblyName;
            EntityHandle = entityHandle;
        }

        public string AssemblyName { get; }
        public int EntityHandle { get; }

        public override int GetHashCode()
        {
            return (AssemblyName + ":" + EntityHandle).GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is RegisteredCoverage objCast && AssemblyName == objCast.AssemblyName &&
                   EntityHandle == objCast.EntityHandle;
        }
    }

    // TODO: Uses custom format because Json requires external package.
    // External packages are somehow not working with test data collectors.
    public class MutationCoverage
    {
        /// <summary>
        ///     Collection with test names as key and the covered method entity handles as value.
        /// </summary>
        public Dictionary<string, List<RegisteredCoverage>> Coverage { get; set; } =
            new Dictionary<string, List<RegisteredCoverage>>();

        public byte[] Serialize()
        {
            var memoryStream = new MemoryStream();
            var binaryWriter = new BinaryWriter(memoryStream);
            binaryWriter.Write(Coverage.Count);
            foreach ((string key, List<RegisteredCoverage> value) in Coverage)
            {
                binaryWriter.Write(key);
                binaryWriter.Write(value.Count);
                foreach (RegisteredCoverage entityHandle in value)
                {
                    binaryWriter.Write(entityHandle.AssemblyName);
                    binaryWriter.Write(entityHandle.EntityHandle);
                }
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
                string key = binaryReader.ReadString();
                var listCount = binaryReader.ReadInt32();
                List<RegisteredCoverage> entityHandles = new List<RegisteredCoverage>(listCount);
                for (var j = 0; j < listCount; j++)
                {
                    string fullQualifiedName = binaryReader.ReadString();
                    int entityHandle = binaryReader.ReadInt32();
                    entityHandles.Add(new RegisteredCoverage(fullQualifiedName, entityHandle));
                }

                mutationCoverage.Coverage.Add(key, entityHandles);
            }

            return mutationCoverage;
        }
    }
}