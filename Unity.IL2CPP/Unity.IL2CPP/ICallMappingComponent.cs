using System;
using System.Collections.Generic;
using System.IO;
using Unity.IL2CPP.Common;
using Unity.IL2CPP.IoCServices;

namespace Unity.IL2CPP
{
	internal class ICallMappingComponent : IIcallMappingService
	{
		private Dictionary<string, string> Map;

		public ICallMappingComponent()
		{
			this.Map = new Dictionary<string, string>();
			this.ReadMap(CommonPaths.Il2CppRoot.Combine(new string[]
			{
				"libil2cpp/libil2cpp.icalls"
			}).ToString());
		}

		private void ReadMap(string path)
		{
			string[] array = File.ReadAllLines(path);
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				string text = array2[i];
				if (!text.StartsWith(";") && !text.StartsWith("#") && !text.StartsWith("//"))
				{
					string[] array3 = text.Split(new char[]
					{
						' '
					});
					if (array3.Length == 2)
					{
						this.Map[array3[0]] = array3[1];
					}
				}
			}
		}

		public string ResolveICallFunction(string icall)
		{
			string result;
			if (this.Map.ContainsKey(icall))
			{
				result = this.Map[icall];
			}
			else
			{
				result = null;
			}
			return result;
		}
	}
}
