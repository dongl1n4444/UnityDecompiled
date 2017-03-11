namespace UnityScript.Steps
{
    using Boo.Lang.Compiler.Ast;
    using Boo.Lang.Compiler.TypeSystem.Reflection;
    using Boo.Lang.Compiler.Util;
    using System;

    [Serializable]
    public class ReferencedAssemblyCollector : DepthFirstVisitor
    {
        protected Set<Assembly> _assemblies = new Set<Assembly>();

        public void CheckTypeReference(Node node)
        {
            ExternalType entity = node.Entity as ExternalType;
            if (entity != null)
            {
                this._assemblies.Add(entity.ActualType.Assembly);
            }
        }

        public override void LeaveMemberReferenceExpression(MemberReferenceExpression node)
        {
            this.CheckTypeReference(node);
        }

        public override void OnReferenceExpression(ReferenceExpression node)
        {
            this.CheckTypeReference(node);
        }

        public override void OnSimpleTypeReference(SimpleTypeReference node)
        {
            this.CheckTypeReference(node);
        }

        public Set<Assembly> ReferencedAssemblies =>
            this._assemblies;
    }
}

