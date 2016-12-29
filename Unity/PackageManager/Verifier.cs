namespace Unity.PackageManager
{
    using System;

    public class Verifier : Task
    {
        protected override bool TaskRunning()
        {
            if (!base.TaskRunning())
            {
                return false;
            }
            return this.Verify();
        }

        public virtual bool Verify() => 
            true;
    }
}

