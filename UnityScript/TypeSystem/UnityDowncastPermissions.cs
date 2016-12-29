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
            bool flag1 = expectedType.get_Rank() == actualType.get_Rank();
            if (!flag1)
            {
                return flag1;
            }
            return this.CanBeReachedByDowncast(expectedType.get_ElementType(), actualType.get_ElementType());
        }

        public override bool CanBeReachedByDowncast(IType expectedType, IType actualType) => 
            (((!expectedType.get_IsArray() || !actualType.get_IsArray()) || !this.IsDowncastAllowed()) ? base.CanBeReachedByDowncast(expectedType, actualType) : this.CanBeReachedByArrayDowncast((IArrayType) expectedType, (IArrayType) actualType));

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

