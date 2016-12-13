using NiceIO;
using System;
using System.Collections.Generic;

namespace Unity.IL2CPP.Common
{
	public class MonoInstall
	{
		private readonly NPath _installRoot;

		public static MonoInstall TwoSix = new MonoInstall("Mono");

		public static MonoInstall BleedingEdge = new MonoInstall("MonoBleedingEdge");

		public NPath ConfigPath
		{
			get
			{
				return this._installRoot.Combine(new string[]
				{
					"etc"
				});
			}
		}

		public NPath Cli
		{
			get
			{
				return this._MonoExecutable("bin/cli");
			}
		}

		public NPath _Gmcs
		{
			get
			{
				return this._MonoExecutable("bin/gmcs");
			}
		}

		public NPath Mcs
		{
			get
			{
				return this._MonoExecutable("bin/mcs");
			}
		}

		public NPath Root
		{
			get
			{
				return this._installRoot;
			}
		}

		private MonoInstall(string installName)
		{
			this._installRoot = MonoInstall.FindInstallRoot(installName);
		}

		public static NPath SmartProfilePath(DotNetProfile profile)
		{
			NPath result;
			if (profile == DotNetProfile.Net45)
			{
				result = MonoInstall.BleedingEdge.ProfilePath(profile);
			}
			else
			{
				result = MonoInstall.TwoSix.ProfilePath(profile);
			}
			return result;
		}

		public static NPath SmartCompiler(DotNetProfile profile)
		{
			NPath result;
			if (profile == DotNetProfile.Net45)
			{
				result = MonoInstall.BleedingEdge.Mcs;
			}
			else
			{
				result = MonoInstall.TwoSix._Gmcs;
			}
			return result;
		}

		public static MonoInstall SmartInstall(DotNetProfile profile)
		{
			MonoInstall result;
			if (profile == DotNetProfile.Net45)
			{
				result = MonoInstall.BleedingEdge;
			}
			else
			{
				result = MonoInstall.TwoSix;
			}
			return result;
		}

		public NPath ProfilePath(DotNetProfile profile)
		{
			return this._installRoot.Combine(new string[]
			{
				"lib/mono"
			}).Combine(new string[]
			{
				MonoInstall.ProfileDirectoryName(profile)
			});
		}

		private static string ProfileDirectoryName(DotNetProfile profile)
		{
			string result;
			switch (profile)
			{
			case DotNetProfile.Unity:
				result = "unity";
				break;
			case DotNetProfile.Net20:
				result = "2.0";
				break;
			case DotNetProfile.Net45:
				result = "4.5";
				break;
			default:
				throw new InvalidOperationException(string.Format("Unknown profile : {0}", profile));
			}
			return result;
		}

		public static NPath MonoBleedingEdgeExecutableForArch(bool use64BitMono)
		{
			NPath result;
			if (PlatformUtils.IsWindows() && use64BitMono)
			{
				result = MonoInstall.BleedingEdge.Root.Combine(new string[]
				{
					"bin-x64",
					"mono.exe"
				});
			}
			else if (PlatformUtils.IsOSX())
			{
				result = MonoInstall.BleedingEdge.Root.Combine(new string[]
				{
					"bin",
					"mono"
				});
			}
			else
			{
				result = MonoInstall.BleedingEdge.Cli;
			}
			return result;
		}

		public static IEnumerable<string> ArgumentsForArchCommand(string monoExecutable, bool use64BitMono)
		{
			if (!PlatformUtils.IsOSX())
			{
				throw new InvalidOperationException("The platform should be OSX to call this method.");
			}
			return new List<string>
			{
				(!use64BitMono) ? "-i386" : "-x86_64",
				monoExecutable
			};
		}

		private NPath _MonoExecutable(string program)
		{
			return this._installRoot.Combine(new string[]
			{
				program + ((!PlatformUtils.IsWindows()) ? "" : ".bat")
			});
		}

		private static NPath FindInstallRoot(string installName)
		{
			NPath result;
			if (Il2CppDependencies.Available && !UnitySourceCode.Available)
			{
				result = Il2CppDependencies.MonoInstall(installName);
			}
			else
			{
				NPath nPath = CommonPaths.Il2CppRoot.Parent.Combine(new string[]
				{
					installName
				});
				if (nPath.DirectoryExists(""))
				{
					result = nPath;
				}
				else
				{
					if (!UnitySourceCode.Available)
					{
						throw new InvalidOperationException("Unable to find mono install at: " + nPath);
					}
					result = UnitySourceCode.Paths.UnityRoot.Combine(new string[]
					{
						"External/" + installName + "/builds/monodistribution"
					});
				}
			}
			return result;
		}
	}
}
