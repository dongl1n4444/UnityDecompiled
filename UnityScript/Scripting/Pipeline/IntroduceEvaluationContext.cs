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
            string text1 = definition.Name = "_";
            TypeMember[] items = new TypeMember[3];
            Field field1 = field = new Field(LexicalInfo.Empty);
            int num1 = (int) (field.Modifiers = TypeMemberModifiers.Static | TypeMemberModifiers.Public);
            string text2 = field.Name = "ScriptContainer";
            TypeReference reference1 = field.Type = TypeReference.Lift(type2);
            int num2 = (int) (field.IsVolatile = false);
            items[0] = field;
            Field field3 = field2 = new Field(LexicalInfo.Empty);
            string text3 = field2.Name = "EvaluationContext";
            TypeReference reference4 = field2.Type = TypeReference.Lift(type);
            int num3 = (int) (field2.IsVolatile = false);
            items[1] = field2;
            Constructor constructor1 = constructor = new Constructor(LexicalInfo.Empty);
            string text4 = constructor.Name = "constructor";
            ParameterDeclaration[] parameters = new ParameterDeclaration[1];
            ParameterDeclaration declaration1 = declaration = new ParameterDeclaration(LexicalInfo.Empty);
            string text5 = declaration.Name = "context";
            TypeReference reference5 = declaration.Type = TypeReference.Lift(type);
            parameters[0] = declaration;
            ParameterDeclarationCollection collection1 = constructor.Parameters = ParameterDeclarationCollection.FromArray(false, parameters);
            Block block1 = block = new Block(LexicalInfo.Empty);
            Statement[] statementArray1 = new Statement[2];
            BinaryExpression expression1 = expression3 = new BinaryExpression(LexicalInfo.Empty);
            int num4 = (int) (expression3.Operator = BinaryOperatorType.Assign);
            MemberReferenceExpression expression14 = expression = new MemberReferenceExpression(LexicalInfo.Empty);
            string text6 = expression.Name = "EvaluationContext";
            SelfLiteralExpression expression15 = expression.Target = new SelfLiteralExpression(LexicalInfo.Empty);
            MemberReferenceExpression expression16 = expression3.Left = expression;
            ReferenceExpression expression17 = expression2 = new ReferenceExpression(LexicalInfo.Empty);
            string text7 = expression2.Name = "context";
            ReferenceExpression expression18 = expression3.Right = expression2;
            statementArray1[0] = Statement.Lift(expression3);
            BinaryExpression expression19 = expression7 = new BinaryExpression(LexicalInfo.Empty);
            int num5 = (int) (expression7.Operator = BinaryOperatorType.Assign);
            ReferenceExpression expression20 = expression4 = new ReferenceExpression(LexicalInfo.Empty);
            string text8 = expression4.Name = "ScriptContainer";
            ReferenceExpression expression21 = expression7.Left = expression4;
            MemberReferenceExpression expression22 = expression6 = new MemberReferenceExpression(LexicalInfo.Empty);
            string text9 = expression6.Name = "ScriptContainer";
            ReferenceExpression expression23 = expression5 = new ReferenceExpression(LexicalInfo.Empty);
            string text10 = expression5.Name = "context";
            ReferenceExpression expression24 = expression6.Target = expression5;
            MemberReferenceExpression expression25 = expression7.Right = expression6;
            statementArray1[1] = Statement.Lift(expression7);
            StatementCollection collection2 = block.Statements = StatementCollection.FromArray(statementArray1);
            Block block3 = constructor.Body = block;
            items[2] = constructor;
            TypeMemberCollection collection3 = definition.Members = TypeMemberCollection.FromArray(items);
            return definition;
        }

        public override void Run()
        {
            ClassDefinition scriptClass = UtilitiesModule.GetScriptClass(this.Context);
            if (scriptClass != null)
            {
                scriptClass.Merge(this.GetContextFieldDeclaration());
            }
        }
    }
}

