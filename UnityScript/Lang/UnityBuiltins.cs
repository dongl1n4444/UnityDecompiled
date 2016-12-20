namespace UnityScript.Lang
{
    using System;

    public static class UnityBuiltins
    {
        public static object eval(string code)
        {
            throw new NotImplementedException();
        }

        public static float parseFloat(double value)
        {
            return (float) value;
        }

        public static float parseFloat(int value)
        {
            return (float) value;
        }

        public static float parseFloat(float value)
        {
            return value;
        }

        public static float parseFloat(string value)
        {
            return float.Parse(value);
        }

        public static int parseInt(double value)
        {
            return (int) value;
        }

        public static int parseInt(int value)
        {
            return value;
        }

        public static int parseInt(float value)
        {
            return (int) value;
        }

        public static int parseInt(string value)
        {
            return int.Parse(value);
        }
    }
}

