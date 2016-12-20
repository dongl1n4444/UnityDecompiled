namespace Unity.PackageManager
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    internal static class TaskFactory
    {
        public static Task[] FromExisting(string path)
        {
            List<Task> list = new List<Task>();
            if (Directory.Exists(path))
            {
                string[] files = Directory.GetFiles(path);
                foreach (string str in files)
                {
                    if (Path.GetFileName(str).StartsWith("task-installer-"))
                    {
                        Installer item = Installer.FromExisting(str);
                        if (item != null)
                        {
                            list.Add(item);
                        }
                    }
                }
            }
            return list.ToArray();
        }
    }
}

