namespace UnityEditor.Connect
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct UserInfo
    {
        private int m_Valid;
        private string m_UserId;
        private string m_UserName;
        private string m_DisplayName;
        private string m_PrimaryOrg;
        private int m_Whitelisted;
        private string m_OrganizationForeignKeys;
        private string m_AccessToken;
        private int m_AccessTokenValiditySeconds;
        public bool valid =>
            (this.m_Valid != 0);
        public string userId =>
            this.m_UserId;
        public string userName =>
            this.m_UserName;
        public string displayName =>
            this.m_DisplayName;
        public string primaryOrg =>
            this.m_PrimaryOrg;
        public bool whitelisted =>
            (this.m_Whitelisted != 0);
        public string organizationForeignKeys =>
            this.m_OrganizationForeignKeys;
        public string accessToken =>
            this.m_AccessToken;
        public int accessTokenValiditySeconds =>
            this.m_AccessTokenValiditySeconds;
    }
}

