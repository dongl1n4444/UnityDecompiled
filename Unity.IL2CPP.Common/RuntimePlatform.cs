using System;

namespace Unity.IL2CPP.Common
{
	public abstract class RuntimePlatform
	{
		public abstract string Name
		{
			get;
		}

		public virtual bool ExecutesOnHostMachine
		{
			get
			{
				return true;
			}
		}

		public static RuntimePlatform Current
		{
			get
			{
				RuntimePlatform result;
				if (PlatformUtils.IsWindows())
				{
					result = new WindowsDesktopRuntimePlatform();
				}
				else if (PlatformUtils.IsLinux())
				{
					result = new LinuxRuntimePlatform();
				}
				else
				{
					if (!PlatformUtils.IsOSX())
					{
						throw new Exception("Running on unexpected OS");
					}
					result = new MacOSXRuntimePlatform();
				}
				return result;
			}
		}

		public static bool operator ==(RuntimePlatform left, RuntimePlatform right)
		{
			bool result;
			if (object.ReferenceEquals(left, null) || object.ReferenceEquals(right, null))
			{
				result = object.ReferenceEquals(left, right);
			}
			else
			{
				result = (left.GetType() == right.GetType());
			}
			return result;
		}

		public static bool operator !=(RuntimePlatform left, RuntimePlatform right)
		{
			return !(left == right);
		}

		public override bool Equals(object obj)
		{
			return obj != null && base.GetType() == obj.GetType();
		}

		public override int GetHashCode()
		{
			return base.GetType().GetHashCode();
		}
	}
}
