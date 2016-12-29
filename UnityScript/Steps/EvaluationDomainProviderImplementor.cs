namespace UnityScript.Steps
{
    using Boo.Lang;
    using Boo.Lang.Compiler;
    using Boo.Lang.Compiler.Ast;
    using Boo.Lang.Compiler.TypeSystem;
    using Boo.Lang.Compiler.Util;
    using Boo.Lang.Environments;
    using Boo.Lang.Runtime;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using UnityScript.Scripting;

    [Serializable]
    public class EvaluationDomainProviderImplementor : AbstractCompilerComponent
    {
        [NonSerialized]
        private static readonly object $CompileUnit$31 = new object();
        [NonSerialized]
        private static readonly object $node$30 = new object();

        private Type AnyPublicTypeOf(Assembly assembly)
        {
            int index = 0;
            Type[] types = assembly.GetTypes();
            int length = types.Length;
            goto Label_003B;
            index++;
        Label_003B:
            if (index >= length)
            {
            }
            return ((!types[index].IsPublic || types[index].IsGenericType) ? null : types[index]);
        }

        private Expression AssemblyReferencesArray()
        {
            object obj2 = this.get_CompileUnit().get_Item($CompileUnit$31);
            if (obj2 == null)
            {
            }
            MemberReferenceExpression expression = this.get_CodeBuilder().CreateReference(this.CreateAssemblyReferencesArray());
            this.get_CompileUnit().set_Item($CompileUnit$31, expression);
            return ((obj2 is Expression) ? expression : ((Expression) RuntimeServices.Coerce(obj2, typeof(Expression))));
        }

        private IField CreateAssemblyReferencesArray()
        {
            ReferenceExpression expression;
            MemberReferenceExpression expression2;
            MemberReferenceExpression expression3;
            MemberReferenceExpression expression4;
            MethodInvocationExpression expression5;
            ArrayLiteralExpression expression6;
            Field field;
            ClassDefinition definition;
            ArrayLiteralExpression expression1 = expression6 = new ArrayLiteralExpression(LexicalInfo.Empty);
            Expression[] expressionArray1 = new Expression[1];
            MethodInvocationExpression expression16 = expression5 = new MethodInvocationExpression(LexicalInfo.Empty);
            MemberReferenceExpression expression17 = expression4 = new MemberReferenceExpression(LexicalInfo.Empty);
            expression4.set_Name("GetExecutingAssembly");
            MemberReferenceExpression expression18 = expression3 = new MemberReferenceExpression(LexicalInfo.Empty);
            expression3.set_Name("Assembly");
            MemberReferenceExpression expression19 = expression2 = new MemberReferenceExpression(LexicalInfo.Empty);
            expression2.set_Name("Reflection");
            ReferenceExpression expression20 = expression = new ReferenceExpression(LexicalInfo.Empty);
            expression.set_Name("System");
            expression2.set_Target(expression);
            expression3.set_Target(expression2);
            expression4.set_Target(expression3);
            expression5.set_Target(expression4);
            expressionArray1[0] = expression5;
            expression6.set_Items(ExpressionCollection.FromArray(expressionArray1));
            ArrayLiteralExpression expression7 = expression6;
            foreach (Assembly assembly in this.ReferencedAssemblies())
            {
                Type type = this.AnyPublicTypeOf(assembly);
                if (type != null)
                {
                    TypeofExpression expression8;
                    MemberReferenceExpression expression9;
                    MemberReferenceExpression expression21 = expression9 = new MemberReferenceExpression(LexicalInfo.Empty);
                    expression9.set_Name("Assembly");
                    TypeofExpression expression22 = expression8 = new TypeofExpression(LexicalInfo.Empty);
                    expression8.set_Type(TypeReference.Lift(type));
                    expression9.set_Target(expression8);
                    expression7.get_Items().Add(expression9);
                }
            }
            ClassDefinition definition1 = definition = new ClassDefinition(LexicalInfo.Empty);
            definition.set_Modifiers(2);
            definition.set_Name("EvalAssemblyReferences");
            TypeMember[] memberArray1 = new TypeMember[1];
            Field field1 = field = new Field(LexicalInfo.Empty);
            field.set_Modifiers(0x68);
            field.set_Name("Value");
            field.set_Initializer(Expression.Lift(expression7));
            field.set_IsVolatile(false);
            memberArray1[0] = field;
            definition.set_Members(TypeMemberCollection.FromArray(memberArray1));
            ClassDefinition definition2 = definition;
            My<CodeReifier>.Instance.ReifyInto(this.get_CompileUnit().get_Modules().get_Item(0), definition2);
            return (IField) definition2.get_Members().get_Item("Value").get_Entity();
        }

        private IEntity CreateStaticEvaluationDomainProviderReferenceOn(ClassDefinition node)
        {
            ReferenceExpression expression;
            MethodInvocationExpression expression2;
            Field field;
            MethodInvocationExpression expression3;
            Block block;
            Constructor constructor;
            ReturnStatement statement;
            Block block2;
            Method method;
            ClassDefinition definition;
            ClassDefinition definition1 = definition = new ClassDefinition(LexicalInfo.Empty);
            definition.set_Modifiers(8);
            definition.set_Name("StaticEvaluationDomainProvider");
            TypeMember[] memberArray1 = new TypeMember[3];
            Field field1 = field = new Field(LexicalInfo.Empty);
            field.set_Modifiers(0x68);
            field.set_Name("Instance");
            MethodInvocationExpression expression1 = expression2 = new MethodInvocationExpression(LexicalInfo.Empty);
            ReferenceExpression expression8 = expression = new ReferenceExpression(LexicalInfo.Empty);
            expression.set_Name("StaticEvaluationDomainProvider");
            expression2.set_Target(expression);
            field.set_Initializer(expression2);
            field.set_IsVolatile(false);
            memberArray1[0] = field;
            Constructor constructor1 = constructor = new Constructor(LexicalInfo.Empty);
            constructor.set_Name("constructor");
            Block block1 = block = new Block(LexicalInfo.Empty);
            Statement[] statementArray1 = new Statement[1];
            MethodInvocationExpression expression9 = expression3 = new MethodInvocationExpression(LexicalInfo.Empty);
            expression3.set_Target(new SuperLiteralExpression(LexicalInfo.Empty));
            Expression[] expressionArray1 = new Expression[] { Expression.Lift(this.ImportsArrayFor(node)) };
            expression3.set_Arguments(ExpressionCollection.FromArray(expressionArray1));
            statementArray1[0] = Statement.Lift(expression3);
            block.set_Statements(StatementCollection.FromArray(statementArray1));
            constructor.set_Body(block);
            memberArray1[1] = constructor;
            Method method1 = method = new Method(LexicalInfo.Empty);
            method.set_Modifiers(0x100);
            method.set_Name("GetAssemblyReferences");
            Block block5 = block2 = new Block(LexicalInfo.Empty);
            Statement[] statementArray2 = new Statement[1];
            ReturnStatement statement1 = statement = new ReturnStatement(LexicalInfo.Empty);
            statement.set_Expression(Expression.Lift(this.AssemblyReferencesArray()));
            statementArray2[0] = Statement.Lift(statement);
            block2.set_Statements(StatementCollection.FromArray(statementArray2));
            method.set_Body(block2);
            memberArray1[2] = method;
            definition.set_Members(TypeMemberCollection.FromArray(memberArray1));
            TypeReference[] referenceArray1 = new TypeReference[] { TypeReference.Lift(typeof(SimpleEvaluationDomainProvider)) };
            definition.set_BaseTypes(TypeReferenceCollection.FromArray(referenceArray1));
            ClassDefinition definition2 = definition;
            My<CodeReifier>.Instance.ReifyInto(node, definition2);
            return definition2.get_Members().get_Item("Instance").get_Entity();
        }

        public void ImplementIEvaluationDomainProviderOn(ClassDefinition node)
        {
            Field field;
            MethodInvocationExpression expression2;
            BinaryExpression expression3;
            BinaryExpression expression4;
            ReturnStatement statement;
            Block block;
            Method method;
            ReturnStatement statement2;
            Block block2;
            Method method2;
            ReturnStatement statement3;
            Block block3;
            Method method3;
            ClassDefinition definition;
            string[] textArray1 = new string[] { "domain" };
            ReferenceExpression expression = new ReferenceExpression(this.get_Context().GetUniqueName(textArray1));
            ClassDefinition definition1 = definition = new ClassDefinition(LexicalInfo.Empty);
            definition.set_Name("_");
            TypeMember[] memberArray1 = new TypeMember[4];
            Field field1 = field = new Field(LexicalInfo.Empty);
            field.set_Modifiers(1);
            field.set_Name("$");
            field.set_Type(TypeReference.Lift(typeof(EvaluationDomain)));
            field.set_IsVolatile(false);
            field.set_Name(CodeSerializer.LiftName(expression));
            memberArray1[0] = field;
            Method method1 = method = new Method(LexicalInfo.Empty);
            method.set_Modifiers(8);
            method.set_Name("GetEvaluationDomain");
            Block block1 = block = new Block(LexicalInfo.Empty);
            Statement[] statementArray1 = new Statement[1];
            ReturnStatement statement1 = statement = new ReturnStatement(LexicalInfo.Empty);
            BinaryExpression expression1 = expression4 = new BinaryExpression(LexicalInfo.Empty);
            expression4.set_Operator(0x1c);
            expression4.set_Left(Expression.Lift(expression));
            BinaryExpression expression13 = expression3 = new BinaryExpression(LexicalInfo.Empty);
            expression3.set_Operator(15);
            expression3.set_Left(Expression.Lift(expression));
            MethodInvocationExpression expression14 = expression2 = new MethodInvocationExpression(LexicalInfo.Empty);
            expression2.set_Target(Expression.Lift(typeof(EvaluationDomain)));
            expression3.set_Right(expression2);
            expression4.set_Right(expression3);
            statement.set_Expression(expression4);
            statementArray1[0] = Statement.Lift(statement);
            block.set_Statements(StatementCollection.FromArray(statementArray1));
            method.set_Body(block);
            memberArray1[1] = method;
            Method method4 = method2 = new Method(LexicalInfo.Empty);
            method2.set_Modifiers(8);
            method2.set_Name("GetImports");
            Block block7 = block2 = new Block(LexicalInfo.Empty);
            Statement[] statementArray2 = new Statement[1];
            ReturnStatement statement4 = statement2 = new ReturnStatement(LexicalInfo.Empty);
            statement2.set_Expression(Expression.Lift(this.ImportsArrayFor(node)));
            statementArray2[0] = Statement.Lift(statement2);
            block2.set_Statements(StatementCollection.FromArray(statementArray2));
            method2.set_Body(block2);
            memberArray1[2] = method2;
            Method method5 = method3 = new Method(LexicalInfo.Empty);
            method3.set_Modifiers(8);
            method3.set_Name("GetAssemblyReferences");
            Block block8 = block3 = new Block(LexicalInfo.Empty);
            Statement[] statementArray3 = new Statement[1];
            ReturnStatement statement5 = statement3 = new ReturnStatement(LexicalInfo.Empty);
            statement3.set_Expression(Expression.Lift(this.AssemblyReferencesArray()));
            statementArray3[0] = Statement.Lift(statement3);
            block3.set_Statements(StatementCollection.FromArray(statementArray3));
            method3.set_Body(block3);
            memberArray1[3] = method3;
            definition.set_Members(TypeMemberCollection.FromArray(memberArray1));
            TypeReference[] referenceArray1 = new TypeReference[] { TypeReference.Lift(typeof(IEvaluationDomainProvider)) };
            definition.set_BaseTypes(TypeReferenceCollection.FromArray(referenceArray1));
            ClassDefinition definition2 = definition;
            My<CodeReifier>.Instance.MergeInto(node, definition2);
        }

        private ArrayLiteralExpression ImportsArrayFor(ClassDefinition node)
        {
            IntegerLiteralExpression expression;
            ArrayTypeReference reference;
            ArrayLiteralExpression expression2;
            $ImportsArrayFor$locals$273 s$ = new $ImportsArrayFor$locals$273 {
                $node = node
            };
            ArrayLiteralExpression expression1 = expression2 = new ArrayLiteralExpression(LexicalInfo.Empty);
            ArrayTypeReference reference1 = reference = new ArrayTypeReference(LexicalInfo.Empty);
            reference.set_IsPointer(false);
            reference.set_ElementType(TypeReference.Lift(typeof(string)));
            IntegerLiteralExpression expression5 = expression = new IntegerLiteralExpression(LexicalInfo.Empty);
            expression.set_Value(1L);
            expression.set_IsLong(false);
            reference.set_Rank(expression);
            expression2.set_Type(reference);
            ArrayLiteralExpression expression3 = expression2;
            expression3.get_Items().AddRange(new $ImportsArrayFor$332(s$));
            return expression3;
        }

        private Set<Assembly> ReferencedAssemblies()
        {
            ReferencedAssemblyCollector collector = new ReferencedAssemblyCollector();
            this.get_CompileUnit().Accept(collector);
            return collector.ReferencedAssemblies;
        }

        public IField StaticEvaluationDomainProviderFor(ClassDefinition node)
        {
            object obj2 = node.get_Item($node$30);
            if (obj2 == null)
            {
            }
            IEntity entity = this.CreateStaticEvaluationDomainProviderReferenceOn(node);
            node.set_Item($node$30, entity);
            return ((obj2 is IField) ? ((IField) entity) : ((IField) RuntimeServices.Coerce(obj2, typeof(IField))));
        }

        [Serializable, CompilerGenerated]
        internal sealed class $ImportsArrayFor$332 : GenericGenerator<Expression>
        {
            internal EvaluationDomainProviderImplementor.$ImportsArrayFor$locals$273 $$locals$334;

            public $ImportsArrayFor$332(EvaluationDomainProviderImplementor.$ImportsArrayFor$locals$273 $$locals$334)
            {
                this.$$locals$334 = $$locals$334;
            }

            public override IEnumerator<Expression> GetEnumerator() => 
                new Enumerator(this.$$locals$334);

            [Serializable]
            internal class Enumerator : IEnumerator<Expression>, IDisposable, ICloneable
            {
                protected Expression $$current;
                protected IEnumerator<Import> $$enumerator;
                internal EvaluationDomainProviderImplementor.$ImportsArrayFor$locals$273 $$locals$333;

                public Enumerator(EvaluationDomainProviderImplementor.$ImportsArrayFor$locals$273 $$locals$333)
                {
                    this.$$locals$333 = $$locals$333;
                    this.Reset();
                }

                public override object Clone() => 
                    this.MemberwiseClone();

                public override void Dispose()
                {
                    this.$$enumerator.Dispose();
                }

                public override bool MoveNext() => 
                    this.$$enumerator.MoveNext();

                public override void Reset()
                {
                    this.$$enumerator = this.$$locals$333.$node.get_EnclosingModule().get_Imports().GetEnumerator();
                }

                public override Expression Current =>
                    this.$$current;

                public override object System.Collections.IEnumerator.Current =>
                    this.$$current;
            }
        }

        [Serializable]
        internal class $ImportsArrayFor$locals$273
        {
            internal ClassDefinition $node;
        }
    }
}

