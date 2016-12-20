namespace Unity.PackageManager.Ivy
{
    using System;

    public class IvyDependency
    {
        public string Branch;
        public bool Force;
        public string Name;
        public string Organisation;
        public string Revision;
        public string RevisionConstraint;

        public IvyDependency Clone()
        {
            return Cloner.CloneObject<IvyDependency>(this);
        }
    }
}

