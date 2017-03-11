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
            object obj2 = this.CompileUnit[$CompileUnit$31];
            if (obj2 == null)
            {
            }
            MemberReferenceExpression expression = this.CodeBuilder.CreateReference(this.CreateAssemblyReferencesArray());
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
            Expression[] items = new Expression[1];
            MethodInvocationExpression expression16 = expression5 = new MethodInvocationExpression(LexicalInfo.Empty);
            MemberReferenceExpression expression17 = expression4 = new MemberReferenceExpression(LexicalInfo.Empty);
            string text1 = expression4.Name = "GetExecutingAssembly";
            MemberReferenceExpression expression18 = expression3 = new MemberReferenceExpression(LexicalInfo.Empty);
            string text2 = expression3.Name = "Assembly";
            MemberReferenceExpression expression19 = expression2 = new MemberReferenceExpression(LexicalInfo.Empty);
            string text3 = expression2.Name = "Reflection";
            ReferenceExpression expression20 = expression = new ReferenceExpression(LexicalInfo.Empty);
            string text4 = expression.Name = "System";
            ReferenceExpression expression21 = expression2.Target = expression;
            MemberReferenceExpression expression22 = expression3.Target = expression2;
            MemberReferenceExpression expression23 = expression4.Target = expression3;
            MemberReferenceExpression expression24 = expression5.Target = expression4;
            items[0] = expression5;
            ExpressionCollection collection1 = expression6.Items = ExpressionCollection.FromArray(items);
            ArrayLiteralExpression e = expression6;
            foreach (Assembly assembly in this.ReferencedAssemblies())
            {
                Type type = this.AnyPublicTypeOf(assembly);
                if (type != null)
                {
                    TypeofExpression expression8;
                    MemberReferenceExpression expression9;
                    MemberReferenceExpression expression25 = expression9 = new MemberReferenceExpression(LexicalInfo.Empty);
                    string text5 = expression9.Name = "Assembly";
                    TypeofExpression expression26 = expression8 = new TypeofExpression(LexicalInfo.Empty);
                    TypeReference reference1 = expression8.Type = TypeReference.Lift(type);
                    TypeofExpression expression27 = expression9.Target = expression8;
                    e.Items.Add(expression9);
                }
            }
            ClassDefinition definition1 = definition = new ClassDefinition(LexicalInfo.Empty);
            int num1 = (int) (definition.Modifiers = TypeMemberModifiers.Internal);
            string text6 = definition.Name = "EvalAssemblyReferences";
            TypeMember[] memberArray1 = new TypeMember[1];
            Field field1 = field = new Field(LexicalInfo.Empty);
            int num2 = (int) (field.Modifiers = TypeMemberModifiers.Final | TypeMemberModifiers.Static | TypeMemberModifiers.Public);
            string text7 = field.Name = "Value";
            Expression expression28 = field.Initializer = Expression.Lift(e);
            int num3 = (int) (field.IsVolatile = false);
            memberArray1[0] = field;
            TypeMemberCollection collection2 = definition.Members = TypeMemberCollection.FromArray(memberArray1);
            ClassDefinition member = definition;
            My<CodeReifier>.Instance.ReifyInto(this.CompileUnit.Modules[0], member);
            return (IField) member.Members["Value"].Entity;
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
            int num1 = (int) (definition.Modifiers = TypeMemberModifiers.Public);
            string text1 = definition.Name = "StaticEvaluationDomainProvider";
            TypeMember[] items = new TypeMember[3];
            Field field1 = field = new Field(LexicalInfo.Empty);
            int num2 = (int) (field.Modifiers = TypeMemberModifiers.Final | TypeMemberModifiers.Static | TypeMemberModifiers.Public);
            string text2 = field.Name = "Instance";
            MethodInvocationExpression expression1 = expression2 = new MethodInvocationExpression(LexicalInfo.Empty);
            ReferenceExpression expression8 = expression = new ReferenceExpression(LexicalInfo.Empty);
            string text3 = expression.Name = "StaticEvaluationDomainProvider";
            ReferenceExpression expression9 = expression2.Target = expression;
            MethodInvocationExpression expression10 = field.Initializer = expression2;
            int num3 = (int) (field.IsVolatile = false);
            items[0] = field;
            Constructor constructor1 = constructor = new Constructor(LexicalInfo.Empty);
            string text4 = constructor.Name = "constructor";
            Block block1 = block = new Block(LexicalInfo.Empty);
            Statement[] statementArray1 = new Statement[1];
            MethodInvocationExpression expression11 = expression3 = new MethodInvocationExpression(LexicalInfo.Empty);
            SuperLiteralExpression expression12 = expression3.Target = new SuperLiteralExpression(LexicalInfo.Empty);
            Expression[] expressionArray1 = new Expression[] { Expression.Lift(this.ImportsArrayFor(node)) };
            ExpressionCollection collection1 = expression3.Arguments = ExpressionCollection.FromArray(expressionArray1);
            statementArray1[0] = Statement.Lift(expression3);
            StatementCollection collection2 = block.Statements = StatementCollection.FromArray(statementArray1);
            Block block5 = constructor.Body = block;
            items[1] = constructor;
            Method method1 = method = new Method(LexicalInfo.Empty);
            int num4 = (int) (method.Modifiers = TypeMemberModifiers.Override);
            string text5 = method.Name = "GetAssemblyReferences";
            Block block6 = block2 = new Block(LexicalInfo.Empty);
            Statement[] statementArray2 = new Statement[1];
            ReturnStatement statement1 = statement = new ReturnStatement(LexicalInfo.Empty);
            Expression expression13 = statement.Expression = Expression.Lift(this.AssemblyReferencesArray());
            statementArray2[0] = Statement.Lift(statement);
            StatementCollection collection3 = block2.Statements = StatementCollection.FromArray(statementArray2);
            Block block7 = method.Body = block2;
            items[2] = method;
            TypeMemberCollection collection4 = definition.Members = TypeMemberCollection.FromArray(items);
            TypeReference[] referenceArray1 = new TypeReference[] { TypeReference.Lift(typeof(SimpleEvaluationDomainProvider)) };
            TypeReferenceCollection collection5 = definition.BaseTypes = TypeReferenceCollection.FromArray(referenceArray1);
            ClassDefinition member = definition;
            My<CodeReifier>.Instance.ReifyInto(node, member);
            return member.Members["Instance"].Entity;
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
            string[] components = new string[] { "domain" };
            ReferenceExpression expression = new ReferenceExpression(this.Context.GetUniqueName(components));
            ClassDefinition definition1 = definition = new ClassDefinition(LexicalInfo.Empty);
            string text1 = definition.Name = "_";
            TypeMember[] items = new TypeMember[4];
            Field field1 = field = new Field(LexicalInfo.Empty);
            int num1 = (int) (field.Modifiers = TypeMemberModifiers.Private);
            string text2 = field.Name = "$";
            TypeReference reference1 = field.Type = TypeReference.Lift(typeof(EvaluationDomain));
            int num2 = (int) (field.IsVolatile = false);
            string text3 = field.Name = CodeSerializer.LiftName(expression);
            items[0] = field;
            Method method1 = method = new Method(LexicalInfo.Empty);
            int num3 = (int) (method.Modifiers = TypeMemberModifiers.Public);
            string text4 = method.Name = "GetEvaluationDomain";
            Block block1 = block = new Block(LexicalInfo.Empty);
            Statement[] statementArray1 = new Statement[1];
            ReturnStatement statement1 = statement = new ReturnStatement(LexicalInfo.Empty);
            BinaryExpression expression1 = expression4 = new BinaryExpression(LexicalInfo.Empty);
            int num4 = (int) (expression4.Operator = BinaryOperatorType.Or);
            Expression expression13 = expression4.Left = Expression.Lift(expression);
            BinaryExpression expression14 = expression3 = new BinaryExpression(LexicalInfo.Empty);
            int num5 = (int) (expression3.Operator = BinaryOperatorType.Assign);
            Expression expression15 = expression3.Left = Expression.Lift(expression);
            MethodInvocationExpression expression16 = expression2 = new MethodInvocationExpression(LexicalInfo.Empty);
            Expression expression17 = expression2.Target = Expression.Lift(typeof(EvaluationDomain));
            MethodInvocationExpression expression18 = expression3.Right = expression2;
            BinaryExpression expression19 = expression4.Right = expression3;
            BinaryExpression expression20 = statement.Expression = expression4;
            statementArray1[0] = Statement.Lift(statement);
            StatementCollection collection1 = block.Statements = StatementCollection.FromArray(statementArray1);
            Block block7 = method.Body = block;
            items[1] = method;
            Method method4 = method2 = new Method(LexicalInfo.Empty);
            int num6 = (int) (method2.Modifiers = TypeMemberModifiers.Public);
            string text5 = method2.Name = "GetImports";
            Block block8 = block2 = new Block(LexicalInfo.Empty);
            Statement[] statementArray2 = new Statement[1];
            ReturnStatement statement4 = statement2 = new ReturnStatement(LexicalInfo.Empty);
            Expression expression21 = statement2.Expression = Expression.Lift(this.ImportsArrayFor(node));
            statementArray2[0] = Statement.Lift(statement2);
            StatementCollection collection2 = block2.Statements = StatementCollection.FromArray(statementArray2);
            Block block9 = method2.Body = block2;
            items[2] = method2;
            Method method5 = method3 = new Method(LexicalInfo.Empty);
            int num7 = (int) (method3.Modifiers = TypeMemberModifiers.Public);
            string text6 = method3.Name = "GetAssemblyReferences";
            Block block10 = block3 = new Block(LexicalInfo.Empty);
            Statement[] statementArray3 = new Statement[1];
            ReturnStatement statement5 = statement3 = new ReturnStatement(LexicalInfo.Empty);
            Expression expression22 = statement3.Expression = Expression.Lift(this.AssemblyReferencesArray());
            statementArray3[0] = Statement.Lift(statement3);
            StatementCollection collection3 = block3.Statements = StatementCollection.FromArray(statementArray3);
            Block block11 = method3.Body = block3;
            items[3] = method3;
            TypeMemberCollection collection4 = definition.Members = TypeMemberCollection.FromArray(items);
            TypeReference[] referenceArray1 = new TypeReference[] { TypeReference.Lift(typeof(IEvaluationDomainProvider)) };
            TypeReferenceCollection collection5 = definition.BaseTypes = TypeReferenceCollection.FromArray(referenceArray1);
            ClassDefinition mixin = definition;
            My<CodeReifier>.Instance.MergeInto(node, mixin);
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
            int num1 = (int) (reference.IsPointer = false);
            TypeReference reference4 = reference.ElementType = TypeReference.Lift(typeof(string));
            IntegerLiteralExpression expression5 = expression = new IntegerLiteralExpression(LexicalInfo.Empty);
            long num2 = expression.Value = 1L;
            int num3 = (int) (expression.IsLong = false);
            IntegerLiteralExpression expression6 = reference.Rank = expression;
            ArrayTypeReference reference5 = expression2.Type = reference;
            ArrayLiteralExpression expression3 = expression2;
            expression3.Items.AddRange(new $ImportsArrayFor$332(s$));
            return expression3;
        }

        private Set<Assembly> ReferencedAssemblies()
        {
            ReferencedAssemblyCollector visitor = new ReferencedAssemblyCollector();
            this.CompileUnit.Accept(visitor);
            return visitor.ReferencedAssemblies;
        }

        public IField StaticEvaluationDomainProviderFor(ClassDefinition node)
        {
            object obj2 = node[$node$30];
            if (obj2 == null)
            {
            }
            IEntity entity = this.CreateStaticEvaluationDomainProviderReferenceOn(node);
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
                    this.$$enumerator = this.$$locals$333.$node.EnclosingModule.Imports.GetEnumerator();
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

