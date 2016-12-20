namespace UnityEngine.Networking
{
    using System;

    internal class FloatConversion
    {
        public static decimal ToDecimal(ulong value1, ulong value2)
        {
            UIntDecimal num = new UIntDecimal {
                longValue1 = value1,
                longValue2 = value2
            };
            return num.decimalValue;
        }

        public static double ToDouble(ulong value)
        {
            UIntFloat num = new UIntFloat {
                longValue = value
            };
            return num.doubleValue;
        }

        public static float ToSingle(uint value)
        {
            UIntFloat num = new UIntFloat {
                intValue = value
            };
            return num.floatValue;
        }
    }
}

