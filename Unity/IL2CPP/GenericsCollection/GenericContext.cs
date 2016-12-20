namespace Unity.IL2CPP.GenericsCollection
{
    using Mono.Cecil;
    using System;

    public class GenericContext
    {
        private readonly GenericInstanceMethod _method;
        private readonly GenericInstanceType _type;

        public GenericContext(GenericInstanceType type, GenericInstanceMethod method)
        {
            this._type = type;
            this._method = method;
        }

        public GenericInstanceMethod Method
        {
            get
            {
                return this._method;
            }
        }

        public GenericInstanceType Type
        {
            get
            {
                return this._type;
            }
        }
    }
}

