namespace UnityScript.Steps
{
    using Boo.Lang.Compiler.Steps;
    using Boo.Lang.Compiler.TypeSystem;
    using Boo.Lang.Compiler.TypeSystem.Core;
    using System;
    using UnityScript.Lang;

    [Serializable]
    public class IntroduceUnityGlobalNamespaces : AbstractCompilerStep
    {
        public override void Run()
        {
            this.NameResolutionService.Reset();
            INamespace[] namespaces = new INamespace[] { this.TypeSystemServices.BuiltinsType, (INamespace) this.SafeGetNamespace("UnityScript.Lang"), this.TypeSystemServices.Map(typeof(UnityBuiltins)), this.TypeSystemServices.Map(typeof(Extensions)) };
            this.NameResolutionService.GlobalNamespace = new NamespaceDelegator(this.NameResolutionService.GlobalNamespace, namespaces);
        }

        public IEntity SafeGetNamespace(string name)
        {
            IEntity entity1 = this.NameResolutionService.ResolveQualifiedName(name);
            if (entity1 > null)
            {
                return entity1;
            }
            return NullNamespace.Default;
        }
    }
}

