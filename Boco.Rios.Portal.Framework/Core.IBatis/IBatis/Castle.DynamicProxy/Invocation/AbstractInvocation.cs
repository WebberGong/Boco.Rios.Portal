namespace Castle.DynamicProxy.Invocation
{
    using Castle.DynamicProxy;
    using System;
    using System.Reflection;

    [CLSCompliant(true)]
    public abstract class AbstractInvocation : IInvocation
    {
        protected ICallable callable;
        protected object changed_target;
        private MethodInfo method;
        private object proxy;
        private object target;

        public AbstractInvocation(ICallable callable, object proxy, MethodInfo method, object newtarget)
        {
            this.callable = callable;
            this.proxy = proxy;
            this.target = callable.Target;
            if (newtarget != null)
            {
                this.target = newtarget;
            }
            this.method = method;
        }

        public virtual object Proceed(params object[] args)
        {
            if (this.changed_target == null)
            {
                return this.callable.Call(args);
            }
            return this.Method.Invoke(this.changed_target, args);
        }

        public object InvocationTarget
        {
            get
            {
                if (this.changed_target == null)
                {
                    return this.target;
                }
                return this.changed_target;
            }
            set
            {
                this.changed_target = value;
            }
        }

        public MethodInfo Method
        {
            get
            {
                return this.method;
            }
        }

        public MethodInfo MethodInvocationTarget
        {
            get
            {
                return this.Method;
            }
        }

        public object Proxy
        {
            get
            {
                return this.proxy;
            }
        }
    }
}

