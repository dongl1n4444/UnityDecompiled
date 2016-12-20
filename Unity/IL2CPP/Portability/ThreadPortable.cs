namespace Unity.IL2CPP.Portability
{
    using System;
    using System.Runtime.Remoting.Contexts;
    using System.Threading;

    public static class ThreadPortable
    {
        public static Context CurrentContext
        {
            get
            {
                return Thread.CurrentContext;
            }
        }

        public static int CurrentContextIdPortable
        {
            get
            {
                return Thread.CurrentContext.ContextID;
            }
        }

        public static bool HasCurrentContext
        {
            get
            {
                return (Thread.CurrentContext != null);
            }
        }
    }
}

