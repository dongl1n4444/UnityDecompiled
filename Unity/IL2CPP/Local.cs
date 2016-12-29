namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using System;
    using Unity.IL2CPP.IoC;
    using Unity.IL2CPP.IoCServices;

    public class Local
    {
        private readonly string _expression;
        private readonly TypeReference _type;
        [Inject]
        public static INamingService Naming;

        public Local(TypeReference type, string expression)
        {
            this._type = type;
            this._expression = expression;
        }

        public string Expression =>
            this._expression;

        public string IdentifierExpression =>
            (Naming.ForVariable(this._type) + " " + this._expression);

        public TypeReference Type =>
            this._type;
    }
}

