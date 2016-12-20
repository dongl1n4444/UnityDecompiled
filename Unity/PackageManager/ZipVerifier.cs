namespace Unity.PackageManager
{
    using ICSharpCode.SharpZipLib.Zip;
    using Mono.Unix.Native;
    using System;
    using System.Collections;
    using System.IO;

    public class ZipVerifier : Verifier
    {
        private readonly string file;
        private readonly string localPath;

        public ZipVerifier(string zipFile, string extractedPath)
        {
            this.file = zipFile;
            this.localPath = extractedPath;
            base.Name = "Unzip Verifier Task";
            base.ProgressMessage = "Verifying";
        }

        public override bool Verify()
        {
            ZipFile file = null;
            try
            {
                file = new ZipFile(File.OpenRead(this.file));
                int num = 0;
                string str = null;
                IEnumerator enumerator = file.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        ZipEntry current = (ZipEntry) enumerator.Current;
                        if (!current.get_IsDirectory())
                        {
                            string str2 = current.get_Name();
                            if (str2.EndsWith("ivy.xml"))
                            {
                                str2 = str2.Replace("ivy.xml", "ivy-waiting-for-unzip-to-end");
                                str = Path.Combine(this.localPath, Path.GetDirectoryName(str2));
                            }
                            string str3 = Path.Combine(this.localPath, str2);
                            if (!File.Exists(str3))
                            {
                                Console.WriteLine("ZipVerifier: file doesn't exist: {0}", str3);
                                return false;
                            }
                            if (((Environment.OSVersion.Platform == PlatformID.Unix) || (Environment.OSVersion.Platform == PlatformID.MacOSX)) && (current.get_ExternalFileAttributes() != 0))
                            {
                                Stat stat;
                                if (Syscall.stat(str3, out stat) != 0)
                                {
                                    Console.WriteLine("ZipVerifier: couldn't stat {0}", str3);
                                    return false;
                                }
                                if (((FilePermissions) current.get_ExternalFileAttributes()) != (((FilePermissions) current.get_ExternalFileAttributes()) & stat.st_mode))
                                {
                                    Console.WriteLine("ZipVerifier: permissions don't match: {0} vs {1}", current.get_ExternalFileAttributes(), stat.st_mode);
                                    return false;
                                }
                            }
                            this.UpdateProgress(((float) (++num)) / ((float) file.get_Size()));
                        }
                    }
                }
                finally
                {
                    IDisposable disposable = enumerator as IDisposable;
                    if (disposable != null)
                    {
                        disposable.Dispose();
                    }
                }
                string path = Path.Combine(str, "ivy-waiting-for-unzip-to-end");
                if (!File.Exists(path))
                {
                    Console.WriteLine("ZipVerifier: Missing package description for {0}", str);
                    return false;
                }
                File.Move(path, Path.Combine(str, "ivy.xml"));
            }
            catch
            {
                throw;
            }
            finally
            {
                if (file != null)
                {
                    file.Close();
                }
            }
            return true;
        }
    }
}

