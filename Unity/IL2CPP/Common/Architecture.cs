namespace Unity.IL2CPP.Common
{
    using System;
    using System.Runtime.CompilerServices;

    public abstract class Architecture
    {
        protected Architecture()
        {
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            return (base.GetType() == obj.GetType());
        }

        public override int GetHashCode() => 
            base.GetType().GetHashCode();

        public static bool operator ==(Unity.IL2CPP.Common.Architecture left, Unity.IL2CPP.Common.Architecture right)
        {
            if (object.ReferenceEquals(left, null) || object.ReferenceEquals(right, null))
            {
                return object.ReferenceEquals(left, right);
            }
            return (left.GetType() == right.GetType());
        }

        public static bool operator !=(Unity.IL2CPP.Common.Architecture left, Unity.IL2CPP.Common.Architecture right) => 
            !(left == right);

        public static Unity.IL2CPP.Common.Architecture BestThisMachineCanRun =>
            new x64Architecture();

        public abstract int Bits { get; }

        public abstract string Name { get; }

        public static Unity.IL2CPP.Common.Architecture OfCurrentProcess =>
            ((IntPtr.Size != 4) ? ((Unity.IL2CPP.Common.Architecture) new x64Architecture()) : ((Unity.IL2CPP.Common.Architecture) new x86Architecture()));
    }
}

