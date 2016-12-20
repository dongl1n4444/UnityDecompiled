namespace UnityEditor.iOS.Xcode.PBX
{
    using System;
    using System.Collections.Generic;

    internal class FileGUIDListBase : PBXObjectData
    {
        private static PropertyCommentChecker checkerData;
        public GUIDList files;

        static FileGUIDListBase()
        {
            string[] props = new string[] { "files/*" };
            checkerData = new PropertyCommentChecker(props);
        }

        public override void UpdateProps()
        {
            base.SetPropertyList("files", (List<string>) this.files);
        }

        public override void UpdateVars()
        {
            this.files = base.GetPropertyList("files");
        }

        internal override PropertyCommentChecker checker
        {
            get
            {
                return checkerData;
            }
        }
    }
}

