namespace UnityEditor.Android.PostProcessor.Tasks
{
    using System;
    using System.IO;
    using System.Threading;
    using UnityEditor.Android.PostProcessor;
    using UnityEditor.Utils;

    internal class SplitLargeFiles : IPostProcessorTask
    {
        public event ProgressHandler OnProgress;

        public void Execute(PostProcessorContext context)
        {
            if (this.OnProgress != null)
            {
                this.OnProgress(this, "Splitting large asset files");
            }
            string str = context.Get<string>("StagingArea");
            string[] components = new string[] { str, "assets", "bin", "Data" };
            this.SplitFiles(Paths.Combine(components), "*.assets", 0x100000);
            string[] textArray2 = new string[] { str, "assets", "bin", "Data" };
            this.SplitFiles(Paths.Combine(textArray2), "level*", 0x100000);
        }

        private void SplitFiles(string path, string extension, int threshold)
        {
            byte[] buffer = new byte[0x4000];
            foreach (string str in Directory.GetFiles(path, extension, SearchOption.AllDirectories))
            {
                FileInfo info = new FileInfo(str);
                if (info.Length >= threshold)
                {
                    FileStream stream = File.Open(info.FullName, FileMode.Open);
                    long length = info.Length;
                    for (long i = 0L; length > 0L; i += 1L)
                    {
                        long num4 = Math.Min(length, (long) threshold);
                        FileStream stream2 = File.Open(Path.Combine(info.DirectoryName, info.Name + ".split" + i), FileMode.OpenOrCreate);
                        for (long j = num4; j > 0L; j -= 0x4000L)
                        {
                            int count = (int) Math.Min(j, 0x4000L);
                            stream.Read(buffer, 0, count);
                            stream2.Write(buffer, 0, count);
                        }
                        stream2.Close();
                        length -= threshold;
                    }
                    stream.Close();
                    File.Delete(str);
                }
            }
        }

        public string Name =>
            "Splitting assets";
    }
}

