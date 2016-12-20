namespace UnityEngine.Events
{
    using System;
    using System.Reflection;
    using System.Threading;
    using UnityEngineInternal;

    internal class InvokableCall<T1, T2, T3, T4> : BaseInvokableCall
    {
        protected event UnityAction<T1, T2, T3, T4> Delegate;

        public InvokableCall(UnityAction<T1, T2, T3, T4> action)
        {
            this.Delegate += action;
        }

        public InvokableCall(object target, MethodInfo theFunction) : base(target, theFunction)
        {
            this.Delegate = (UnityAction<T1, T2, T3, T4>) NetFxCoreExtensions.CreateDelegate(theFunction, typeof(UnityAction<T1, T2, T3, T4>), target);
        }

        public override bool Find(object targetObj, MethodInfo method)
        {
            return ((this.Delegate.Target == targetObj) && NetFxCoreExtensions.GetMethodInfo(this.Delegate).Equals(method));
        }

        public override void Invoke(object[] args)
        {
            if (args.Length != 4)
            {
                throw new ArgumentException("Passed argument 'args' is invalid size. Expected size is 1");
            }
            BaseInvokableCall.ThrowOnInvalidArg<T1>(args[0]);
            BaseInvokableCall.ThrowOnInvalidArg<T2>(args[1]);
            BaseInvokableCall.ThrowOnInvalidArg<T3>(args[2]);
            BaseInvokableCall.ThrowOnInvalidArg<T4>(args[3]);
            if (BaseInvokableCall.AllowInvoke(this.Delegate))
            {
                this.Delegate((T1) args[0], (T2) args[1], (T3) args[2], (T4) args[3]);
            }
        }
    }
}

