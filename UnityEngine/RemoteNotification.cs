namespace UnityEngine
{
    using System;
    using System.Collections;

    [Obsolete("RemoteNotification is deprecated. Please use iOS.RemoteNotification instead (UnityUpgradable) -> UnityEngine.iOS.RemoteNotification", true)]
    public sealed class RemoteNotification
    {
        public string alertBody =>
            null;

        public int applicationIconBadgeNumber =>
            0;

        public bool hasAction =>
            false;

        public string soundName =>
            null;

        public IDictionary userInfo =>
            null;
    }
}

