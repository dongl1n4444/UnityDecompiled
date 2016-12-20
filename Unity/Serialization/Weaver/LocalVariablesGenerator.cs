namespace Unity.Serialization.Weaver
{
    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class LocalVariablesGenerator
    {
        private readonly MethodDefinition _methodDefinition;
        private readonly ILProcessor _processor;
        private readonly List<LocalVariable> _variables = new List<LocalVariable>();

        public LocalVariablesGenerator(MethodDefinition methodDefinition, ILProcessor processor)
        {
            this._processor = processor;
            this._methodDefinition = methodDefinition;
        }

        public LocalVariable Create(TypeReference type)
        {
            this._methodDefinition.Body.InitLocals = true;
            LocalVariable item = new LocalVariable(this._variables.Count, type, this.LoadGeneratorForNextVariable(), this.StoreGeneratorForNextVariable());
            this._variables.Add(item);
            this._methodDefinition.Body.Variables.Add(new VariableDefinition(type));
            return item;
        }

        private Action LoadGeneratorForNextVariable()
        {
            <LoadGeneratorForNextVariable>c__AnonStorey0 storey = new <LoadGeneratorForNextVariable>c__AnonStorey0 {
                $this = this,
                index = this._variables.Count
            };
            if (storey.index < 4)
            {
                return new Action(storey, (IntPtr) this.<>m__0);
            }
            return new Action(storey, (IntPtr) this.<>m__1);
        }

        private static OpCode LoadOpCodeForIndex(int index)
        {
            switch (index)
            {
                case 0:
                    return OpCodes.Ldloc_0;

                case 1:
                    return OpCodes.Ldloc_1;

                case 2:
                    return OpCodes.Ldloc_2;

                case 3:
                    return OpCodes.Ldloc_3;
            }
            return OpCodes.Ldloc;
        }

        private Action StoreGeneratorForNextVariable()
        {
            <StoreGeneratorForNextVariable>c__AnonStorey1 storey = new <StoreGeneratorForNextVariable>c__AnonStorey1 {
                $this = this,
                index = this._variables.Count
            };
            if (storey.index < 4)
            {
                return new Action(storey, (IntPtr) this.<>m__0);
            }
            return new Action(storey, (IntPtr) this.<>m__1);
        }

        private static OpCode StoreOpCodeForIndex(int index)
        {
            switch (index)
            {
                case 0:
                    return OpCodes.Stloc_0;

                case 1:
                    return OpCodes.Stloc_1;

                case 2:
                    return OpCodes.Stloc_2;

                case 3:
                    return OpCodes.Stloc_3;
            }
            return OpCodes.Stloc;
        }

        [CompilerGenerated]
        private sealed class <LoadGeneratorForNextVariable>c__AnonStorey0
        {
            internal LocalVariablesGenerator $this;
            internal int index;

            internal void <>m__0()
            {
                this.$this._processor.Emit(LocalVariablesGenerator.LoadOpCodeForIndex(this.index));
            }

            internal void <>m__1()
            {
                this.$this._processor.Emit(LocalVariablesGenerator.LoadOpCodeForIndex(this.index), this.index);
            }
        }

        [CompilerGenerated]
        private sealed class <StoreGeneratorForNextVariable>c__AnonStorey1
        {
            internal LocalVariablesGenerator $this;
            internal int index;

            internal void <>m__0()
            {
                this.$this._processor.Emit(LocalVariablesGenerator.StoreOpCodeForIndex(this.index));
            }

            internal void <>m__1()
            {
                this.$this._processor.Emit(LocalVariablesGenerator.StoreOpCodeForIndex(this.index), this.index);
            }
        }
    }
}

