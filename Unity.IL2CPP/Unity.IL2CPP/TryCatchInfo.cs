using System;

namespace Unity.IL2CPP
{
	internal class TryCatchInfo
	{
		public int TryStart;

		public int TryEnd;

		public int CatchStart;

		public int CatchEnd;

		public int FinallyStart;

		public int FinallyEnd;

		public int FaultStart;

		public int FaultEnd;

		public override string ToString()
		{
			return string.Format("try {0}:{1}, catch {2}:{3}, finally {4}:{5}, fault {6}:{7}", new object[]
			{
				this.TryStart,
				this.TryEnd,
				this.CatchStart,
				this.CatchEnd,
				this.FinallyStart,
				this.FinallyEnd,
				this.FaultStart,
				this.FaultEnd
			});
		}
	}
}
