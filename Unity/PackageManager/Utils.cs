namespace Unity.PackageManager
{
    using System;
    using System.Diagnostics;
    using System.IO;

    public class Utils
    {
        internal static string BuildPath(params string[] parts)
        {
            string str = "";
            for (int i = parts.Length - 1; i >= 0; i--)
            {
                if (!string.IsNullOrEmpty(parts[i]))
                {
                    str = Path.Combine(parts[i], str);
                }
            }
            return str;
        }

        public static bool Copy(Stream source, Stream destination, int chunkSize) => 
            Copy(source, destination, chunkSize, 0L, null, 0x3e8);

        public static bool Copy(Stream source, Stream destination, int chunkSize, long totalSize, Func<long, long, bool> progress, int progressUpdateRate)
        {
            byte[] buffer = new byte[chunkSize];
            int count = 0;
            long num2 = 0L;
            float num3 = -1f;
            float num4 = 0f;
            float num5 = 0.005f;
            long num6 = 0L;
            long num7 = 0L;
            Stopwatch stopwatch = null;
            bool flag2 = (totalSize > 0L) && (progress != null);
            if (flag2)
            {
                stopwatch = new Stopwatch();
            }
            do
            {
                if (flag2)
                {
                    stopwatch.Start();
                }
                count = source.Read(buffer, 0, chunkSize);
                if (flag2)
                {
                    stopwatch.Stop();
                }
                num2 += count;
                if (count > 0)
                {
                    destination.Write(buffer, 0, count);
                    if (flag2)
                    {
                        num6 += count;
                        if ((stopwatch.ElapsedMilliseconds >= progressUpdateRate) || (num2 == totalSize))
                        {
                            stopwatch.Reset();
                            num4 = num6;
                            num6 = 0L;
                            num3 = (num3 >= 0f) ? ((num5 * num4) + ((1f - num5) * num3)) : num4;
                            num7 = Math.Max(1L, (long) (((float) (totalSize - num2)) / (num3 / ((float) progressUpdateRate))));
                            if (!progress.Invoke(num2, num7))
                            {
                                break;
                            }
                        }
                    }
                }
            }
            while (count > 0);
            if (num2 > 0L)
            {
                destination.Flush();
            }
            return true;
        }

        internal static DirectoryInfo GetTempDirectory()
        {
            string tempFileName;
            do
            {
                tempFileName = Path.GetTempFileName();
                try
                {
                    File.Delete(tempFileName);
                }
                catch
                {
                }
                string[] parts = new string[] { "unity", Path.GetDirectoryName(tempFileName), Path.GetFileNameWithoutExtension(tempFileName) };
                tempFileName = BuildPath(parts);
            }
            while (Directory.Exists(tempFileName));
            return Directory.CreateDirectory(tempFileName);
        }
    }
}

