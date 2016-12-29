namespace UnityEditorInternal
{
    using System;
    using System.IO;
    using System.Text.RegularExpressions;

    internal class ProvisioningProfile
    {
        private string m_UUID;
        private static readonly string s_FirstLinePattern = @"<key>UUID<\/key>";
        private static readonly string s_SecondLinePattern = @"<string>((\w*\-?){5})";

        internal ProvisioningProfile()
        {
            this.m_UUID = string.Empty;
        }

        internal ProvisioningProfile(string UUID)
        {
            this.m_UUID = string.Empty;
            this.m_UUID = UUID;
        }

        private static void parseFile(string filePath, ProvisioningProfile profile)
        {
            string str;
            StreamReader reader = new StreamReader(filePath);
            while ((str = reader.ReadLine()) != null)
            {
                if (Regex.Match(str, s_FirstLinePattern).Success && ((str = reader.ReadLine()) != null))
                {
                    Match match2 = Regex.Match(str, s_SecondLinePattern);
                    if (match2.Success)
                    {
                        profile.UUID = match2.Groups[1].Value;
                        break;
                    }
                }
                if (!string.IsNullOrEmpty(profile.UUID))
                {
                    break;
                }
            }
            reader.Close();
        }

        internal static ProvisioningProfile ParseProvisioningProfileAtPath(string pathToFile)
        {
            ProvisioningProfile profile = new ProvisioningProfile();
            parseFile(pathToFile, profile);
            return profile;
        }

        public string UUID
        {
            get => 
                this.m_UUID;
            set
            {
                this.m_UUID = value;
            }
        }
    }
}

