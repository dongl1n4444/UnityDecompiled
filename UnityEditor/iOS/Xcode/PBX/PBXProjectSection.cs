namespace UnityEditor.iOS.Xcode.PBX
{
    using System;
    using System.Collections.Generic;

    internal class PBXProjectSection : KnownSectionBase<PBXProjectObjectData>
    {
        public PBXProjectSection() : base("PBXProject")
        {
        }

        public PBXProjectObjectData project
        {
            get
            {
                foreach (KeyValuePair<string, PBXProjectObjectData> pair in base.GetEntries())
                {
                    return pair.Value;
                }
                return null;
            }
        }
    }
}

