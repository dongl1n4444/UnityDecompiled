namespace Unity.PackageManager.Ivy
{
    using System.Collections.Generic;

    public class IvyDependencies : List<IvyDependency>
    {
        public IvyDependencies Clone() => 
            Cloner.CloneObject<IvyDependencies>(this);
    }
}

