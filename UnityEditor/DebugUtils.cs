namespace UnityEditor
{
    using System;
    using System.Collections.Generic;

    internal class DebugUtils
    {
        internal static string ListToString<T>(IEnumerable<T> list)
        {
            if (list == null)
            {
                return "[null list]";
            }
            string str2 = "[";
            int num = 0;
            foreach (T local in list)
            {
                if (num != 0)
                {
                    str2 = str2 + ", ";
                }
                if (local != null)
                {
                    str2 = str2 + local.ToString();
                }
                else
                {
                    str2 = str2 + "'null'";
                }
                num++;
            }
            str2 = str2 + "]";
            if (num == 0)
            {
                return "[empty list]";
            }
            object[] objArray1 = new object[] { "(", num, ") ", str2 };
            return string.Concat(objArray1);
        }
    }
}

