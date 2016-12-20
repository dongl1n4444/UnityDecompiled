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
            this.get_NameResolutionService().Reset();
            INamespace[] namespaceArray1 = new INamespace[] { this.get_TypeSystemServices().BuiltinsType, (INamespace) this.SafeGetNamespace("UnityScript.Lang"), this.get_TypeSystemServices().Map(typeof(UnityBuiltins)), this.get_TypeSystemServices().Map(typeof(Extensions)) };
            this.get_NameResolutionService().set_GlobalNamespace(new NamespaceDelegator(this.get_NameResolutionService().get_GlobalNamespace(), namespaceArray1));
        }

        public IEntity SafeGetNamespace(string name)
        {
            IEntity entity1 = this.get_NameResolutionService().ResolveQualifiedName(name);
            if (entity1 > null)
            {
                return entity1;
            }
            return NullNamespace.Default;
        }
    }
}

