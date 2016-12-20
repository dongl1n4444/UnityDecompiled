namespace Unity.IL2CPP.Metadata
{
    using System;
    using System.Collections.Generic;

    public class FieldInitializers : List<FieldInitializer>
    {
        public void Add(string name, object value)
        {
            FieldInitializer item = new FieldInitializer {
                Name = name,
                Value = value
            };
            base.Add(item);
        }
    }
}

