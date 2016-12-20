namespace Unity.IL2CPP
{
    using System;

    internal class Statement
    {
        public static string Expression(string expression)
        {
            return (expression + ";");
        }

        public static string Return(string expression)
        {
            return ("return " + Expression(expression));
        }
    }
}

