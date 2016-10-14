using System;

namespace Unity.IL2CPP
{
	internal class Statement
	{
		public static string Expression(string expression)
		{
			return expression + ";";
		}

		public static string Return(string expression)
		{
			return "return " + Statement.Expression(expression);
		}
	}
}
