namespace Unity.IL2CPP.Portability
{
    using System;
    using System.Runtime.Remoting.Contexts;

    public static class ContextPortable
    {
        public static int DefaultContextIdPortable
        {
            get
            {
                return Context.DefaultContext.ContextID;
            }
        }
    }
}

