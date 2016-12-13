using System;
using System.Text.RegularExpressions;

namespace Unity.DataContract
{
	public class PackageVersion : IComparable
	{
		private static readonly string kVersionMatch = "(?<major>\\d+)\\.(?<minor>\\d+)(\\.(?<micro>\\d+))?(\\.?(?<special>.+))?";

		public int major
		{
			get;
			private set;
		}

		public int minor
		{
			get;
			private set;
		}

		public int micro
		{
			get;
			private set;
		}

		public string special
		{
			get;
			private set;
		}

		public string text
		{
			get;
			private set;
		}

		public int parts
		{
			get;
			private set;
		}

		public PackageVersion(string version)
		{
			if (version != null)
			{
				Match match = Regex.Match(version, PackageVersion.kVersionMatch);
				if (!match.Success)
				{
					throw new ArgumentException("Invalid version: " + version);
				}
				this.major = int.Parse(match.Groups["major"].Value);
				this.minor = int.Parse(match.Groups["minor"].Value);
				this.micro = 0;
				this.special = string.Empty;
				this.parts = 2;
				if (match.Groups["micro"].Success)
				{
					this.micro = int.Parse(match.Groups["micro"].Value);
					this.parts = 3;
				}
				if (match.Groups["special"].Success)
				{
					this.special = match.Groups["special"].Value;
					this.parts = 4;
					if (!this.ValidateSpecial())
					{
						throw new ArgumentException("Invalid version: " + version);
					}
				}
				this.text = version;
			}
		}

		public override string ToString()
		{
			return this.text;
		}

		public int CompareTo(object obj)
		{
			PackageVersion z = obj as PackageVersion;
			int result;
			if (this > z)
			{
				result = 1;
			}
			else if (this == z)
			{
				result = 0;
			}
			else
			{
				result = -1;
			}
			return result;
		}

		public override int GetHashCode()
		{
			int result;
			if (this.text == null)
			{
				result = 0;
			}
			else
			{
				result = this.text.GetHashCode();
			}
			return result;
		}

		public bool IsCompatibleWith(PackageVersion other)
		{
			return !(other == null) && (this == other || (this.parts == 2 && other.parts > 2 && this.major == other.major && this.minor == other.minor) || (this.parts == 3 && other.parts >= 3 && this.major == other.major && this.minor == other.minor && this.micro == other.micro));
		}

		public override bool Equals(object obj)
		{
			PackageVersion z = obj as PackageVersion;
			return this == z;
		}

		public static bool operator ==(PackageVersion a, PackageVersion z)
		{
			return (a == null && z == null) || (a != null && z != null && (a.major == z.major && a.minor == z.minor && a.micro == z.micro) && a.special == z.special);
		}

		public static bool operator !=(PackageVersion a, PackageVersion z)
		{
			return !(a == z);
		}

		public static bool operator >(PackageVersion a, PackageVersion z)
		{
			bool result;
			if (a == null && z == null)
			{
				result = false;
			}
			else if (a == null)
			{
				result = false;
			}
			else if (z == null)
			{
				result = true;
			}
			else if (a == z)
			{
				result = false;
			}
			else if (a.major != z.major)
			{
				result = (a.major > z.major);
			}
			else if (a.minor != z.minor)
			{
				result = (a.minor > z.minor);
			}
			else if (a.micro != z.micro)
			{
				result = (a.micro > z.micro);
			}
			else if (a.parts != z.parts)
			{
				if (a.parts == 4)
				{
					result = char.IsDigit(a.special[0]);
				}
				else
				{
					result = !char.IsDigit(z.special[0]);
				}
			}
			else
			{
				int num = 0;
				int num2 = 0;
				while (num < a.special.Length && num2 < z.special.Length)
				{
					while (num < a.special.Length && num2 < z.special.Length && !char.IsDigit(a.special[num]) && !char.IsDigit(z.special[num2]))
					{
						if (a.special[num] != z.special[num2])
						{
							result = (a.special[num] > z.special[num2]);
							return result;
						}
						num++;
						num2++;
					}
					if (num < a.special.Length && num2 < z.special.Length && (!char.IsDigit(a.special[num]) || !char.IsDigit(z.special[num2])))
					{
						result = char.IsDigit(a.special[num]);
						return result;
					}
					int num3 = PackageVersion.FindFirstNonDigit(a.special.Substring(num));
					int num4 = PackageVersion.FindFirstNonDigit(z.special.Substring(num2));
					int num5 = -1;
					if (num3 > -1)
					{
						num5 = int.Parse(a.special.Substring(num, num3));
						num += num3;
					}
					else
					{
						int.TryParse(a.special.Substring(num), out num5);
						num = a.special.Length;
					}
					int num6 = -1;
					if (num4 > -1)
					{
						num6 = int.Parse(z.special.Substring(num2, num4));
						num2 += num4;
					}
					else
					{
						int.TryParse(z.special.Substring(num2), out num6);
						num2 = z.special.Length;
					}
					if (num5 != num6)
					{
						result = (num5 > num6);
						return result;
					}
				}
				result = (a.special.Length < z.special.Length);
			}
			return result;
		}

		private bool ValidateSpecial()
		{
			int num = 0;
			int num2;
			bool result;
			while (num < this.special.Length && (num2 = PackageVersion.FindFirstDigit(this.special.Substring(num))) >= 0)
			{
				num += num2;
				int num3 = PackageVersion.FindFirstNonDigit(this.special.Substring(num));
				if (num3 < 0)
				{
					num3 = this.special.Length - num;
				}
				if (int.Parse(this.special.Substring(num, num3)) == 0)
				{
					result = false;
					return result;
				}
				num += num3;
			}
			result = true;
			return result;
		}

		private static int FindFirstNonDigit(string str)
		{
			int result;
			for (int i = 0; i < str.Length; i++)
			{
				if (!char.IsDigit(str[i]))
				{
					result = i;
					return result;
				}
			}
			result = -1;
			return result;
		}

		private static int FindFirstDigit(string str)
		{
			int result;
			for (int i = 0; i < str.Length; i++)
			{
				if (char.IsDigit(str[i]))
				{
					result = i;
					return result;
				}
			}
			result = -1;
			return result;
		}

		public static bool operator <(PackageVersion a, PackageVersion z)
		{
			return a != z && !(a > z);
		}

		public static bool operator >=(PackageVersion a, PackageVersion z)
		{
			return a == z || a > z;
		}

		public static bool operator <=(PackageVersion a, PackageVersion z)
		{
			return a == z || a < z;
		}

		public static implicit operator string(PackageVersion version)
		{
			string result;
			if (version == null)
			{
				result = null;
			}
			else
			{
				result = version.ToString();
			}
			return result;
		}
	}
}
