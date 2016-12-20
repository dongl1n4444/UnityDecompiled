namespace NiceIO
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [Extension]
    public static class Extensions
    {
        [Extension]
        public static IEnumerable<NPath> Copy(IEnumerable<NPath> self, NPath dest)
        {
            <Copy>c__AnonStorey0 storey = new <Copy>c__AnonStorey0 {
                dest = dest
            };
            if (storey.dest.IsRelative)
            {
                throw new ArgumentException("When copying multiple files, the destination cannot be a relative path");
            }
            storey.dest.EnsureDirectoryExists("");
            return Enumerable.ToArray<NPath>(Enumerable.Select<NPath, NPath>(self, new Func<NPath, NPath>(storey, (IntPtr) this.<>m__0)));
        }

        [Extension]
        public static IEnumerable<NPath> Copy(IEnumerable<NPath> self, string dest)
        {
            return Copy(self, new NPath(dest));
        }

        [Extension]
        public static IEnumerable<NPath> Delete(IEnumerable<NPath> self)
        {
            foreach (NPath path in self)
            {
                path.Delete(DeleteMode.Normal);
            }
            return self;
        }

        [Extension]
        public static IEnumerable<string> InQuotes(IEnumerable<NPath> self, [Optional, DefaultParameterValue(0)] SlashMode forward)
        {
            <InQuotes>c__AnonStorey2 storey = new <InQuotes>c__AnonStorey2 {
                forward = forward
            };
            return Enumerable.Select<NPath, string>(self, new Func<NPath, string>(storey, (IntPtr) this.<>m__0));
        }

        [Extension]
        public static IEnumerable<NPath> Move(IEnumerable<NPath> self, NPath dest)
        {
            <Move>c__AnonStorey1 storey = new <Move>c__AnonStorey1 {
                dest = dest
            };
            if (storey.dest.IsRelative)
            {
                throw new ArgumentException("When moving multiple files, the destination cannot be a relative path");
            }
            storey.dest.EnsureDirectoryExists("");
            return Enumerable.ToArray<NPath>(Enumerable.Select<NPath, NPath>(self, new Func<NPath, NPath>(storey, (IntPtr) this.<>m__0)));
        }

        [Extension]
        public static IEnumerable<NPath> Move(IEnumerable<NPath> self, string dest)
        {
            return Move(self, new NPath(dest));
        }

        [Extension]
        public static NPath ToNPath(string path)
        {
            return new NPath(path);
        }

        [CompilerGenerated]
        private sealed class <Copy>c__AnonStorey0
        {
            internal NPath dest;

            internal NPath <>m__0(NPath p)
            {
                string[] append = new string[] { p.FileName };
                return p.Copy(this.dest.Combine(append));
            }
        }

        [CompilerGenerated]
        private sealed class <InQuotes>c__AnonStorey2
        {
            internal SlashMode forward;

            internal string <>m__0(NPath p)
            {
                return p.InQuotes(this.forward);
            }
        }

        [CompilerGenerated]
        private sealed class <Move>c__AnonStorey1
        {
            internal NPath dest;

            internal NPath <>m__0(NPath p)
            {
                string[] append = new string[] { p.FileName };
                return p.Move(this.dest.Combine(append));
            }
        }
    }
}

