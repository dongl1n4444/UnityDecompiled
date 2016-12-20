namespace UnityEngine.Events
{
    using System;
    using System.Reflection;
    using System.Threading;
    using UnityEngineInternal;

    internal class InvokableCall<T1, T2> : BaseInvokableCall
    {
        protected event UnityAction<T1, T2> Delegate;

        public InvokableCall(UnityAction<T1, T2> action)
        {
            this.Delegate += action;
        }

        public InvokableCall(object target, MethodInfo theFunction) : base(target, theFunction)
        {
            this.Delegate = (UnityAction<T1, T2>) NetFxCoreExtensions.CreateDelegate(theFunction, typeof(UnityAction<T1, T2>), target);
        }

        public override bool Find(object targetObj, MethodInfo method)
        {
            return ((this.Delegate.Target == targetObj) && NetFxCoreExtensions.GetMethodInfo(this.Delegate).Equals(method));
        }

        public override void Invoke(object[] args)
        {
            if (args.Length != 2)
            {
                throw new ArgumentException("Passed argument 'args' is invalid size. Expected size is 1");
            }
            BaseInvokableCall.ThrowOnInvalidArg<T1>(args[0]);
            BaseInvokableCall.ThrowOnInvalidArg<T2>(args[1]);
            if (BaseInvokableCall.AllowInvoke(this.Delegate))
            {
                this.Delegate((T1) args[0], (T2) args[1]);
            }
        }
    }
}

