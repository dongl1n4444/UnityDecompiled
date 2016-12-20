namespace Unity.PackageManager.Ivy
{
    using System;
    using System.IO;
    using System.Text;

    internal class UTF8StringWriter : StringWriter
    {
        public UTF8StringWriter(StringBuilder builder) : base(builder)
        {
        }

        public override System.Text.Encoding Encoding
        {
            get
            {
                return System.Text.Encoding.UTF8;
            }
        }
    }
}

