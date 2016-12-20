namespace UnityScript.Macros
{
    using Boo.Lang.Compiler;
    using Boo.Lang.Compiler.Ast;
    using Boo.Lang.PatternMatching;
    using CompilerGenerated;
    using System;
    using System.Runtime.CompilerServices;
    using System.Text;

    [Serializable]
    public sealed class SwitchMacro : LexicalInfoPreservingMacro
    {
        [CompilerGenerated]
        private MacroStatement __macro;

        internal string $ExpandImpl$UniqueName$201()
        {
            string[] textArray1 = new string[] { "switch" };
            return this.get_Context().GetUniqueName(textArray1);
        }

        public SwitchMacro()
        {
        }

        public SwitchMacro(CompilerContext context) : base(context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
        }

        protected override Statement ExpandImpl(MacroStatement @switch)
        {
            Block block;
            BinaryExpression expression2;
            $ExpandImpl$locals$272 s$ = new $ExpandImpl$locals$272();
            if (@switch == null)
            {
                throw new ArgumentNullException("switch");
            }
            this.__macro = @switch;
            if (@switch.get_Arguments().Count != 1)
            {
                this.get_Errors().Add(CompilerErrorFactory.CustomError(@switch, "switch requires an expression."));
                return block;
            }
            StatementCollection statements = @switch.get_Body().get_Statements();
            if (statements.Count == 0)
            {
                this.get_Warnings().Add(CompilerWarningFactory.CustomWarning(@switch, "switch statement has no cases."));
                return block;
            }
            s$.$UniqueName = new __SwitchMacro_ExpandImpl$callable1$31_9__(this.$ExpandImpl$UniqueName$201);
            __SwitchMacro_ExpandImpl$callable2$34_9__ $callable$___ = new __SwitchMacro_ExpandImpl$callable2$34_9__(new $ExpandImpl$NewLabel$202(s$).Invoke);
            ReferenceExpression local = new ReferenceExpression(s$.$UniqueName());
            LabelStatement label = $callable$___();
            block = new Block();
            BinaryExpression expression1 = expression2 = new BinaryExpression(LexicalInfo.Empty);
            expression2.set_Operator(15);
            expression2.set_Left(Expression.Lift(local));
            expression2.set_Right(Expression.Lift(@switch.get_Arguments().get_Item(0)));
            block.Add(expression2);
            LabelStatement statement2 = null;
            Statement statement3 = statements.get_Item(-1);
            foreach (Statement statement4 in statements)
            {
                Block block2;
                Statement statement5 = statement4;
                if (statement5 is CaseStatement)
                {
                    CaseStatement statement6;
                    CaseStatement statement1 = statement6 = statement5;
                    if (1 != 0)
                    {
                        ExpressionCollection expressions;
                        ExpressionCollection collection1 = expressions = statement6.Expressions;
                        if (1 != 0)
                        {
                            Block block1 = block2 = statement6.Body;
                            if (1 != 0)
                            {
                                IfStatement statement7;
                                Expression expression3 = SwitchMacroModule.ComparisonFor(local, expressions);
                                IfStatement statement10 = statement7 = new IfStatement(LexicalInfo.Empty);
                                statement7.set_Condition(Expression.Lift(expression3));
                                statement7.set_TrueBlock(new Block(LexicalInfo.Empty));
                                IfStatement statement8 = statement7;
                                if (statement2 != null)
                                {
                                    statement8.get_TrueBlock().Add(statement2);
                                    statement2 = null;
                                }
                                statement8.get_TrueBlock().Add(block2);
                                if ((statement4 != statement3) && !SwitchMacroModule.EndsWithBreak(block2))
                                {
                                    statement2 = $callable$___();
                                    statement8.get_TrueBlock().Add(SwitchMacroModule.NewGoto(statement2));
                                }
                                block.Add(statement8);
                                continue;
                            }
                        }
                    }
                }
                if (statement5 is DefaultStatement)
                {
                    DefaultStatement statement9;
                    DefaultStatement statement11 = statement9 = statement5;
                    if (1 != 0)
                    {
                        Block block4 = block2 = statement9.Body;
                        if (1 != 0)
                        {
                            if (statement2 != null)
                            {
                                block.Add(statement2);
                            }
                            block.Add(block2);
                            continue;
                        }
                    }
                }
                throw new MatchError(new StringBuilder("`item` failed to match `").Append(statement5).Append("`").ToString());
            }
            block.Accept(new GotoOnTopLevelBreak(label));
            block.Add(label);
            return block;
        }

        [Serializable]
        internal class $ExpandImpl$locals$272
        {
            internal __SwitchMacro_ExpandImpl$callable1$31_9__ $UniqueName;
        }

        [Serializable]
        internal class $ExpandImpl$NewLabel$202
        {
            internal SwitchMacro.$ExpandImpl$locals$272 $$locals$275;

            public $ExpandImpl$NewLabel$202(SwitchMacro.$ExpandImpl$locals$272 $$locals$275)
            {
                this.$$locals$275 = $$locals$275;
            }

            public LabelStatement Invoke()
            {
                LabelStatement statement;
                LabelStatement statement1 = statement = new LabelStatement();
                statement.set_Name(this.$$locals$275.$UniqueName());
                return statement;
            }
        }

        [Serializable]
        public sealed class CaseMacro : LexicalInfoPreservingMacro
        {
            [CompilerGenerated]
            private MacroStatement $switch;
            [CompilerGenerated]
            private MacroStatement __macro;

            public CaseMacro()
            {
            }

            public CaseMacro(CompilerContext context) : base(context)
            {
                if (context == null)
                {
                    throw new ArgumentNullException("context");
                }
            }

            protected override Statement ExpandImpl(MacroStatement @case)
            {
                CaseStatement statement;
                if (@case == null)
                {
                    throw new ArgumentNullException("case");
                }
                this.__macro = @case;
                CaseStatement statement1 = statement = new CaseStatement();
                ExpressionCollection collection1 = statement.Expressions = @case.get_Arguments();
                Block block1 = statement.Body = @case.get_Body();
                return statement;
            }

            [CompilerGenerated]
            private MacroStatement @switch
            {
                get
                {
                    if (this.$switch <= null)
                    {
                        this.$switch = this.__macro.GetParentMacroByName("switch");
                    }
                    return this.$switch;
                }
            }
        }

        [Serializable]
        public sealed class DefaultMacro : LexicalInfoPreservingMacro
        {
            [CompilerGenerated]
            private MacroStatement $switch;
            [CompilerGenerated]
            private MacroStatement __macro;

            public DefaultMacro()
            {
            }

            public DefaultMacro(CompilerContext context) : base(context)
            {
                if (context == null)
                {
                    throw new ArgumentNullException("context");
                }
            }

            protected override Statement ExpandImpl(MacroStatement @default)
            {
                DefaultStatement statement;
                if (@default == null)
                {
                    throw new ArgumentNullException("default");
                }
                this.__macro = @default;
                if (this.__macro.get_Arguments().get_Count() != 0)
                {
                    throw new Exception("`default` macro invocation argument(s) did not match definition: `default()`");
                }
                DefaultStatement statement1 = statement = new DefaultStatement();
                Block block1 = statement.Body = @default.get_Body();
                return statement;
            }

            [CompilerGenerated]
            private MacroStatement @switch
            {
                get
                {
                    if (this.$switch <= null)
                    {
                        this.$switch = this.__macro.GetParentMacroByName("switch");
                    }
                    return this.$switch;
                }
            }
        }
    }
}

