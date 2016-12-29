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
    public sealed class DeferredMacro : LexicalInfoPreservingGeneratorMacro
    {
        [CompilerGenerated]
        private MacroStatement __macro;

        public DeferredMacro()
        {
        }

        public DeferredMacro(CompilerContext context) : base(context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
        }

        protected override IEnumerable<Node> ExpandGeneratorImpl(MacroStatement deferred) => 
            new $ExpandGeneratorImpl$279(deferred, this);

        [CompilerGenerated]
        protected override Statement ExpandImpl(MacroStatement deferred)
        {
            throw new NotImplementedException("Boo installed version is older than the new macro syntax 'deferred' is using. Read BOO-1077 for more info.");
        }

        [Serializable, CompilerGenerated]
        internal sealed class $ExpandGeneratorImpl$279 : GenericGenerator<Node>
        {
            internal MacroStatement $deferred$294;
            internal DeferredMacro $self_$295;

            public $ExpandGeneratorImpl$279(MacroStatement deferred, DeferredMacro self_)
            {
                this.$deferred$294 = deferred;
                this.$self_$295 = self_;
            }

            public override IEnumerator<Node> GetEnumerator() => 
                new $(this.$deferred$294, this.$self_$295);

            [Serializable, CompilerGenerated]
            internal sealed class $ : GenericGeneratorEnumerator<Node>, IEnumerator
            {
                internal Property $$172$286;
                internal Attribute $$173$288;
                internal ReturnStatement $$174$289;
                internal Block $$175$290;
                internal Method $$176$291;
                internal ReferenceExpression $$match$10$283;
                internal MacroStatement $$match$7$280;
                internal MacroStatement $$match$8$281;
                internal BinaryExpression $$match$9$282;
                internal MacroStatement $deferred$292;
                internal Expression $initializer$285;
                internal string $name$284;
                internal Property $p$287;
                internal DeferredMacro $self_$293;

                public $(MacroStatement deferred, DeferredMacro self_)
                {
                    this.$deferred$292 = deferred;
                    this.$self_$293 = self_;
                }

                public override bool MoveNext()
                {
                    // This item is obfuscated and can not be translated.
                    switch (base._state)
                    {
                        case 1:
                            break;

                        case 2:
                            this.YieldDefault(1);
                            break;

                        default:
                            if (this.$deferred$292 == null)
                            {
                                throw new ArgumentNullException("deferred");
                            }
                            this.$self_$293.__macro = this.$deferred$292;
                            this.$$match$7$280 = this.$deferred$292;
                            if (this.$$match$7$280 is MacroStatement)
                            {
                                MacroStatement statement1 = this.$$match$8$281 = this.$$match$7$280;
                                if (((1 != 0) && (this.$$match$8$281.get_Name() == "deferred")) && ((1 == this.$$match$8$281.get_Arguments().Count) && (this.$$match$8$281.get_Arguments().get_Item(0) is BinaryExpression)))
                                {
                                    BinaryExpression expression1 = this.$$match$9$282 = (BinaryExpression) this.$$match$8$281.get_Arguments().get_Item(0);
                                    if (((1 != 0) && (this.$$match$9$282.get_Operator() == 15)) && (this.$$match$9$282.get_Left() is ReferenceExpression))
                                    {
                                        ReferenceExpression expression5 = this.$$match$10$283 = this.$$match$9$282.get_Left();
                                        if (1 != 0)
                                        {
                                            string text1 = this.$name$284 = this.$$match$10$283.get_Name();
                                            if (1 != 0)
                                            {
                                                Expression expression6 = this.$initializer$285 = this.$$match$9$282.get_Right();
                                                if (1 != 0)
                                                {
                                                    Property property1 = this.$$172$286 = new Property();
                                                    this.$$172$286.set_Name(this.$name$284);
                                                    this.$p$287 = this.$$172$286;
                                                    Method method1 = this.$$176$291 = new Method(LexicalInfo.Empty);
                                                    this.$$176$291.set_Name("get");
                                                    Attribute[] attributeArray1 = new Attribute[1];
                                                    Attribute attribute1 = this.$$173$288 = new Attribute(LexicalInfo.Empty);
                                                    this.$$173$288.set_Name("Boo.Lang.Useful.Attributes.OnceAttribute");
                                                    attributeArray1[0] = this.$$173$288;
                                                    this.$$176$291.set_Attributes(AttributeCollection.FromArray(attributeArray1));
                                                    Block block1 = this.$$175$290 = new Block(LexicalInfo.Empty);
                                                    Statement[] statementArray1 = new Statement[1];
                                                    ReturnStatement statement3 = this.$$174$289 = new ReturnStatement(LexicalInfo.Empty);
                                                    this.$$174$289.set_Expression(Expression.Lift(this.$initializer$285));
                                                    statementArray1[0] = Statement.Lift(this.$$174$289);
                                                    this.$$175$290.set_Statements(StatementCollection.FromArray(statementArray1));
                                                    this.$$176$291.set_Body(this.$$175$290);
                                                    this.$p$287.set_Getter(this.$$176$291);
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            throw new MatchError(new StringBuilder("`deferred` failed to match `").Append(this.$$match$7$280).Append("`").ToString());
                    }
                    return false;
                }
            }
        }
    }
}

