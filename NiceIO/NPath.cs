namespace NiceIO
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using Unity.IL2CPP.Portability;

    public class NPath : IEquatable<NPath>
    {
        private readonly string _driveLetter;
        private readonly string[] _elements;
        private readonly bool _isRelative;
        [CompilerGenerated]
        private static Func<string, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<string, bool> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<string, bool> <>f__am$cache2;
        [CompilerGenerated]
        private static Func<string, NPath> <>f__am$cache3;
        [CompilerGenerated]
        private static Func<NPath, bool> <>f__am$cache4;
        [CompilerGenerated]
        private static Func<NPath, IEnumerable<string>> <>f__am$cache5;
        [CompilerGenerated]
        private static Func<string, NPath> <>f__am$cache6;
        [CompilerGenerated]
        private static Func<string, NPath> <>f__am$cache7;
        [CompilerGenerated]
        private static Func<NPath, bool> <>f__am$cache8;
        [CompilerGenerated]
        private static Func<NPath, bool> <>f__mg$cache0;
        private static readonly StringComparison PathStringComparison = (!IsLinux() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);

        public NPath(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException();
            }
            path = this.ParseDriveLetter(path, out this._driveLetter);
            char[] separator = new char[] { '/', '\\' };
            string[] split = path.Split(separator);
            this._isRelative = (this._driveLetter == null) && IsRelativeFromSplitString(split);
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = new Func<string, bool>(null, (IntPtr) <NPath>m__0);
            }
            this._elements = this.ParseSplitStringIntoElements(Enumerable.ToArray<string>(Enumerable.Where<string>(split, <>f__am$cache0)));
        }

        private NPath(string[] elements, bool isRelative, string driveLetter)
        {
            this._elements = elements;
            this._isRelative = isRelative;
            this._driveLetter = driveLetter;
        }

        [CompilerGenerated]
        private static bool <NPath>m__0(string s)
        {
            return (s.Length > 0);
        }

        private static bool AlwaysTrue(NPath p)
        {
            return true;
        }

        public NPath ChangeExtension(string extension)
        {
            string[] elements = (string[]) this._elements.Clone();
            elements[elements.Length - 1] = Path.ChangeExtension(this._elements[this._elements.Length - 1], WithDot(extension));
            if (extension == string.Empty)
            {
                char[] trimChars = new char[] { '.' };
                elements[elements.Length - 1] = elements[elements.Length - 1].TrimEnd(trimChars);
            }
            return new NPath(elements, this._isRelative, this._driveLetter);
        }

        public NPath Combine(params NPath[] append)
        {
            if (<>f__am$cache4 == null)
            {
                <>f__am$cache4 = new Func<NPath, bool>(null, (IntPtr) <Combine>m__4);
            }
            if (!Enumerable.All<NPath>(append, <>f__am$cache4))
            {
                throw new ArgumentException("You cannot .Combine a non-relative path");
            }
            if (<>f__am$cache5 == null)
            {
                <>f__am$cache5 = new Func<NPath, IEnumerable<string>>(null, (IntPtr) <Combine>m__5);
            }
            return new NPath(this.ParseSplitStringIntoElements(Enumerable.Concat<string>(this._elements, Enumerable.SelectMany<NPath, string>(append, <>f__am$cache5))), this._isRelative, this._driveLetter);
        }

        public NPath Combine(params string[] append)
        {
            if (<>f__am$cache3 == null)
            {
                <>f__am$cache3 = new Func<string, NPath>(null, (IntPtr) <Combine>m__3);
            }
            return this.Combine(Enumerable.ToArray<NPath>(Enumerable.Select<string, NPath>(append, <>f__am$cache3)));
        }

        public IEnumerable<NPath> Contents([Optional, DefaultParameterValue(false)] bool recurse)
        {
            return this.Contents("*", recurse);
        }

        public IEnumerable<NPath> Contents(string filter, [Optional, DefaultParameterValue(false)] bool recurse)
        {
            return Enumerable.Concat<NPath>(this.Files(filter, recurse), this.Directories(filter, recurse));
        }

        public NPath Copy(NPath dest)
        {
            if (<>f__am$cache8 == null)
            {
                <>f__am$cache8 = new Func<NPath, bool>(null, (IntPtr) <Copy>m__8);
            }
            return this.Copy(dest, <>f__am$cache8);
        }

        public NPath Copy(string dest)
        {
            return this.Copy(new NPath(dest));
        }

        public NPath Copy(NPath dest, Func<NPath, bool> fileFilter)
        {
            this.ThrowIfRelative();
            if (dest.IsRelative)
            {
                NPath[] append = new NPath[] { dest };
                dest = this.Parent.Combine(append);
            }
            if (dest.DirectoryExists(""))
            {
                string[] textArray1 = new string[] { this.FileName };
                return this.CopyWithDeterminedDestination(dest.Combine(textArray1), fileFilter);
            }
            return this.CopyWithDeterminedDestination(dest, fileFilter);
        }

        public NPath Copy(string dest, Func<NPath, bool> fileFilter)
        {
            return this.Copy(new NPath(dest), fileFilter);
        }

        public IEnumerable<NPath> CopyFiles(NPath destination, bool recurse, [Optional, DefaultParameterValue(null)] Func<NPath, bool> fileFilter)
        {
            <CopyFiles>c__AnonStorey1 storey = new <CopyFiles>c__AnonStorey1 {
                destination = destination,
                $this = this
            };
            storey.destination.EnsureDirectoryExists("");
            if ((fileFilter == null) && (<>f__mg$cache0 == null))
            {
                <>f__mg$cache0 = new Func<NPath, bool>(null, (IntPtr) AlwaysTrue);
            }
            return Enumerable.ToArray<NPath>(Enumerable.Select<NPath, NPath>(Enumerable.Where<NPath>(this.Files(recurse), <>f__mg$cache0), new Func<NPath, NPath>(storey, (IntPtr) this.<>m__0)));
        }

        private NPath CopyWithDeterminedDestination(NPath absoluteDestination, Func<NPath, bool> fileFilter)
        {
            if (absoluteDestination.IsRelative)
            {
                throw new ArgumentException("absoluteDestination must be absolute");
            }
            if (this.FileExists(""))
            {
                if (!fileFilter.Invoke(absoluteDestination))
                {
                    return null;
                }
                absoluteDestination.EnsureParentDirectoryExists();
                File.Copy(this.ToString(), absoluteDestination.ToString(), true);
                return absoluteDestination;
            }
            if (!this.DirectoryExists(""))
            {
                throw new ArgumentException("Copy() called on path that doesnt exist: " + this.ToString());
            }
            absoluteDestination.EnsureDirectoryExists("");
            foreach (NPath path2 in this.Contents(false))
            {
                NPath[] append = new NPath[] { path2.RelativeTo(this) };
                path2.CopyWithDeterminedDestination(absoluteDestination.Combine(append), fileFilter);
            }
            return absoluteDestination;
        }

        public NPath CreateDirectory()
        {
            this.ThrowIfRelative();
            Directory.CreateDirectory(this.ToString());
            return this;
        }

        public NPath CreateDirectory(NPath directory)
        {
            if (!directory.IsRelative)
            {
                throw new ArgumentException("Cannot call CreateDirectory with an absolute argument");
            }
            NPath[] append = new NPath[] { directory };
            return this.Combine(append).CreateDirectory();
        }

        public NPath CreateDirectory(string directory)
        {
            return this.CreateDirectory(new NPath(directory));
        }

        public NPath CreateFile()
        {
            this.ThrowIfRelative();
            this.EnsureParentDirectoryExists();
            File.WriteAllBytes(this.ToString(), new byte[0]);
            return this;
        }

        public NPath CreateFile(NPath file)
        {
            if (!file.IsRelative)
            {
                throw new ArgumentException("You cannot call CreateFile() on an existing path with a non relative argument");
            }
            NPath[] append = new NPath[] { file };
            return this.Combine(append).CreateFile();
        }

        public NPath CreateFile(string file)
        {
            return this.CreateFile(new NPath(file));
        }

        public static NPath CreateTempDirectory(string myprefix)
        {
            Random random = new Random();
            while (true)
            {
                object[] objArray1 = new object[] { Path.GetTempPath(), "/", myprefix, "_", random.Next() };
                NPath path = new NPath(string.Concat(objArray1));
                if (!path.Exists(""))
                {
                    return path.CreateDirectory();
                }
            }
        }

        public void Delete([Optional, DefaultParameterValue(0)] DeleteMode deleteMode)
        {
            this.ThrowIfRelative();
            if (this.FileExists(""))
            {
                File.Delete(this.ToString());
            }
            else
            {
                if (!this.DirectoryExists(""))
                {
                    throw new InvalidOperationException("Trying to delete a path that does not exist: " + this.ToString());
                }
                try
                {
                    Directory.Delete(this.ToString(), true);
                }
                catch (IOException)
                {
                    if (deleteMode == DeleteMode.Normal)
                    {
                        throw;
                    }
                }
            }
        }

        public IEnumerable<NPath> Directories([Optional, DefaultParameterValue(false)] bool recurse)
        {
            return this.Directories("*", recurse);
        }

        public IEnumerable<NPath> Directories(string filter, [Optional, DefaultParameterValue(false)] bool recurse)
        {
            if (<>f__am$cache7 == null)
            {
                <>f__am$cache7 = new Func<string, NPath>(null, (IntPtr) <Directories>m__7);
            }
            return Enumerable.Select<string, NPath>(Directory.GetDirectories(this.ToString(), filter, !recurse ? SearchOption.TopDirectoryOnly : SearchOption.AllDirectories), <>f__am$cache7);
        }

        public bool DirectoryExists(NPath append)
        {
            NPath[] pathArray1 = new NPath[] { append };
            return Directory.Exists(this.Combine(pathArray1).ToString());
        }

        public bool DirectoryExists([Optional, DefaultParameterValue("")] string append)
        {
            return this.DirectoryExists(new NPath(append));
        }

        public NPath EnsureDirectoryExists(NPath append)
        {
            NPath[] pathArray1 = new NPath[] { append };
            NPath path = this.Combine(pathArray1);
            if (!path.DirectoryExists(""))
            {
                path.EnsureParentDirectoryExists();
                path.CreateDirectory();
            }
            return path;
        }

        public NPath EnsureDirectoryExists([Optional, DefaultParameterValue("")] string append)
        {
            return this.EnsureDirectoryExists(new NPath(append));
        }

        public NPath EnsureParentDirectoryExists()
        {
            NPath parent = this.Parent;
            parent.EnsureDirectoryExists("");
            return parent;
        }

        public bool Equals(NPath p)
        {
            if (p._isRelative != this._isRelative)
            {
                return false;
            }
            if (!string.Equals(p._driveLetter, this._driveLetter, PathStringComparison))
            {
                return false;
            }
            if (p._elements.Length != this._elements.Length)
            {
                return false;
            }
            for (int i = 0; i != this._elements.Length; i++)
            {
                if (!string.Equals(p._elements[i], this._elements[i], PathStringComparison))
                {
                    return false;
                }
            }
            return true;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            NPath p = obj as NPath;
            if (p == null)
            {
                return false;
            }
            return this.Equals(p);
        }

        public bool Exists(NPath append)
        {
            return (this.FileExists(append) || this.DirectoryExists(append));
        }

        public bool Exists([Optional, DefaultParameterValue("")] string append)
        {
            return this.Exists(new NPath(append));
        }

        public bool FileExists(NPath append)
        {
            NPath[] pathArray1 = new NPath[] { append };
            return File.Exists(this.Combine(pathArray1).ToString());
        }

        public bool FileExists([Optional, DefaultParameterValue("")] string append)
        {
            return this.FileExists(new NPath(append));
        }

        public IEnumerable<NPath> Files([Optional, DefaultParameterValue(false)] bool recurse)
        {
            return this.Files("*", recurse);
        }

        public IEnumerable<NPath> Files(string filter, [Optional, DefaultParameterValue(false)] bool recurse)
        {
            if (<>f__am$cache6 == null)
            {
                <>f__am$cache6 = new Func<string, NPath>(null, (IntPtr) <Files>m__6);
            }
            return Enumerable.Select<string, NPath>(Directory.GetFiles(this.ToString(), filter, !recurse ? SearchOption.TopDirectoryOnly : SearchOption.AllDirectories), <>f__am$cache6);
        }

        public NPath FirstParentMatching(Func<NPath, bool> predicate)
        {
            this.ThrowIfRelative();
            NPath parent = this;
            while (!predicate.Invoke(parent))
            {
                if (parent.IsEmpty())
                {
                    return null;
                }
                parent = parent.Parent;
            }
            return parent;
        }

        public override int GetHashCode()
        {
            int num = 0x11;
            num = (num * 0x17) + this._isRelative.GetHashCode();
            foreach (string str in this._elements)
            {
                num = (num * 0x17) + str.GetHashCode();
            }
            if (this._driveLetter != null)
            {
                num = (num * 0x17) + this._driveLetter.GetHashCode();
            }
            return num;
        }

        public bool HasExtension(params string[] extensions)
        {
            <HasExtension>c__AnonStorey0 storey = new <HasExtension>c__AnonStorey0 {
                extensionWithDotLower = this.ExtensionWithDot.ToLower()
            };
            return Enumerable.Any<string>(extensions, new Func<string, bool>(storey, (IntPtr) this.<>m__0));
        }

        private static bool HasNonDotDotLastElement(List<string> stack)
        {
            return ((stack.Count > 0) && (stack[stack.Count - 1] != ".."));
        }

        public string InQuotes()
        {
            return ("\"" + this.ToString() + "\"");
        }

        public string InQuotes(SlashMode slashMode)
        {
            return ("\"" + this.ToString(slashMode) + "\"");
        }

        public bool IsChildOf(NPath potentialBasePath)
        {
            if ((this.IsRelative && !potentialBasePath.IsRelative) || (!this.IsRelative && potentialBasePath.IsRelative))
            {
                throw new ArgumentException("You can only call IsChildOf with two relative paths, or with two absolute paths");
            }
            if (this.IsEmpty())
            {
                return false;
            }
            return (this.Equals(potentialBasePath) || this.Parent.IsChildOf(potentialBasePath));
        }

        public bool IsChildOf(string potentialBasePath)
        {
            return this.IsChildOf(new NPath(potentialBasePath));
        }

        private bool IsEmpty()
        {
            return (this._elements.Length == 0);
        }

        private static bool IsLinux()
        {
            return Directory.Exists("/proc");
        }

        private static bool IsRelativeFromSplitString(string[] split)
        {
            if (split.Length < 2)
            {
                return true;
            }
            if (split[0].Length == 0)
            {
            }
            return ((<>f__am$cache2 != null) || !Enumerable.Any<string>(split, <>f__am$cache2));
        }

        public NPath MakeAbsolute()
        {
            if (!this.IsRelative)
            {
                return this;
            }
            NPath[] append = new NPath[] { this };
            return CurrentDirectory.Combine(append);
        }

        public NPath MakeDirectoryEmpty()
        {
            this.ThrowIfRelative();
            if (this.FileExists(""))
            {
                throw new InvalidOperationException("It is not valid to perform this operation on a file");
            }
            if (this.DirectoryExists(""))
            {
                try
                {
                    this.Delete(DeleteMode.Normal);
                }
                catch (IOException)
                {
                    if (Enumerable.Any<NPath>(this.Files(true)))
                    {
                        throw;
                    }
                }
            }
            return this.EnsureDirectoryExists("");
        }

        public NPath Move(NPath dest)
        {
            this.ThrowIfRelative();
            if (dest.IsRelative)
            {
                NPath[] append = new NPath[] { dest };
                return this.Move(this.Parent.Combine(append));
            }
            if (dest.DirectoryExists(""))
            {
                string[] textArray1 = new string[] { this.FileName };
                return this.Move(dest.Combine(textArray1));
            }
            if (this.FileExists(""))
            {
                dest.EnsureParentDirectoryExists();
                File.Move(this.ToString(), dest.ToString());
                return dest;
            }
            if (!this.DirectoryExists(""))
            {
                throw new ArgumentException("Move() called on a path that doesn't exist: " + this.ToString());
            }
            Directory.Move(this.ToString(), dest.ToString());
            return dest;
        }

        public NPath Move(string dest)
        {
            return this.Move(new NPath(dest));
        }

        public static bool operator ==(NPath a, NPath b)
        {
            if (object.ReferenceEquals(a, b))
            {
                return true;
            }
            if ((a == null) || (b == null))
            {
                return false;
            }
            return a.Equals(b);
        }

        public static bool operator !=(NPath a, NPath b)
        {
            return !(a == b);
        }

        public NPath ParentContaining(NPath needle)
        {
            this.ThrowIfRelative();
            NPath parent = this;
            while (!parent.Exists(needle))
            {
                if (parent.IsEmpty())
                {
                    return null;
                }
                parent = parent.Parent;
            }
            return parent;
        }

        public NPath ParentContaining(string needle)
        {
            return this.ParentContaining(new NPath(needle));
        }

        private string ParseDriveLetter(string path, out string driveLetter)
        {
            if ((path.Length >= 2) && (path[1] == ':'))
            {
                driveLetter = path[0].ToString();
                return path.Substring(2);
            }
            driveLetter = null;
            return path;
        }

        private string[] ParseSplitStringIntoElements(IEnumerable<string> inputs)
        {
            List<string> stack = new List<string>();
            if (<>f__am$cache1 == null)
            {
                <>f__am$cache1 = new Func<string, bool>(null, (IntPtr) <ParseSplitStringIntoElements>m__1);
            }
            foreach (string str in Enumerable.Where<string>(inputs, <>f__am$cache1))
            {
                if (str == "..")
                {
                    if (HasNonDotDotLastElement(stack))
                    {
                        stack.RemoveAt(stack.Count - 1);
                        continue;
                    }
                    if (!this._isRelative)
                    {
                        throw new ArgumentException("You cannot create a path that tries to .. past the root");
                    }
                }
                stack.Add(str);
            }
            return stack.ToArray();
        }

        public string[] ReadAllLines()
        {
            this.ThrowIfRelative();
            return File.ReadAllLines(this.ToString());
        }

        public string ReadAllText()
        {
            this.ThrowIfRelative();
            return File.ReadAllText(this.ToString());
        }

        public NPath RelativeTo(NPath path)
        {
            if (!this.IsChildOf(path))
            {
                object[] objArray1 = new object[] { "Path.RelativeTo() was invoked with two paths that are unrelated. invoked on: ", this.ToString(), " asked to be made relative to: ", path };
                throw new ArgumentException(string.Concat(objArray1));
            }
            return new NPath(Enumerable.ToArray<string>(Enumerable.Skip<string>(this._elements, path._elements.Length)), true, null);
        }

        private static char Slash(SlashMode slashMode)
        {
            if (slashMode != SlashMode.Backward)
            {
                if (slashMode == SlashMode.Forward)
                {
                    return '/';
                }
            }
            else
            {
                return '\\';
            }
            return Path.DirectorySeparatorChar;
        }

        private void ThrowIfRelative()
        {
            if (this._isRelative)
            {
                throw new ArgumentException("You are attempting an operation on a Path that requires an absolute path, but the path is relative");
            }
        }

        public override string ToString()
        {
            return this.ToString(SlashMode.Native);
        }

        public string ToString(SlashMode slashMode)
        {
            if (this._isRelative && (this._elements.Length == 0))
            {
                return ".";
            }
            StringBuilder builder = new StringBuilder();
            if (this._driveLetter != null)
            {
                builder.Append(this._driveLetter);
                builder.Append(":");
            }
            if (!this._isRelative)
            {
                builder.Append(Slash(slashMode));
            }
            bool flag = true;
            foreach (string str2 in this._elements)
            {
                if (!flag)
                {
                    builder.Append(Slash(slashMode));
                }
                builder.Append(str2);
                flag = false;
            }
            return builder.ToString();
        }

        private static string WithDot(string extension)
        {
            return (!extension.StartsWith(".") ? ("." + extension) : extension);
        }

        public NPath WriteAllLines(string[] contents)
        {
            this.ThrowIfRelative();
            this.EnsureParentDirectoryExists();
            File.WriteAllLines(this.ToString(), contents);
            return this;
        }

        public NPath WriteAllText(string contents)
        {
            this.ThrowIfRelative();
            this.EnsureParentDirectoryExists();
            File.WriteAllText(this.ToString(), contents);
            return this;
        }

        public static NPath CurrentDirectory
        {
            get
            {
                return new NPath(Directory.GetCurrentDirectory());
            }
        }

        public IEnumerable<string> Elements
        {
            get
            {
                return this._elements;
            }
        }

        public string ExtensionWithDot
        {
            get
            {
                string str = Enumerable.Last<string>(this._elements);
                int startIndex = str.LastIndexOf(".");
                if (startIndex < 0)
                {
                    return string.Empty;
                }
                return str.Substring(startIndex);
            }
        }

        public string FileName
        {
            get
            {
                return Enumerable.Last<string>(this._elements);
            }
        }

        public string FileNameWithoutExtension
        {
            get
            {
                return Path.GetFileNameWithoutExtension(this.FileName);
            }
        }

        public static NPath HomeDirectory
        {
            get
            {
                if (Path.DirectorySeparatorChar == '\\')
                {
                    return new NPath(Environment.GetEnvironmentVariable("USERPROFILE"));
                }
                return new NPath(EnvironmentPortable.GetPersonalFolderPortable());
            }
        }

        public bool IsRelative
        {
            get
            {
                return this._isRelative;
            }
        }

        public NPath Parent
        {
            get
            {
                if (this._elements.Length == 0)
                {
                    throw new InvalidOperationException("Parent is called on an empty path");
                }
                return new NPath(Enumerable.ToArray<string>(Enumerable.Take<string>(this._elements, this._elements.Length - 1)), this._isRelative, this._driveLetter);
            }
        }

        public static NPath SystemTemp
        {
            get
            {
                return new NPath(Path.GetTempPath());
            }
        }

        [CompilerGenerated]
        private sealed class <CopyFiles>c__AnonStorey1
        {
            internal NPath $this;
            internal NPath destination;

            internal NPath <>m__0(NPath file)
            {
                NPath[] append = new NPath[] { file.RelativeTo(this.$this) };
                return file.Copy(this.destination.Combine(append));
            }
        }

        [CompilerGenerated]
        private sealed class <HasExtension>c__AnonStorey0
        {
            internal string extensionWithDotLower;

            internal bool <>m__0(string e)
            {
                return (NPath.WithDot(e).ToLower() == this.extensionWithDotLower);
            }
        }
    }
}

