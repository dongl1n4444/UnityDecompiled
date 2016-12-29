namespace UnityScript.Lang
{
    using System;
    using System.Runtime.CompilerServices;

    public static class Extensions
    {
        public static bool operator ==(this char lhs, string rhs)
        {
            bool flag1 = 1 == rhs.Length;
            if (!flag1)
            {
                return flag1;
            }
            return (lhs == rhs[0]);
        }

        public static bool operator ==(this string lhs, char rhs) => 
            (rhs == lhs);

        public static implicit operator bool(this Enum e) => 
            (((IConvertible) e).ToInt32(null) != 0);

        public static bool operator !=(this char lhs, string rhs)
        {
            bool flag1 = 1 != rhs.Length;
            if (flag1)
            {
                return flag1;
            }
            return (lhs != rhs[0]);
        }

        public static bool operator !=(this string lhs, char rhs) => 
            (rhs != lhs);

        [Extension]
        public static int this[System.Array a] =>
            a.Length;

        [Extension]
        public static int this[string s] =>
            s.Length;
    }
}

