namespace Unity.IL2CPP
{
    using System;

    internal class TryCatchInfo
    {
        public int CatchEnd;
        public int CatchStart;
        public int FaultEnd;
        public int FaultStart;
        public int FilterEnd;
        public int FilterStart;
        public int FinallyEnd;
        public int FinallyStart;
        public int TryEnd;
        public int TryStart;

        public override string ToString() => 
            string.Format($"try {this.TryStart}:{this.TryEnd}, filter {this.FilterStart}:{this.FilterEnd}, catch {this.CatchStart}:{this.CatchEnd}, finally {this.FinallyStart}:{this.FinallyEnd}, fault {this.FaultStart}:{this.FaultEnd}", new object[0]);
    }
}

