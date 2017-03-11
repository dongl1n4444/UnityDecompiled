namespace UnityEditor.Build
{
    using System;
    using UnityEngine.Scripting;

    /// <summary>
    /// <para>An exception class that represents a failed build.</para>
    /// </summary>
    [RequiredByNativeCode]
    public class BuildFailedException : Exception
    {
        /// <summary>
        /// <para>Constructs a BuildFailedException object.</para>
        /// </summary>
        /// <param name="message">A string of text describing the error that caused the build to fail.</param>
        /// <param name="innerException">The exception that caused the build to fail.</param>
        public BuildFailedException(Exception innerException) : base(null, innerException)
        {
        }

        /// <summary>
        /// <para>Constructs a BuildFailedException object.</para>
        /// </summary>
        /// <param name="message">A string of text describing the error that caused the build to fail.</param>
        /// <param name="innerException">The exception that caused the build to fail.</param>
        public BuildFailedException(string message) : base(message)
        {
        }

        [RequiredByNativeCode]
        private Exception BuildFailedException_GetInnerException() => 
            base.InnerException;
    }
}

