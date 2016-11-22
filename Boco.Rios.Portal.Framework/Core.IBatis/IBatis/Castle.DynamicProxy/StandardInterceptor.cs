namespace Castle.DynamicProxy
{
    using System;

    [Serializable, CLSCompliant(true)]
    public class StandardInterceptor : IInterceptor
    {
        public virtual object Intercept(IInvocation invocation, params object[] args)
        {
            this.PreProceed(invocation, args);
            object returnValue = invocation.Proceed(args);
            this.PostProceed(invocation, ref returnValue, args);
            return returnValue;
        }

        protected virtual void PostProceed(IInvocation invocation, ref object returnValue, params object[] args)
        {
        }

        protected virtual void PreProceed(IInvocation invocation, params object[] args)
        {
        }
    }
}

