namespace Unity.PackageManager
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using Unity.PackageManager.Ivy;

    public class BinaryVerifier : Verifier
    {
        private readonly IvyArtifact artifact;
        private readonly Uri localPath;

        public BinaryVerifier(Uri localPath, IvyArtifact artifact)
        {
            this.localPath = localPath;
            this.artifact = artifact;
            base.ProgressMessage = "Verifying";
        }

        public override bool Verify()
        {
            string path = Path.Combine(this.localPath.LocalPath, this.artifact.Filename);
            if (!File.Exists(path))
            {
                Console.WriteLine("Verification failure: local path '{0}' does not exist", path);
                return false;
            }
            string str2 = Path.Combine(this.localPath.LocalPath, this.artifact.MD5Filename);
            if (!File.Exists(str2))
            {
                Console.WriteLine("Verification failure: local checksum file '{0}' does not exist", str2);
                return false;
            }
            char[] separator = new char[] { ' ' };
            string[] strArray = File.ReadAllText(str2).Split(separator, StringSplitOptions.RemoveEmptyEntries);
            if (strArray.Length != 2)
            {
                Console.WriteLine("Verification failure: checksum file has {0} tokens, expected 2", strArray.Length);
                return false;
            }
            string str3 = strArray[0];
            MD5 md = MD5.Create();
            byte[] buffer = null;
            using (FileStream stream = File.OpenRead(path))
            {
                buffer = md.ComputeHash(stream);
            }
            string str4 = "";
            foreach (byte num in buffer)
            {
                str4 = str4 + num.ToString("x2");
            }
            if (!str3.Equals(str4))
            {
                Console.WriteLine("Verification failure: remote checksum {0} does not match local checksum {1}", str3, str4);
                return false;
            }
            return true;
        }
    }
}

