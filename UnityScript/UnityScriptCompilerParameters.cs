namespace UnityScript
{
    using Boo.Lang;
    using Boo.Lang.Compiler;
    using Boo.Lang.Compiler.Services;
    using Boo.Lang.Compiler.TypeSystem;
    using Boo.Lang.Compiler.TypeSystem.Reflection;
    using Boo.Lang.Compiler.TypeSystem.Services;
    using Boo.Lang.Environments;
    using CompilerGenerated;
    using System;
    using UnityScript.Lang;
    using UnityScript.TypeSystem;

    [Serializable]
    public class UnityScriptCompilerParameters : CompilerParameters
    {
        private string $DisableEval$24;
        private bool $Expando$22;
        private bool $GlobalVariablesBecomeFields$23;
        private List<string> $Imports$21;
        private bool $initialized__UnityScript_UnityScriptCompilerParameters$;
        private Type $ScriptBaseType$19;
        private string $ScriptMainMethod$20;
        private int $TabSize$25;
        [NonSerialized]
        public const int DefaultTabSize = 8;

        internal UnityScriptEntityFormatter $constructor$closure$48()
        {
            return new UnityScriptEntityFormatter();
        }

        internal UnityScriptTypeSystem $constructor$closure$49()
        {
            return new UnityScriptTypeSystem();
        }

        internal UnityCallableResolutionService $constructor$closure$50()
        {
            return new UnityCallableResolutionService();
        }

        internal UnityDowncastPermissions $constructor$closure$51()
        {
            return new UnityDowncastPermissions();
        }

        internal UnityScriptAmbiance $constructor$closure$52()
        {
            return new UnityScriptAmbiance();
        }

        public UnityScriptCompilerParameters() : this(true)
        {
            if (!this.$initialized__UnityScript_UnityScriptCompilerParameters$)
            {
                this.$Imports$21 = new List<string>();
                this.$GlobalVariablesBecomeFields$23 = true;
                this.$TabSize$25 = 8;
                this.$initialized__UnityScript_UnityScriptCompilerParameters$ = true;
            }
        }

        public UnityScriptCompilerParameters(bool loadDefaultReferences) : this(new ReflectionTypeSystemProvider(), loadDefaultReferences)
        {
            if (!this.$initialized__UnityScript_UnityScriptCompilerParameters$)
            {
                this.$Imports$21 = new List<string>();
                this.$GlobalVariablesBecomeFields$23 = true;
                this.$TabSize$25 = 8;
                this.$initialized__UnityScript_UnityScriptCompilerParameters$ = true;
            }
        }

        public UnityScriptCompilerParameters(IReflectionTypeSystemProvider reflectionTypeSystemProvider, bool loadDefaultReferences) : base(reflectionTypeSystemProvider, loadDefaultReferences)
        {
            DeferredEnvironment environment;
            if (!this.$initialized__UnityScript_UnityScriptCompilerParameters$)
            {
                this.$Imports$21 = new List<string>();
                this.$GlobalVariablesBecomeFields$23 = true;
                this.$TabSize$25 = 8;
                this.$initialized__UnityScript_UnityScriptCompilerParameters$ = true;
            }
            this.set_Checked(false);
            this.set_OutputType(1);
            DeferredEnvironment environment1 = environment = new DeferredEnvironment();
            environment.Add(typeof(EntityFormatter), $adaptor$__UnityScriptCompilerParameters$callable3$40_30__$ObjectFactory$0.Adapt(new __UnityScriptCompilerParameters$callable3$40_30__(this.$constructor$closure$48)));
            environment.Add(typeof(TypeSystemServices), $adaptor$__UnityScriptCompilerParameters$callable4$41_33__$ObjectFactory$1.Adapt(new __UnityScriptCompilerParameters$callable4$41_33__(this.$constructor$closure$49)));
            environment.Add(typeof(CallableResolutionService), $adaptor$__UnityScriptCompilerParameters$callable5$42_40__$ObjectFactory$2.Adapt(new __UnityScriptCompilerParameters$callable5$42_40__(this.$constructor$closure$50)));
            environment.Add(typeof(DowncastPermissions), $adaptor$__UnityScriptCompilerParameters$callable6$43_34__$ObjectFactory$3.Adapt(new __UnityScriptCompilerParameters$callable6$43_34__(this.$constructor$closure$51)));
            environment.Add(typeof(LanguageAmbiance), $adaptor$__UnityScriptCompilerParameters$callable7$44_31__$ObjectFactory$4.Adapt(new __UnityScriptCompilerParameters$callable7$44_31__(this.$constructor$closure$52)));
            this.set_Environment(environment);
            if (loadDefaultReferences)
            {
                this.get_References().Add(typeof(Array).Assembly);
                this.get_References().Add(this.GetType().Assembly);
            }
        }

        public void AddToEnvironment(Type serviceType, ObjectFactory factory)
        {
            (this.get_Environment() as DeferredEnvironment).Add(serviceType, factory);
        }

        public string DisableEval
        {
            get
            {
                return this.$DisableEval$24;
            }
            set
            {
                this.$DisableEval$24 = value;
            }
        }

        public override bool Ducky
        {
            get
            {
                return !this.get_Strict();
            }
            set
            {
                throw new Exception("Ducky is always equals not Strict. Set Strict instead.");
            }
        }

        public bool Expando
        {
            get
            {
                return this.$Expando$22;
            }
            set
            {
                this.$Expando$22 = value;
            }
        }

        public bool GlobalVariablesBecomeFields
        {
            get
            {
                return this.$GlobalVariablesBecomeFields$23;
            }
            set
            {
                this.$GlobalVariablesBecomeFields$23 = value;
            }
        }

        public List<string> Imports
        {
            get
            {
                return this.$Imports$21;
            }
            set
            {
                this.$Imports$21 = value;
            }
        }

        public Type ScriptBaseType
        {
            get
            {
                return this.$ScriptBaseType$19;
            }
            set
            {
                this.$ScriptBaseType$19 = value;
            }
        }

        public string ScriptMainMethod
        {
            get
            {
                return this.$ScriptMainMethod$20;
            }
            set
            {
                this.$ScriptMainMethod$20 = value;
            }
        }

        public int TabSize
        {
            get
            {
                return this.$TabSize$25;
            }
            set
            {
                this.$TabSize$25 = value;
            }
        }
    }
}

