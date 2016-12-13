using NiceIO;
using System;

namespace Unity.IL2CPP.Common
{
	public class UnusedByteCodeStripper
	{
		public static bool Available
		{
			get
			{
				return Il2CppDependencies.HasUnusedByteCodeStripper || UnitySourceCode.Available;
			}
		}

		public static NPath Path
		{
			get
			{
				NPath result;
				if (Il2CppDependencies.HasUnusedByteCodeStripper)
				{
					result = Il2CppDependencies.UnusedByteCodeStripper;
				}
				else
				{
					if (!UnitySourceCode.Available)
					{
						throw new InvalidOperationException("Could not locate UnusedByteCodeStripper");
					}
					result = UnitySourceCode.Paths.UnusedBytecodeStripper;
				}
				return result;
			}
		}
	}
}
