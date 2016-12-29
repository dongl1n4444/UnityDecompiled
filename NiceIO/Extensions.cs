namespace NiceIO
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public static class Extensions
    {
        public static IEnumerable<NPath> Copy(this IEnumerable<NPath> self, NPath dest)
        {
            <Copy>c__AnonStorey0 storey = new <Copy>c__AnonStorey0 {
                dest = dest
            };
            if (storey.dest.IsRelative)
            {
                throw new ArgumentException("When copying multiple files, the destination cannot be a relative path");
            }
            storey.dest.EnsureDirectoryExists("");
            return self.Select<NPath, NPath>(new Func<NPath, NPath>(storey, (IntPtr) this.<>m__0)).ToArray<NPath>();
        }

        public static IEnumerable<NPath> Copy(this IEnumerable<NPath> self, string dest) => 
            self.Copy(new NPath(dest));

        public static IEnumerable<NPath> Delete(this IEnumerable<NPath> self)
        {
            foreach (NPath path in self)
            {
                path.Delete(DeleteMode.Normal);
            }
            return self;
        }

        public static IEnumerable<string> InQuotes(this IEnumerable<NPath> self, SlashMode forward = 0)
        {
            <InQuotes>c__AnonStorey2 storey = new <InQuotes>c__AnonStorey2 {
                forward = forward
            };
            return self.Select<NPath, string>(new Func<NPath, string>(storey, (IntPtr) this.<>m__0));
        }

        public static IEnumerable<NPath> Move(this IEnumerable<NPath> self, NPath dest)
        {
            <Move>c__AnonStorey1 storey = new <Move>c__AnonStorey1 {
                dest = dest
            };
            if (storey.dest.IsRelative)
            {
                throw new ArgumentException("When moving multiple files, the destination cannot be a relative path");
            }
            storey.dest.EnsureDirectoryExists("");
            return self.Select<NPath, NPath>(new Func<NPath, NPath>(storey, (IntPtr) this.<>m__0)).ToArray<NPath>();
        }

        public static IEnumerable<NPath> Move(this IEnumerable<NPath> self, string dest) => 
            self.Move(new NPath(dest));

        public static NPath ToNPath(this string path) => 
            new NPath(path);

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

            internal string <>m__0(NPath p) => 
                p.InQuotes(this.forward);
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

