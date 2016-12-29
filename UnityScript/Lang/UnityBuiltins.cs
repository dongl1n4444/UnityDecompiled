namespace UnityScript.Lang
{
    using System;

    public static class UnityBuiltins
    {
        public static object eval(string code)
        {
            throw new NotImplementedException();
        }

        public static float parseFloat(double value) => 
            ((float) value);

        public static float parseFloat(int value) => 
            ((float) value);

        public static float parseFloat(float value) => 
            value;

        public static float parseFloat(string value) => 
            float.Parse(value);

        public static int parseInt(double value) => 
            ((int) value);

        public static int parseInt(int value) => 
            value;

        public static int parseInt(float value) => 
            ((int) value);

        public static int parseInt(string value) => 
            int.Parse(value);
    }
}

