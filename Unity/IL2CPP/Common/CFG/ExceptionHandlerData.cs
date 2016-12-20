namespace Unity.IL2CPP.Common.CFG
{
    using System;
    using System.Collections.Generic;

    public class ExceptionHandlerData : IComparable<ExceptionHandlerData>
    {
        private List<CatchHandlerData> catches = new List<CatchHandlerData>();
        private BlockRange fault_range;
        private BlockRange finally_range;
        private BlockRange try_range;

        public ExceptionHandlerData(BlockRange try_range)
        {
            this.try_range = try_range;
        }

        public int CompareTo(ExceptionHandlerData data)
        {
            return (this.try_range.Start.First.Offset - data.try_range.Start.First.Offset);
        }

        public List<CatchHandlerData> Catches
        {
            get
            {
                return this.catches;
            }
        }

        public BlockRange FaultRange
        {
            get
            {
                return this.fault_range;
            }
            set
            {
                this.fault_range = value;
            }
        }

        public BlockRange FinallyRange
        {
            get
            {
                return this.finally_range;
            }
            set
            {
                this.finally_range = value;
            }
        }

        public BlockRange TryRange
        {
            get
            {
                return this.try_range;
            }
            set
            {
                this.try_range = value;
            }
        }
    }
}

