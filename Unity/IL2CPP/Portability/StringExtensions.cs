namespace Unity.IL2CPP.Portability
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Text;

    [Extension]
    public static class StringExtensions
    {
        [Extension]
        public static string NormalizeFormCPortable(string value)
        {
            return value.Normalize(NormalizationForm.FormC);
        }

        [Extension]
        public static string NormalizeFormDPortable(string value)
        {
            return value.Normalize(NormalizationForm.FormD);
        }

        [Extension]
        public static string NormalizeFormKCPortable(string value)
        {
            return value.Normalize(NormalizationForm.FormKC);
        }

        [Extension]
        public static string NormalizeFormKDPortable(string value)
        {
            return value.Normalize(NormalizationForm.FormKD);
        }
    }
}

