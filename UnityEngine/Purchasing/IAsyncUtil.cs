namespace UnityEngine.Purchasing
{
    using System;

    internal interface IAsyncUtil
    {
        void Get(string url, Action<string> responseHandler, Action<string> errorHandler);
        void Schedule(Action a, int delayInSeconds);
    }
}

