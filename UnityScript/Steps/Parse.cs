namespace UnityScript.Steps
{
    using Boo.Lang.Compiler;
    using Boo.Lang.Compiler.Ast;
    using Boo.Lang.Compiler.Steps;
    using System;
    using System.IO;
    using UnityScript.Core;
    using UnityScript.Parser;

    [Serializable]
    public class Parse : AbstractCompilerStep
    {
        private void NormalizePropertyAccessors()
        {
            PropertyAccessorNormalizer normalizer;
            PropertyAccessorNormalizer normalizer1 = normalizer = new PropertyAccessorNormalizer();
            CompilerErrorCollection collection1 = normalizer.Errors = this.get_Errors();
            this.get_CompileUnit().Accept(normalizer);
        }

        private void ParseInput(ICompilerInput input)
        {
            TextReader reader;
            IDisposable disposable = (reader = input.Open()) as IDisposable;
            try
            {
                UnityScriptParser.ParseReader(reader, input.get_Name(), this.get_Context(), this.get_CompileUnit());
            }
            finally
            {
                if (disposable != null)
                {
                    disposable.Dispose();
                    disposable = null;
                }
            }
        }

        private void ParseInputs()
        {
            foreach (ICompilerInput input in this.get_Parameters().get_Input())
            {
                try
                {
                    this.ParseInput(input);
                }
                catch (Exception exception)
                {
                    this.get_Errors().Add(CompilerErrorFactory.InputError(input.get_Name(), exception));
                }
            }
        }

        public override void Run()
        {
            this.ParseInputs();
            this.NormalizePropertyAccessors();
        }

        [Serializable]
        public class PropertyAccessorNormalizer : FastDepthFirstVisitor
        {
            private CompilerErrorCollection $Errors$35;

            public void CheckSetterReturnType(Method setter)
            {
                if (setter.get_ReturnType() != null)
                {
                    this.ReportError(UnityScriptCompilerErrors.SetterCanNotDeclareReturnType(setter.get_ReturnType().get_LexicalInfo()));
                }
            }

            public void NormalizeGetter(Method getter)
            {
                if ((getter != null) && (getter.get_Parameters().get_Count() != 0))
                {
                    this.ReportError(UnityScriptCompilerErrors.InvalidPropertyGetter(getter.get_LexicalInfo()));
                }
            }

            public void NormalizeSetter(Method setter)
            {
                if (setter != null)
                {
                    this.NormalizeSetterParameters(setter);
                    this.CheckSetterReturnType(setter);
                }
            }

            public void NormalizeSetterParameters(Method setter)
            {
                if ((setter.get_Parameters().get_Count() != 1) || (setter.get_Parameters().get_Item(0).get_Name() != "value"))
                {
                    this.ReportError(UnityScriptCompilerErrors.InvalidPropertySetter(setter.get_LexicalInfo()));
                }
                else
                {
                    setter.get_Parameters().Clear();
                }
            }

            public override void OnProperty(Property node)
            {
                this.NormalizeSetter(node.get_Setter());
                this.NormalizeGetter(node.get_Getter());
            }

            public void ReportError(CompilerError error)
            {
                this.Errors.Add(error);
            }

            public CompilerErrorCollection Errors
            {
                get => 
                    this.$Errors$35;
                set
                {
                    this.$Errors$35 = value;
                }
            }
        }
    }
}

