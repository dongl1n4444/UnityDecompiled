namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;

    public class ResourceWriter
    {
        [CompilerGenerated]
        private static Func<ResourceRecord, int> <>f__am$cache0;

        private static List<ResourceRecord> GenerateResourceInfomation(AssemblyDefinition assembly)
        {
            List<ResourceRecord> list = new List<ResourceRecord>();
            foreach (EmbeddedResource resource in assembly.MainModule.Resources.OfType<EmbeddedResource>())
            {
                byte[] resourceData = resource.GetResourceData();
                list.Add(new ResourceRecord(resource.Name, resourceData.Length, resourceData));
            }
            return list;
        }

        private static int GetSizeOfNumberOfRecords() => 
            4;

        private static int GetSumOfAllRecordSizes(IEnumerable<ResourceRecord> resourceRecords)
        {
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = resourceRecord => resourceRecord.GetRecordSize();
            }
            return resourceRecords.Sum<ResourceRecord>(<>f__am$cache0);
        }

        public static void WriteEmbeddedResources(AssemblyDefinition assembly, Stream stream)
        {
            WriteResourceInformation(stream, GenerateResourceInfomation(assembly));
        }

        private static void WriteResourceInformation(Stream stream, List<ResourceRecord> resourceRecords)
        {
            int num = GetSumOfAllRecordSizes(resourceRecords) + GetSizeOfNumberOfRecords();
            BinaryWriter writer = new BinaryWriter(stream);
            writer.Write(num);
            writer.Write(resourceRecords.Count);
            foreach (ResourceRecord record in resourceRecords)
            {
                record.WriteRecord(writer);
            }
            foreach (ResourceRecord record2 in resourceRecords)
            {
                record2.WriteData(writer);
            }
        }

        private class ResourceRecord
        {
            private readonly byte[] data;
            private readonly byte[] name;
            private readonly int size;

            public ResourceRecord(string name, int size, byte[] data)
            {
                this.name = Encoding.UTF8.GetBytes(name);
                this.size = size;
                this.data = data;
            }

            public int GetRecordSize() => 
                ((4 + this.name.Length) + 4);

            public void WriteData(BinaryWriter writer)
            {
                writer.Write(this.data);
            }

            public void WriteRecord(BinaryWriter writer)
            {
                writer.Write(this.size);
                writer.Write(this.name.Length);
                writer.Write(this.name);
            }
        }
    }
}

