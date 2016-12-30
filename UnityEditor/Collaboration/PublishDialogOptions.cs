namespace UnityEditor.Collaboration
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct PublishDialogOptions
    {
        public string Comments;
        public bool DoPublish;
    }
}

