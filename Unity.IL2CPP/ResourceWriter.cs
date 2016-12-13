using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Unity.IL2CPP
{
	public class ResourceWriter
	{
		private class ResourceRecord
		{
			private readonly byte[] name;

			private readonly byte[] data;

			private readonly int size;

			public ResourceRecord(string name, int size, byte[] data)
			{
				this.name = Encoding.UTF8.GetBytes(name);
				this.size = size;
				this.data = data;
			}

			public void WriteRecord(BinaryWriter writer)
			{
				writer.Write(this.size);
				writer.Write(this.name.Length);
				writer.Write(this.name);
			}

			public void WriteData(BinaryWriter writer)
			{
				writer.Write(this.data);
			}

			public int GetRecordSize()
			{
				return 4 + this.name.Length + 4;
			}
		}

		public static void WriteEmbeddedResources(AssemblyDefinition assembly, Stream stream)
		{
			ResourceWriter.WriteResourceInformation(stream, ResourceWriter.GenerateResourceInfomation(assembly));
		}

		private static List<ResourceWriter.ResourceRecord> GenerateResourceInfomation(AssemblyDefinition assembly)
		{
			List<ResourceWriter.ResourceRecord> list = new List<ResourceWriter.ResourceRecord>();
			foreach (EmbeddedResource current in assembly.MainModule.Resources.OfType<EmbeddedResource>())
			{
				byte[] resourceData = current.GetResourceData();
				list.Add(new ResourceWriter.ResourceRecord(current.Name, resourceData.Length, resourceData));
			}
			return list;
		}

		private static void WriteResourceInformation(Stream stream, List<ResourceWriter.ResourceRecord> resourceRecords)
		{
			int value = ResourceWriter.GetSumOfAllRecordSizes(resourceRecords) + ResourceWriter.GetSizeOfNumberOfRecords();
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			binaryWriter.Write(value);
			binaryWriter.Write(resourceRecords.Count);
			foreach (ResourceWriter.ResourceRecord current in resourceRecords)
			{
				current.WriteRecord(binaryWriter);
			}
			foreach (ResourceWriter.ResourceRecord current2 in resourceRecords)
			{
				current2.WriteData(binaryWriter);
			}
		}

		private static int GetSumOfAllRecordSizes(IEnumerable<ResourceWriter.ResourceRecord> resourceRecords)
		{
			return resourceRecords.Sum((ResourceWriter.ResourceRecord resourceRecord) => resourceRecord.GetRecordSize());
		}

		private static int GetSizeOfNumberOfRecords()
		{
			return 4;
		}
	}
}
