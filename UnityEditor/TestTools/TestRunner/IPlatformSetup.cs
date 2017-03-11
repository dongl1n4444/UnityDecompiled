namespace UnityEditor.TestTools.TestRunner
{
    using System;

    internal interface IPlatformSetup
    {
        void CleanUp();
        void Setup();
    }
}

