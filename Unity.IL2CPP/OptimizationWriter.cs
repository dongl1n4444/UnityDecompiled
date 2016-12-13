using System;
using System.Linq;

namespace Unity.IL2CPP
{
	internal class OptimizationWriter : IDisposable
	{
		private string[] _platformsWithOptimizationsDisabled;

		private CppCodeWriter _writer;

		public OptimizationWriter(CppCodeWriter writer, string methodFullName)
		{
			this._platformsWithOptimizationsDisabled = OptimizationDatabase.GetPlatformsWithDisabledOptimizations(methodFullName);
			if (this._platformsWithOptimizationsDisabled != null)
			{
				this._writer = writer;
				CodeWriter arg_62_0 = this._writer;
				string arg_62_1 = "#if {0}";
				object[] expr_37 = new object[1];
				expr_37[0] = this._platformsWithOptimizationsDisabled.Aggregate((string x, string y) => x + " || " + y);
				arg_62_0.WriteLine(arg_62_1, expr_37);
				this._writer.WriteLine("IL2CPP_DISABLE_OPTIMIZATIONS");
				this._writer.WriteLine("#endif");
			}
		}

		public void Dispose()
		{
			if (this._platformsWithOptimizationsDisabled != null)
			{
				CodeWriter arg_49_0 = this._writer;
				string arg_49_1 = "#if {0}";
				object[] expr_1E = new object[1];
				expr_1E[0] = this._platformsWithOptimizationsDisabled.Aggregate((string x, string y) => x + " || " + y);
				arg_49_0.WriteLine(arg_49_1, expr_1E);
				this._writer.WriteLine("IL2CPP_ENABLE_OPTIMIZATIONS");
				this._writer.WriteLine("#endif");
			}
		}
	}
}
