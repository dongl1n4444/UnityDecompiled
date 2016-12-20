namespace UnityEditor.Android
{
    using System;
    using System.IO;
    using System.Text.RegularExpressions;

    internal class AndroidComponentVersion
    {
        private const string kPkgRevision = "Pkg.Revision";
        private const string kPropertiesFileName = "source.properties";

        public static string GetComponentVersion(string directory)
        {
            string path = Path.Combine(directory, "source.properties");
            if (File.Exists(path))
            {
                string input = "";
                try
                {
                    input = File.ReadAllText(path);
                }
                catch (Exception)
                {
                    return "0";
                }
                Match match = new Regex(string.Format(@"^{0}\s*=\s*([\d.]+).*$", "Pkg.Revision"), RegexOptions.Multiline).Match(input);
                if (match.Success)
                {
                    return match.Groups[1].Value;
                }
            }
            return "0";
        }
    }
}

