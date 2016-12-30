namespace UnityEngine.TestTools.Logging
{
    using System;
    using System.Collections.Generic;

    internal interface ILogScope : IDisposable
    {
        List<LogEvent> LogEvents { get; }
    }
}

