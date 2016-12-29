namespace Unity.DataContract
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Text.RegularExpressions;

    public class PackageVersion : IComparable
    {
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int <major>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int <micro>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int <minor>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int <parts>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <special>k__BackingField;
        [CompilerGenerated, DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string <text>k__BackingField;
        private static readonly string kVersionMatch = @"(?<major>\d+)\.(?<minor>\d+)(\.(?<micro>\d+))?(\.?(?<special>.+))?";

        public PackageVersion(string version)
        {
            if (version != null)
            {
                Match match = Regex.Match(version, kVersionMatch);
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

        public int CompareTo(object obj)
        {
            PackageVersion version = obj as PackageVersion;
            if (this > version)
            {
                return 1;
            }
            if (this == version)
            {
                return 0;
            }
            return -1;
        }

        public override bool Equals(object obj)
        {
            PackageVersion version = obj as PackageVersion;
            return (this == version);
        }

        private static int FindFirstDigit(string str)
        {
            for (int i = 0; i < str.Length; i++)
            {
                if (char.IsDigit(str[i]))
                {
                    return i;
                }
            }
            return -1;
        }

        private static int FindFirstNonDigit(string str)
        {
            for (int i = 0; i < str.Length; i++)
            {
                if (!char.IsDigit(str[i]))
                {
                    return i;
                }
            }
            return -1;
        }

        public override int GetHashCode() => 
            this.text?.GetHashCode();

        public bool IsCompatibleWith(PackageVersion other)
        {
            if (other == null)
            {
                return false;
            }
            return ((this == other) || ((((this.parts == 2) && (other.parts > 2)) && ((this.major == other.major) && (this.minor == other.minor))) || ((((this.parts == 3) && (other.parts >= 3)) && ((this.major == other.major) && (this.minor == other.minor))) && (this.micro == other.micro))));
        }

        public static bool operator ==(PackageVersion a, PackageVersion z)
        {
            if ((a == null) && (z == null))
            {
                return true;
            }
            if ((a == null) || (z == null))
            {
                return false;
            }
            return ((((a.major == z.major) && (a.minor == z.minor)) && (a.micro == z.micro)) && (a.special == z.special));
        }

        public static bool operator >(PackageVersion a, PackageVersion z)
        {
            if ((a == null) && (z == null))
            {
                return false;
            }
            if (a == null)
            {
                return false;
            }
            if (z == null)
            {
                return true;
            }
            if (a == z)
            {
                return false;
            }
            if (a.major != z.major)
            {
                return (a.major > z.major);
            }
            if (a.minor != z.minor)
            {
                return (a.minor > z.minor);
            }
            if (a.micro != z.micro)
            {
                return (a.micro > z.micro);
            }
            if (a.parts != z.parts)
            {
                if (a.parts == 4)
                {
                    return char.IsDigit(a.special[0]);
                }
                return !char.IsDigit(z.special[0]);
            }
            int startIndex = 0;
            int num2 = 0;
            while ((startIndex < a.special.Length) && (num2 < z.special.Length))
            {
                while (((startIndex < a.special.Length) && (num2 < z.special.Length)) && (!char.IsDigit(a.special[startIndex]) && !char.IsDigit(z.special[num2])))
                {
                    if (a.special[startIndex] != z.special[num2])
                    {
                        return (a.special[startIndex] > z.special[num2]);
                    }
                    startIndex++;
                    num2++;
                }
                if (((startIndex < a.special.Length) && (num2 < z.special.Length)) && (!char.IsDigit(a.special[startIndex]) || !char.IsDigit(z.special[num2])))
                {
                    return char.IsDigit(a.special[startIndex]);
                }
                int length = FindFirstNonDigit(a.special.Substring(startIndex));
                int num4 = FindFirstNonDigit(z.special.Substring(num2));
                int result = -1;
                if (length > -1)
                {
                    result = int.Parse(a.special.Substring(startIndex, length));
                    startIndex += length;
                }
                else
                {
                    int.TryParse(a.special.Substring(startIndex), out result);
                    startIndex = a.special.Length;
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
                if (result != num6)
                {
                    return (result > num6);
                }
            }
            return (a.special.Length < z.special.Length);
        }

        public static bool operator >=(PackageVersion a, PackageVersion z) => 
            ((a == z) || (a > z));

        public static implicit operator string(PackageVersion version) => 
            version?.ToString();

        public static bool operator !=(PackageVersion a, PackageVersion z) => 
            !(a == z);

        public static bool operator <(PackageVersion a, PackageVersion z) => 
            ((a != z) && (a <= z));

        public static bool operator <=(PackageVersion a, PackageVersion z) => 
            ((a == z) || (a < z));

        public override string ToString() => 
            this.text;

        private bool ValidateSpecial()
        {
            int startIndex = 0;
            int num2 = 0;
            while ((startIndex < this.special.Length) && ((num2 = FindFirstDigit(this.special.Substring(startIndex))) >= 0))
            {
                startIndex += num2;
                int length = FindFirstNonDigit(this.special.Substring(startIndex));
                if (length < 0)
                {
                    length = this.special.Length - startIndex;
                }
                if (int.Parse(this.special.Substring(startIndex, length)) == 0)
                {
                    return false;
                }
                startIndex += length;
            }
            return true;
        }

        public int major { get; private set; }

        public int micro { get; private set; }

        public int minor { get; private set; }

        public int parts { get; private set; }

        public string special { get; private set; }

        public string text { get; private set; }
    }
}

