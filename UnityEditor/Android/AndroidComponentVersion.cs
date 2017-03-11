namespace UnityEditor.Android
{
    using System;
    using System.IO;
    using System.Text.RegularExpressions;

    internal class AndroidComponentVersion
    {
        private const string kPkgRevision = "Pkg.Revision";
        private const string kPropertiesFileName = "source.properties";

        public static Version GetComponentVersion(string directory)
        {
            string str2;
            string path = Path.Combine(directory, "source.properties");
            if (!File.Exists(path))
            {
                return Utils.DefaultVersion;
            }
            try
            {
                str2 = File.ReadAllText(path);
            }
            catch
            {
                return Utils.DefaultVersion;
            }
            Match match = new Regex($"^{"Pkg.Revision"}\s*=\s*([\d.]+).*$", RegexOptions.Multiline).Match(str2);
            return (!match.Success ? Utils.DefaultVersion : Utils.ParseVersion(match.Groups[1].Value));
        }
    }
}

