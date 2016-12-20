namespace Unity.IL2CPP.Common
{
    using System;
    using System.IO;
    using System.Reflection;

    public static class ResourceExtraction
    {
        public static byte[] Extract(string filename, Assembly assembly)
        {
            using (Stream stream = assembly.GetManifestResourceStream(filename))
            {
                if (stream == null)
                {
                    return null;
                }
                byte[] buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                return buffer;
            }
        }
    }
}

