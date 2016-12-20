namespace UnityScript.Scripting.Pipeline
{
    using Boo.Lang.Compiler.Ast;
    using Boo.Lang.Compiler.Steps;
    using System;
    using UnityScript.Scripting;
    using UnityScript.Steps;

    [Serializable]
    public class IntroduceEvaluationContext : AbstractCompilerStep
    {
        protected EvaluationContext _evaluationContext;

        public IntroduceEvaluationContext(EvaluationContext evaluationContext)
        {
            this._evaluationContext = evaluationContext;
        }

        public ClassDefinition GetContextFieldDeclaration()
        {
            Field field;
            Field field2;
            ParameterDeclaration declaration;
            MemberReferenceExpression expression;
            ReferenceExpression expression2;
            BinaryExpression expression3;
            ReferenceExpression expression4;
            ReferenceExpression expression5;
            MemberReferenceExpression expression6;
            BinaryExpression expression7;
            Block block;
            Constructor constructor;
            ClassDefinition definition;
            Type type = this._evaluationContext.GetType();
            Type type2 = this._evaluationContext.ScriptContainer.GetType();
            ClassDefinition definition1 = definition = new ClassDefinition(LexicalInfo.Empty);
            definition.set_Name("_");
            TypeMember[] memberArray1 = new TypeMember[3];
            Field field1 = field = new Field(LexicalInfo.Empty);
            field.set_Modifiers(40);
            field.set_Name("ScriptContainer");
            field.set_Type(TypeReference.Lift(type2));
            field.set_IsVolatile(false);
            memberArray1[0] = field;
            Field field3 = field2 = new Field(LexicalInfo.Empty);
            field2.set_Name("EvaluationContext");
            field2.set_Type(TypeReference.Lift(type));
            field2.set_IsVolatile(false);
            memberArray1[1] = field2;
            Constructor constructor1 = constructor = new Constructor(LexicalInfo.Empty);
            constructor.set_Name("constructor");
            ParameterDeclaration[] declarationArray1 = new ParameterDeclaration[1];
            ParameterDeclaration declaration1 = declaration = new ParameterDeclaration(LexicalInfo.Empty);
            declaration.set_Name("context");
            declaration.set_Type(TypeReference.Lift(type));
            declarationArray1[0] = declaration;
            constructor.set_Parameters(ParameterDeclarationCollection.FromArray(false, declarationArray1));
            Block block1 = block = new Block(LexicalInfo.Empty);
            Statement[] statementArray1 = new Statement[2];
            BinaryExpression expression1 = expression3 = new BinaryExpression(LexicalInfo.Empty);
            expression3.set_Operator(15);
            MemberReferenceExpression expression14 = expression = new MemberReferenceExpression(LexicalInfo.Empty);
            expression.set_Name("EvaluationContext");
            expression.set_Target(new SelfLiteralExpression(LexicalInfo.Empty));
            expression3.set_Left(expression);
            ReferenceExpression expression15 = expression2 = new ReferenceExpression(LexicalInfo.Empty);
            expression2.set_Name("context");
            expression3.set_Right(expression2);
            statementArray1[0] = Statement.Lift(expression3);
            BinaryExpression expression16 = expression7 = new BinaryExpression(LexicalInfo.Empty);
            expression7.set_Operator(15);
            ReferenceExpression expression17 = expression4 = new ReferenceExpression(LexicalInfo.Empty);
            expression4.set_Name("ScriptContainer");
            expression7.set_Left(expression4);
            MemberReferenceExpression expression18 = expression6 = new MemberReferenceExpression(LexicalInfo.Empty);
            expression6.set_Name("ScriptContainer");
            ReferenceExpression expression19 = expression5 = new ReferenceExpression(LexicalInfo.Empty);
            expression5.set_Name("context");
            expression6.set_Target(expression5);
            expression7.set_Right(expression6);
            statementArray1[1] = Statement.Lift(expression7);
            block.set_Statements(StatementCollection.FromArray(statementArray1));
            constructor.set_Body(block);
            memberArray1[2] = constructor;
            definition.set_Members(TypeMemberCollection.FromArray(memberArray1));
            return definition;
        }

        public override void Run()
        {
            ClassDefinition scriptClass = UtilitiesModule.GetScriptClass(this.get_Context());
            if (scriptClass != null)
            {
                scriptClass.Merge(this.GetContextFieldDeclaration());
            }
        }
    }
}

