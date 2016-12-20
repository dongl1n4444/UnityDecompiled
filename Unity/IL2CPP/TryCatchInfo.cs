namespace Unity.IL2CPP
{
    using System;

    internal class TryCatchInfo
    {
        public int CatchEnd;
        public int CatchStart;
        public int FaultEnd;
        public int FaultStart;
        public int FinallyEnd;
        public int FinallyStart;
        public int TryEnd;
        public int TryStart;

        public override string ToString()
        {
            return string.Format("try {0}:{1}, catch {2}:{3}, finally {4}:{5}, fault {6}:{7}", new object[] { this.TryStart, this.TryEnd, this.CatchStart, this.CatchEnd, this.FinallyStart, this.FinallyEnd, this.FaultStart, this.FaultEnd });
        }
    }
}

