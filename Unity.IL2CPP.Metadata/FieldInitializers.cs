using System;
using System.Collections.Generic;

namespace Unity.IL2CPP.Metadata
{
	public class FieldInitializers : List<FieldInitializer>
	{
		public void Add(string name, object value)
		{
			base.Add(new FieldInitializer
			{
				Name = name,
				Value = value
			});
		}
	}
}
