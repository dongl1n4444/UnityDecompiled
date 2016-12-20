namespace UnityEditor.iOS.Xcode.PBX
{
    using System;
    using System.Text;

    internal abstract class SectionBase
    {
        protected SectionBase()
        {
        }

        public abstract void AddObject(string key, PBXElementDict value);
        public abstract void WriteSection(StringBuilder sb, GUIDToCommentMap comments);
    }
}

