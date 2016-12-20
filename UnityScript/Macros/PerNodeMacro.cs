namespace UnityScript.Macros
{
    using Boo.Lang;
    using Boo.Lang.Compiler;
    using Boo.Lang.Compiler.Ast;
    using Boo.Lang.PatternMatching;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Text;

    [Serializable]
    public sealed class PerNodeMacro : LexicalInfoPreservingGeneratorMacro
    {
        [CompilerGenerated]
        private MacroStatement __macro;

        public PerNodeMacro()
        {
        }

        public PerNodeMacro(CompilerContext context) : base(context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
        }

        protected override IEnumerable<Node> ExpandGeneratorImpl(MacroStatement perNode)
        {
            return new $ExpandGeneratorImpl$296(perNode, this);
        }

        [CompilerGenerated]
        protected override Statement ExpandImpl(MacroStatement perNode)
        {
            throw new NotImplementedException("Boo installed version is older than the new macro syntax 'perNode' is using. Read BOO-1077 for more info.");
        }

        [Serializable, CompilerGenerated]
        internal sealed class $ExpandGeneratorImpl$296 : GenericGenerator<Node>
        {
            internal MacroStatement $perNode$330;
            internal PerNodeMacro $self_$331;

            public $ExpandGeneratorImpl$296(MacroStatement perNode, PerNodeMacro self_)
            {
                this.$perNode$330 = perNode;
                this.$self_$331 = self_;
            }

            public override IEnumerator<Node> GetEnumerator()
            {
                return new $(this.$perNode$330, this.$self_$331);
            }

            [Serializable, CompilerGenerated]
            internal sealed class $ : GenericGeneratorEnumerator<Node>, IEnumerator
            {
                internal ReferenceExpression $$177$304;
                internal ReferenceExpression $$178$306;
                internal MethodInvocationExpression $$179$307;
                internal Field $$180$308;
                internal ReferenceExpression $$181$309;
                internal Slice $$182$310;
                internal SlicingExpression $$183$311;
                internal BinaryExpression $$184$312;
                internal ReferenceExpression $$185$313;
                internal BinaryExpression $$186$314;
                internal StatementModifier $$187$315;
                internal ReferenceExpression $$188$316;
                internal ReturnStatement $$189$317;
                internal ReferenceExpression $$190$318;
                internal BinaryExpression $$191$319;
                internal Slice $$192$320;
                internal SlicingExpression $$193$321;
                internal ReferenceExpression $$194$322;
                internal BinaryExpression $$195$323;
                internal ReferenceExpression $$196$324;
                internal ReturnStatement $$197$325;
                internal Block $$198$326;
                internal MacroStatement $$199$327;
                internal Block $$match$11$300;
                internal Block $$match$12$301;
                internal ReturnStatement $$match$13$302;
                internal MacroStatement $$match$14$297;
                internal MacroStatement $$match$15$298;
                internal ReferenceExpression $key$305;
                internal ReferenceExpression $node$299;
                internal MacroStatement $perNode$328;
                internal PerNodeMacro $self_$329;
                internal Expression $value$303;

                public $(MacroStatement perNode, PerNodeMacro self_)
                {
                    this.$perNode$328 = perNode;
                    this.$self_$329 = self_;
                }

                public override bool MoveNext()
                {
                    // This item is obfuscated and can not be translated.
                    switch (base._state)
                    {
                        case 1:
                            break;

                        case 2:
                        {
                            MacroStatement statement7 = this.$$199$327 = new MacroStatement(LexicalInfo.Empty);
                            this.$$199$327.set_Name("block");
                            Block block4 = this.$$198$326 = new Block(LexicalInfo.Empty);
                            Statement[] statementArray1 = new Statement[5];
                            BinaryExpression expression39 = this.$$184$312 = new BinaryExpression(LexicalInfo.Empty);
                            this.$$184$312.set_Operator(15);
                            ReferenceExpression expression40 = this.$$181$309 = new ReferenceExpression(LexicalInfo.Empty);
                            this.$$181$309.set_Name("cached");
                            this.$$184$312.set_Left(this.$$181$309);
                            SlicingExpression expression41 = this.$$183$311 = new SlicingExpression(LexicalInfo.Empty);
                            this.$$183$311.set_Target(Expression.Lift(this.$node$299));
                            Slice[] sliceArray1 = new Slice[1];
                            Slice slice1 = this.$$182$310 = new Slice(LexicalInfo.Empty);
                            this.$$182$310.set_Begin(Expression.Lift(this.$key$305));
                            sliceArray1[0] = this.$$182$310;
                            this.$$183$311.set_Indices(SliceCollection.FromArray(sliceArray1));
                            this.$$184$312.set_Right(this.$$183$311);
                            statementArray1[0] = Statement.Lift(this.$$184$312);
                            ReturnStatement statement8 = this.$$189$317 = new ReturnStatement(LexicalInfo.Empty);
                            StatementModifier modifier1 = this.$$187$315 = new StatementModifier(LexicalInfo.Empty);
                            this.$$187$315.set_Type(1);
                            BinaryExpression expression42 = this.$$186$314 = new BinaryExpression(LexicalInfo.Empty);
                            this.$$186$314.set_Operator(0x18);
                            ReferenceExpression expression43 = this.$$185$313 = new ReferenceExpression(LexicalInfo.Empty);
                            this.$$185$313.set_Name("cached");
                            this.$$186$314.set_Left(this.$$185$313);
                            this.$$186$314.set_Right(new NullLiteralExpression(LexicalInfo.Empty));
                            this.$$187$315.set_Condition(this.$$186$314);
                            this.$$189$317.set_Modifier(this.$$187$315);
                            ReferenceExpression expression44 = this.$$188$316 = new ReferenceExpression(LexicalInfo.Empty);
                            this.$$188$316.set_Name("cached");
                            this.$$189$317.set_Expression(this.$$188$316);
                            statementArray1[1] = Statement.Lift(this.$$189$317);
                            BinaryExpression expression45 = this.$$191$319 = new BinaryExpression(LexicalInfo.Empty);
                            this.$$191$319.set_Operator(15);
                            ReferenceExpression expression46 = this.$$190$318 = new ReferenceExpression(LexicalInfo.Empty);
                            this.$$190$318.set_Name("value");
                            this.$$191$319.set_Left(this.$$190$318);
                            this.$$191$319.set_Right(Expression.Lift(this.$value$303));
                            statementArray1[2] = Statement.Lift(this.$$191$319);
                            BinaryExpression expression47 = this.$$195$323 = new BinaryExpression(LexicalInfo.Empty);
                            this.$$195$323.set_Operator(15);
                            SlicingExpression expression48 = this.$$193$321 = new SlicingExpression(LexicalInfo.Empty);
                            this.$$193$321.set_Target(Expression.Lift(this.$node$299));
                            Slice[] sliceArray2 = new Slice[1];
                            Slice slice3 = this.$$192$320 = new Slice(LexicalInfo.Empty);
                            this.$$192$320.set_Begin(Expression.Lift(this.$key$305));
                            sliceArray2[0] = this.$$192$320;
                            this.$$193$321.set_Indices(SliceCollection.FromArray(sliceArray2));
                            this.$$195$323.set_Left(this.$$193$321);
                            ReferenceExpression expression49 = this.$$194$322 = new ReferenceExpression(LexicalInfo.Empty);
                            this.$$194$322.set_Name("value");
                            this.$$195$323.set_Right(this.$$194$322);
                            statementArray1[3] = Statement.Lift(this.$$195$323);
                            ReturnStatement statement9 = this.$$197$325 = new ReturnStatement(LexicalInfo.Empty);
                            ReferenceExpression expression50 = this.$$196$324 = new ReferenceExpression(LexicalInfo.Empty);
                            this.$$196$324.set_Name("value");
                            this.$$197$325.set_Expression(this.$$196$324);
                            statementArray1[4] = Statement.Lift(this.$$197$325);
                            this.$$198$326.set_Statements(StatementCollection.FromArray(statementArray1));
                            this.$$199$327.set_Body(this.$$198$326);
                            break;
                        }
                        case 3:
                            this.YieldDefault(1);
                            break;

                        default:
                            if (this.$perNode$328 == null)
                            {
                                throw new ArgumentNullException("perNode");
                            }
                            this.$self_$329.__macro = this.$perNode$328;
                            this.$$match$14$297 = this.$perNode$328;
                            if (this.$$match$14$297 is MacroStatement)
                            {
                                MacroStatement statement1 = this.$$match$15$298 = this.$$match$14$297;
                                if (((1 != 0) && (this.$$match$15$298.get_Name() == "perNode")) && ((1 == this.$$match$15$298.get_Arguments().Count) && (this.$$match$15$298.get_Arguments().get_Item(0) is ReferenceExpression)))
                                {
                                    ReferenceExpression expression1 = this.$node$299 = (ReferenceExpression) this.$$match$15$298.get_Arguments().get_Item(0);
                                    if (1 != 0)
                                    {
                                        this.$$match$11$300 = this.$perNode$328.get_Body();
                                        if (this.$$match$11$300 is Block)
                                        {
                                            Block block1 = this.$$match$12$301 = this.$$match$11$300;
                                            if (((1 != 0) && (1 == this.$$match$12$301.get_Statements().Count)) && (this.$$match$12$301.get_Statements().get_Item(0) is ReturnStatement))
                                            {
                                                ReturnStatement statement6 = this.$$match$13$302 = (ReturnStatement) this.$$match$12$301.get_Statements().get_Item(0);
                                                if (1 != 0)
                                                {
                                                    Expression expression35 = this.$value$303 = this.$$match$13$302.get_Expression();
                                                    if (1 != 0)
                                                    {
                                                        ReferenceExpression expression36 = this.$$177$304 = new ReferenceExpression();
                                                        string[] textArray1 = new string[] { this.$node$299.get_Name() };
                                                        this.$$177$304.set_Name(this.$self_$329.get_Context().GetUniqueName(textArray1));
                                                        this.$key$305 = this.$$177$304;
                                                        Field field1 = this.$$180$308 = new Field(LexicalInfo.Empty);
                                                        this.$$180$308.set_Modifiers(0x61);
                                                        this.$$180$308.set_Name("$");
                                                        MethodInvocationExpression expression37 = this.$$179$307 = new MethodInvocationExpression(LexicalInfo.Empty);
                                                        ReferenceExpression expression38 = this.$$178$306 = new ReferenceExpression(LexicalInfo.Empty);
                                                        this.$$178$306.set_Name("object");
                                                        this.$$179$307.set_Target(this.$$178$306);
                                                        this.$$180$308.set_Initializer(this.$$179$307);
                                                        this.$$180$308.set_IsVolatile(false);
                                                        this.$$180$308.set_Name(CodeSerializer.LiftName(this.$key$305));
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                        throw new MatchError(new StringBuilder("`perNode.Body` failed to match `").Append(this.$$match$11$300).Append("`").ToString());
                                    }
                                }
                            }
                            throw new Exception("`perNode` macro invocation argument(s) did not match definition: `perNode((node as Boo.Lang.Compiler.Ast.ReferenceExpression))`");
                    }
                    return false;
                }
            }
        }
    }
}

