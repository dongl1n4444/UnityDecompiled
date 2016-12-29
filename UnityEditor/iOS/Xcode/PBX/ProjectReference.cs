namespace UnityEditor.iOS.Xcode.PBX
{
    using System;

    internal class ProjectReference
    {
        public string group;
        public string projectRef;

        public static ProjectReference Create(string group, string projectRef) => 
            new ProjectReference { 
                group = group,
                projectRef = projectRef
            };
    }
}

