namespace Unity.IL2CPP.Building
{
    using System;

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

        public static bool operator ==(Architecture left, Architecture right)
        {
            if (object.ReferenceEquals(left, null) || object.ReferenceEquals(right, null))
            {
                return object.ReferenceEquals(left, right);
            }
            return (left.GetType() == right.GetType());
        }

        public static bool operator !=(Architecture left, Architecture right) => 
            !(left == right);

        public static Architecture BestThisMachineCanRun =>
            new x64Architecture();

        public abstract int Bits { get; }

        public abstract string Name { get; }

        public static Architecture OfCurrentProcess =>
            ((IntPtr.Size != 4) ? ((Architecture) new x64Architecture()) : ((Architecture) new x86Architecture()));
    }
}

