namespace UnityScript.Scripting.Pipeline
{
    using Boo.Lang.Compiler.TypeSystem;
    using System;

    [Serializable]
    public class EvaluationContextEntity : ITypedEntity
    {
        protected IMember _delegate;

        public EvaluationContextEntity(IMember @delegate)
        {
            this._delegate = @delegate;
        }

        public IMember Delegate
        {
            get
            {
                return this._delegate;
            }
        }

        public override Boo.Lang.Compiler.TypeSystem.EntityType EntityType
        {
            get
            {
                return this._delegate.get_EntityType();
            }
        }

        public override string FullName
        {
            get
            {
                return this._delegate.get_FullName();
            }
        }

        public override string Name
        {
            get
            {
                return this._delegate.get_Name();
            }
        }

        public override IType Type
        {
            get
            {
                return this._delegate.get_Type();
            }
        }
    }
}

