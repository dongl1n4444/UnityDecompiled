namespace Unity.Bindings
{
    using System;

    [AttributeUsage(AttributeTargets.Method)]
    internal class FreeFunction : NativeMethodAttribute
    {
        public FreeFunction()
        {
            base.IsFreeFunction = true;
        }

        public FreeFunction(string name) : base(name, true)
        {
        }

        public FreeFunction(string name, bool isThreadSafe) : base(name, true, isThreadSafe)
        {
        }
    }
}

