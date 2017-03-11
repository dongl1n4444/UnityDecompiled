namespace UnityScript.TypeSystem
{
    using Boo.Lang.Compiler.TypeSystem;
    using Boo.Lang.Compiler.TypeSystem.Services;
    using System;

    [Serializable]
    public class UnityDowncastPermissions : DowncastPermissions
    {
        private bool $Enabled$42;

        private bool CanBeReachedByArrayDowncast(IArrayType expectedType, IArrayType actualType)
        {
            bool flag1 = expectedType.Rank == actualType.Rank;
            if (!flag1)
            {
                return flag1;
            }
            return this.CanBeReachedByDowncast(expectedType.ElementType, actualType.ElementType);
        }

        public override bool CanBeReachedByDowncast(IType expectedType, IType actualType) => 
            (((!expectedType.IsArray || !actualType.IsArray) || !this.IsDowncastAllowed()) ? base.CanBeReachedByDowncast(expectedType, actualType) : this.CanBeReachedByArrayDowncast((IArrayType) expectedType, (IArrayType) actualType));

        public override bool IsDowncastAllowed()
        {
            bool enabled = this.Enabled;
            if (enabled)
            {
                return enabled;
            }
            return base.IsDowncastAllowed();
        }

        public bool Enabled
        {
            get => 
                this.$Enabled$42;
            set
            {
                this.$Enabled$42 = value;
            }
        }
    }
}

