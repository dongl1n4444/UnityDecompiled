namespace Unity.Bindings
{
    using System;

    [AttributeUsage(AttributeTargets.Parameter)]
    internal class Unmarshalled : NativeParameterAttribute
    {
        public Unmarshalled()
        {
            base.Unmarshalled = true;
        }
    }
}

