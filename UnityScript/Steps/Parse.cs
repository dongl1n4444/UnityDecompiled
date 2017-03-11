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
            CompilerErrorCollection collection1 = normalizer.Errors = this.Errors;
            this.CompileUnit.Accept(normalizer);
        }

        private void ParseInput(ICompilerInput input)
        {
            TextReader reader;
            IDisposable disposable = (reader = input.Open()) as IDisposable;
            try
            {
                UnityScriptParser.ParseReader(reader, input.Name, this.Context, this.CompileUnit);
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
            foreach (ICompilerInput input in this.Parameters.Input)
            {
                try
                {
                    this.ParseInput(input);
                }
                catch (Exception exception)
                {
                    this.Errors.Add(CompilerErrorFactory.InputError(input.Name, exception));
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
                if (setter.ReturnType != null)
                {
                    this.ReportError(UnityScriptCompilerErrors.SetterCanNotDeclareReturnType(setter.ReturnType.LexicalInfo));
                }
            }

            public void NormalizeGetter(Method getter)
            {
                if ((getter != null) && (getter.Parameters.Count != 0))
                {
                    this.ReportError(UnityScriptCompilerErrors.InvalidPropertyGetter(getter.LexicalInfo));
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
                if ((setter.Parameters.Count != 1) || (setter.Parameters[0].Name != "value"))
                {
                    this.ReportError(UnityScriptCompilerErrors.InvalidPropertySetter(setter.LexicalInfo));
                }
                else
                {
                    setter.Parameters.Clear();
                }
            }

            public override void OnProperty(Property node)
            {
                this.NormalizeSetter(node.Setter);
                this.NormalizeGetter(node.Getter);
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

