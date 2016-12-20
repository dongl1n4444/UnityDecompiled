namespace Unity.PackageManager.Ivy
{
    using System.Collections.Generic;

    public class IvyArtifacts : List<IvyArtifact>
    {
        public IvyArtifacts Clone()
        {
            return Cloner.CloneObject<IvyArtifacts>(this);
        }
    }
}

