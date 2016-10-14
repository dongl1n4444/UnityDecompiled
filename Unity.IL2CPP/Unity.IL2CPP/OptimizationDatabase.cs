using System;
using System.Collections.Generic;

namespace Unity.IL2CPP
{
	internal static class OptimizationDatabase
	{
		private static Dictionary<string, string[]> _disabledOptimizationMap = new Dictionary<string, string[]>
		{
			{
				"System.Void Mono.Globalization.Unicode.MSCompatUnicodeTable::.cctor()",
				new string[]
				{
					"IL2CPP_TARGET_XBOXONE"
				}
			}
		};

		public static string[] GetPlatformsWithDisabledOptimizations(string methodFullName)
		{
			string[] result = null;
			OptimizationDatabase._disabledOptimizationMap.TryGetValue(methodFullName, out result);
			return result;
		}
	}
}
