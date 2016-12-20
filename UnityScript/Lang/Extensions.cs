namespace UnityScript.Lang
{
    using System;
    using System.Runtime.CompilerServices;

    [Extension]
    public static class Extensions
    {
        [Extension]
        public static bool operator ==(char lhs, string rhs)
        {
            bool flag1 = 1 == rhs.Length;
            if (!flag1)
            {
                return flag1;
            }
            return (lhs == rhs[0]);
        }

        [Extension]
        public static bool operator ==(string lhs, char rhs)
        {
            return (rhs == lhs);
        }

        [Extension]
        public static implicit operator bool(Enum e)
        {
            return (((IConvertible) e).ToInt32(null) != 0);
        }

        [Extension]
        public static bool operator !=(char lhs, string rhs)
        {
            bool flag1 = 1 != rhs.Length;
            if (flag1)
            {
                return flag1;
            }
            return (lhs != rhs[0]);
        }

        [Extension]
        public static bool operator !=(string lhs, char rhs)
        {
            return (rhs != lhs);
        }

        [Extension]
        public static int this[System.Array a]
        {
            get
            {
                return a.Length;
            }
        }

        [Extension]
        public static int this[string s]
        {
            get
            {
                return s.Length;
            }
        }
    }
}

