using System;
using System.Text;

namespace Unity.IL2CPP.Portability
{
	public static class StringExtensions
	{
		public static string NormalizeFormCPortable(this string value)
		{
			return value.Normalize(NormalizationForm.FormC);
		}

		public static string NormalizeFormDPortable(this string value)
		{
			return value.Normalize(NormalizationForm.FormD);
		}

		public static string NormalizeFormKCPortable(this string value)
		{
			return value.Normalize(NormalizationForm.FormKC);
		}

		public static string NormalizeFormKDPortable(this string value)
		{
			return value.Normalize(NormalizationForm.FormKD);
		}
	}
}
