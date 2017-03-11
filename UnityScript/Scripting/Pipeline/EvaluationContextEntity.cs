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

        public IMember Delegate =>
            this._delegate;

        public override Boo.Lang.Compiler.TypeSystem.EntityType EntityType =>
            this._delegate.EntityType;

        public override string FullName =>
            this._delegate.FullName;

        public override string Name =>
            this._delegate.Name;

        public override IType Type =>
            this._delegate.Type;
    }
}

