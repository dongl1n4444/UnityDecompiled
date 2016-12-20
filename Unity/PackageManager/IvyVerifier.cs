namespace Unity.PackageManager
{
    using System;
    using System.IO;
    using System.Text;
    using Unity.PackageManager.Ivy;

    public class IvyVerifier : Verifier
    {
        private string xml;

        public IvyVerifier(Uri path)
        {
            if (File.Exists(path.LocalPath))
            {
                this.xml = File.ReadAllText(path.LocalPath, Encoding.UTF8);
            }
        }

        public IvyVerifier(byte[] bytes)
        {
            this.xml = Encoding.UTF8.GetString(bytes);
        }

        public IvyVerifier(string xml)
        {
            this.xml = xml;
        }

        public override bool Verify()
        {
            if (this.xml == null)
            {
                return false;
            }
            try
            {
                base.Result = IvyParser.Parse<object>(this.xml);
                if (IvyParser.HasErrors)
                {
                    return false;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return false;
            }
            return true;
        }
    }
}

