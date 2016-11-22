namespace Castle.DynamicProxy.Invocation
{
    using Castle.DynamicProxy;
    using System;
    using System.Reflection;

    [CLSCompliant(true)]
    public class SameClassInvocation : AbstractInvocation
    {
        public SameClassInvocation(ICallable callable, object proxy, MethodInfo method, object newtarget) : base(callable, proxy, method, newtarget)
        {
        }
    }
}

