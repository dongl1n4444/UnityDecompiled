namespace UnityEngine
{
    using System;

    /// <summary>
    /// <para>The type of the log message in Debug.logger.Log or delegate registered with Application.RegisterLogCallback.</para>
    /// </summary>
    public enum LogType
    {
        Error,
        Assert,
        Warning,
        Log,
        Exception
    }
}

