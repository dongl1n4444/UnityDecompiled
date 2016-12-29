namespace Unity.PackageManager.Ivy
{
    using System;
    using System.Collections.Generic;
    using Unity.DataContract;

    public class IvyInfo
    {
        public string Branch;
        public string Description;
        public string Module;
        public string Organisation;
        public string Publication;
        public bool Published;
        internal List<IvyRepository> repositories;
        public string Revision;
        public string Status;
        public string Title;
        public PackageType Type;
        public PackageVersion UnityVersion;
        public PackageVersion Version;

        public IvyInfo Clone() => 
            Cloner.CloneObject<IvyInfo>(this);

        public string FullName =>
            $"{this.Organisation}.{this.Module}.{this.Version}";

        public List<IvyRepository> Repositories
        {
            get
            {
                if (this.repositories == null)
                {
                    this.repositories = new List<IvyRepository>();
                }
                return this.repositories;
            }
        }
    }
}

