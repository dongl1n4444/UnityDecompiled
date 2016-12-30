namespace UnityScript.Scripting.Pipeline
{
    using Boo.Lang;
    using Boo.Lang.Compiler.TypeSystem;
    using Boo.Lang.Runtime;
    using CompilerGenerated;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityScript.Scripting;

    [Serializable]
    public class EvaluationContextNamespace : INamespace
    {
        protected object[] _activeScripts;
        protected EvaluationContext _context;
        protected INamespace _contextNamespace;
        protected Hash _entityMapping = new Hash();
        protected INamespace _parent;
        protected INamespace _scriptContainerNamespace;
        protected TypeSystemServices _tss;

        internal EvaluationContextEntity $Resolve$closure$212(IEntity entity) => 
            new EvaluationContextEntity((IMember) entity);

        public EvaluationContextNamespace(TypeSystemServices tss, INamespace parent, EvaluationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if (parent == null)
            {
                throw new ArgumentNullException("parent");
            }
            if (tss == null)
            {
                throw new ArgumentNullException("tss");
            }
            this._tss = tss;
            this._parent = parent;
            this._context = context;
            IType type = tss.Map(this._context.GetType());
            this._contextNamespace = type;
            IEntity entity1 = type.get_DeclaringEntity();
            if (entity1 <= null)
            {
            }
            this._scriptContainerNamespace = NullNamespace.Default;
            this._activeScripts = this._context.GetActiveScripts();
        }

        public override IEnumerable<IEntity> GetMembers() => 
            null;

        public object MapEntity(IEntity entity, EntityMapper mapper)
        {
            object obj2 = this._entityMapping[entity];
            return ((obj2 == null) ? obj2 : obj2);
        }

        public override bool Resolve(ICollection<IEntity> targetList, string name, Boo.Lang.Compiler.TypeSystem.EntityType filter)
        {
            EntityMapper mapper;
            IType type;
            int num = 0;
            object[] objArray = this._activeScripts;
            int length = objArray.Length;
            while (num < length)
            {
                mapper = $adaptor$__EvaluationContextNamespace_Resolve$callable15$92_13__$EntityMapper$5.Adapt(new __EvaluationContextNamespace_Resolve$callable15$92_13__(new $Resolve$closure$211(num, this, objArray).Invoke));
                type = this._tss.Map(objArray[num].GetType());
                num++;
            }
            return (!this.Resolve(this._contextNamespace, targetList, name, filter) ? (!this.Resolve(this._scriptContainerNamespace, targetList, name, filter) ? this.Resolve(type, targetList, name, filter, mapper) : true) : true);
        }

        public bool Resolve(INamespace ns, ICollection<IEntity> targetList, string name, Boo.Lang.Compiler.TypeSystem.EntityType filter) => 
            this.Resolve(ns, targetList, name, filter, new EntityMapper(this.$Resolve$closure$212));

        public bool Resolve(INamespace ns, ICollection<IEntity> targetList, string name, Boo.Lang.Compiler.TypeSystem.EntityType filter, EntityMapper mapper)
        {
            // This item is obfuscated and can not be translated.
        }

        public override Boo.Lang.Compiler.TypeSystem.EntityType EntityType =>
            0x1000;

        public override string FullName =>
            this.Name;

        public override string Name =>
            "EvaluationContext";

        public override INamespace ParentNamespace =>
            this._parent;

        [Serializable]
        internal class $Resolve$closure$211
        {
            internal int $$234$276;
            internal object[] $$235$278;
            internal EvaluationContextNamespace $this$277;

            public $Resolve$closure$211(int $$234$276, EvaluationContextNamespace $this$277, object[] $$235$278)
            {
                this.$$234$276 = $$234$276;
                this.$this$277 = $this$277;
                this.$$235$278 = $$235$278;
            }

            public ActiveScriptEntity Invoke(IEntity entity) => 
                new ActiveScriptEntity(this.$this$277._context.GetActiveScriptId(this.$$235$278[this.$$234$276]), (IMember) entity);
        }

        [Serializable, CompilerGenerated]
        public delegate EvaluationContextEntity EntityMapper(IEntity entity);
    }
}

