namespace Unity.IL2CPP.Portability
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Text;

    public static class StringExtensions
    {
        public static string NormalizeFormCPortable(this string value) => 
            value.Normalize(NormalizationForm.FormC);

        public static string NormalizeFormDPortable(this string value) => 
            value.Normalize(NormalizationForm.FormD);

        public static string NormalizeFormKCPortable(this string value) => 
            value.Normalize(NormalizationForm.FormKC);

        public static string NormalizeFormKDPortable(this string value) => 
            value.Normalize(NormalizationForm.FormKD);
    }
}

