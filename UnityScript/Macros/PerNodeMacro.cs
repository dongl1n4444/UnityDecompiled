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

        protected override IEnumerable<Node> ExpandGeneratorImpl(MacroStatement perNode) => 
            new $ExpandGeneratorImpl$296(perNode, this);

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

            public override IEnumerator<Node> GetEnumerator() => 
                new $(this.$perNode$330, this.$self_$331);

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
                            string text5 = this.$$199$327.Name = "block";
                            Block block4 = this.$$198$326 = new Block(LexicalInfo.Empty);
                            Statement[] items = new Statement[5];
                            BinaryExpression expression41 = this.$$184$312 = new BinaryExpression(LexicalInfo.Empty);
                            int num3 = (int) (this.$$184$312.Operator = BinaryOperatorType.Assign);
                            ReferenceExpression expression42 = this.$$181$309 = new ReferenceExpression(LexicalInfo.Empty);
                            string text6 = this.$$181$309.Name = "cached";
                            ReferenceExpression expression43 = this.$$184$312.Left = this.$$181$309;
                            SlicingExpression expression44 = this.$$183$311 = new SlicingExpression(LexicalInfo.Empty);
                            Expression expression45 = this.$$183$311.Target = Expression.Lift(this.$node$299);
                            Slice[] sliceArray1 = new Slice[1];
                            Slice slice1 = this.$$182$310 = new Slice(LexicalInfo.Empty);
                            Expression expression46 = this.$$182$310.Begin = Expression.Lift(this.$key$305);
                            sliceArray1[0] = this.$$182$310;
                            SliceCollection collection1 = this.$$183$311.Indices = SliceCollection.FromArray(sliceArray1);
                            SlicingExpression expression47 = this.$$184$312.Right = this.$$183$311;
                            items[0] = Statement.Lift(this.$$184$312);
                            ReturnStatement statement8 = this.$$189$317 = new ReturnStatement(LexicalInfo.Empty);
                            StatementModifier modifier1 = this.$$187$315 = new StatementModifier(LexicalInfo.Empty);
                            int num4 = (int) (this.$$187$315.Type = StatementModifierType.If);
                            BinaryExpression expression48 = this.$$186$314 = new BinaryExpression(LexicalInfo.Empty);
                            int num5 = (int) (this.$$186$314.Operator = BinaryOperatorType.ReferenceInequality);
                            ReferenceExpression expression49 = this.$$185$313 = new ReferenceExpression(LexicalInfo.Empty);
                            string text7 = this.$$185$313.Name = "cached";
                            ReferenceExpression expression50 = this.$$186$314.Left = this.$$185$313;
                            NullLiteralExpression expression51 = this.$$186$314.Right = new NullLiteralExpression(LexicalInfo.Empty);
                            BinaryExpression expression52 = this.$$187$315.Condition = this.$$186$314;
                            StatementModifier modifier3 = this.$$189$317.Modifier = this.$$187$315;
                            ReferenceExpression expression53 = this.$$188$316 = new ReferenceExpression(LexicalInfo.Empty);
                            string text8 = this.$$188$316.Name = "cached";
                            ReferenceExpression expression54 = this.$$189$317.Expression = this.$$188$316;
                            items[1] = Statement.Lift(this.$$189$317);
                            BinaryExpression expression55 = this.$$191$319 = new BinaryExpression(LexicalInfo.Empty);
                            int num6 = (int) (this.$$191$319.Operator = BinaryOperatorType.Assign);
                            ReferenceExpression expression56 = this.$$190$318 = new ReferenceExpression(LexicalInfo.Empty);
                            string text9 = this.$$190$318.Name = "value";
                            ReferenceExpression expression57 = this.$$191$319.Left = this.$$190$318;
                            Expression expression58 = this.$$191$319.Right = Expression.Lift(this.$value$303);
                            items[2] = Statement.Lift(this.$$191$319);
                            BinaryExpression expression59 = this.$$195$323 = new BinaryExpression(LexicalInfo.Empty);
                            int num7 = (int) (this.$$195$323.Operator = BinaryOperatorType.Assign);
                            SlicingExpression expression60 = this.$$193$321 = new SlicingExpression(LexicalInfo.Empty);
                            Expression expression61 = this.$$193$321.Target = Expression.Lift(this.$node$299);
                            Slice[] sliceArray2 = new Slice[1];
                            Slice slice3 = this.$$192$320 = new Slice(LexicalInfo.Empty);
                            Expression expression62 = this.$$192$320.Begin = Expression.Lift(this.$key$305);
                            sliceArray2[0] = this.$$192$320;
                            SliceCollection collection2 = this.$$193$321.Indices = SliceCollection.FromArray(sliceArray2);
                            SlicingExpression expression63 = this.$$195$323.Left = this.$$193$321;
                            ReferenceExpression expression64 = this.$$194$322 = new ReferenceExpression(LexicalInfo.Empty);
                            string text10 = this.$$194$322.Name = "value";
                            ReferenceExpression expression65 = this.$$195$323.Right = this.$$194$322;
                            items[3] = Statement.Lift(this.$$195$323);
                            ReturnStatement statement9 = this.$$197$325 = new ReturnStatement(LexicalInfo.Empty);
                            ReferenceExpression expression66 = this.$$196$324 = new ReferenceExpression(LexicalInfo.Empty);
                            string text11 = this.$$196$324.Name = "value";
                            ReferenceExpression expression67 = this.$$197$325.Expression = this.$$196$324;
                            items[4] = Statement.Lift(this.$$197$325);
                            StatementCollection collection3 = this.$$198$326.Statements = StatementCollection.FromArray(items);
                            Block block5 = this.$$199$327.Body = this.$$198$326;
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
                                if (((1 != 0) && (this.$$match$15$298.Name == "perNode")) && ((1 == this.$$match$15$298.Arguments.Count) && (this.$$match$15$298.Arguments[0] is ReferenceExpression)))
                                {
                                    ReferenceExpression expression1 = this.$node$299 = (ReferenceExpression) this.$$match$15$298.Arguments[0];
                                    if (1 != 0)
                                    {
                                        this.$$match$11$300 = this.$perNode$328.Body;
                                        if (this.$$match$11$300 is Block)
                                        {
                                            Block block1 = this.$$match$12$301 = this.$$match$11$300;
                                            if (((1 != 0) && (1 == this.$$match$12$301.Statements.Count)) && (this.$$match$12$301.Statements[0] is ReturnStatement))
                                            {
                                                ReturnStatement statement6 = this.$$match$13$302 = (ReturnStatement) this.$$match$12$301.Statements[0];
                                                if (1 != 0)
                                                {
                                                    Expression expression35 = this.$value$303 = this.$$match$13$302.Expression;
                                                    if (1 != 0)
                                                    {
                                                        ReferenceExpression expression36 = this.$$177$304 = new ReferenceExpression();
                                                        string[] components = new string[] { this.$node$299.Name };
                                                        string text1 = this.$$177$304.Name = this.$self_$329.Context.GetUniqueName(components);
                                                        this.$key$305 = this.$$177$304;
                                                        Field field1 = this.$$180$308 = new Field(LexicalInfo.Empty);
                                                        int num1 = (int) (this.$$180$308.Modifiers = TypeMemberModifiers.Final | TypeMemberModifiers.Static | TypeMemberModifiers.Private);
                                                        string text2 = this.$$180$308.Name = "$";
                                                        MethodInvocationExpression expression37 = this.$$179$307 = new MethodInvocationExpression(LexicalInfo.Empty);
                                                        ReferenceExpression expression38 = this.$$178$306 = new ReferenceExpression(LexicalInfo.Empty);
                                                        string text3 = this.$$178$306.Name = "object";
                                                        ReferenceExpression expression39 = this.$$179$307.Target = this.$$178$306;
                                                        MethodInvocationExpression expression40 = this.$$180$308.Initializer = this.$$179$307;
                                                        int num2 = (int) (this.$$180$308.IsVolatile = false);
                                                        string text4 = this.$$180$308.Name = CodeSerializer.LiftName(this.$key$305);
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

