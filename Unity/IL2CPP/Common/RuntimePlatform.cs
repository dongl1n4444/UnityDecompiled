namespace Unity.IL2CPP.Common
{
    using System;

    public abstract class RuntimePlatform
    {
        protected RuntimePlatform()
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

        public override int GetHashCode()
        {
            return base.GetType().GetHashCode();
        }

        public static bool operator ==(RuntimePlatform left, RuntimePlatform right)
        {
            if (object.ReferenceEquals(left, null) || object.ReferenceEquals(right, null))
            {
                return object.ReferenceEquals(left, right);
            }
            return (left.GetType() == right.GetType());
        }

        public static bool operator !=(RuntimePlatform left, RuntimePlatform right)
        {
            return !(left == right);
        }

        public static RuntimePlatform Current
        {
            get
            {
                if (PlatformUtils.IsWindows())
                {
                    return new WindowsDesktopRuntimePlatform();
                }
                if (PlatformUtils.IsLinux())
                {
                    return new LinuxRuntimePlatform();
                }
                if (!PlatformUtils.IsOSX())
                {
                    throw new Exception("Running on unexpected OS");
                }
                return new MacOSXRuntimePlatform();
            }
        }

        public virtual bool ExecutesOnHostMachine
        {
            get
            {
                return true;
            }
        }

        public abstract string Name { get; }
    }
}

