namespace Unity.IL2CPP.Portability
{
    using System;
    using System.Threading;

    public static class ThreadPortable
    {
        public static int CurrentContextIdPortable =>
            Thread.CurrentContext.ContextID;

        public static bool HasCurrentContext =>
            (Thread.CurrentContext != null);
    }
}

