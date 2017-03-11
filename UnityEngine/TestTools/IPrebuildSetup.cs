namespace UnityEngine.TestTools
{
    using System;

    public interface IPrebuildSetup
    {
        /// <summary>
        /// <para>Setup method that is automatically called before the test run.</para>
        /// </summary>
        void Setup();
    }
}

