namespace UnityEditor.Facebook
{
    using System;
    using UnityEditor;
    using UnityEngine;

    internal class SDKManagerData : ScriptableSingleton<SDKManagerData>
    {
        [SerializeField]
        private long m_lastSDKRefreshTicks;

        public static bool CanRefreshSDKNow()
        {
            DateTime time = new DateTime(ScriptableSingleton<SDKManagerData>.instance.m_lastSDKRefreshTicks);
            bool flag = DateTime.UtcNow.Subtract(time).TotalHours > 6.0;
            if (flag)
            {
                ScriptableSingleton<SDKManagerData>.instance.m_lastSDKRefreshTicks = DateTime.UtcNow.Ticks;
            }
            return flag;
        }
    }
}

