namespace Unity.IL2CPP
{
    using Mono.Cecil;
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct StackInfo
    {
        public readonly string Expression;
        public readonly TypeReference Type;
        public StackInfo(string expression, TypeReference type)
        {
            this.Expression = expression;
            this.Type = type;
        }

        public StackInfo(Local local)
        {
            this.Expression = local.Expression;
            this.Type = local.Type;
        }

        public override string ToString()
        {
            return this.Expression;
        }
    }
}

