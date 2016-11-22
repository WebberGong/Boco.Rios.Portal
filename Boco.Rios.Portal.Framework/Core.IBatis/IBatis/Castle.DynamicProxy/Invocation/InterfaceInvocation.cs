namespace Castle.DynamicProxy.Invocation
{
    using Castle.DynamicProxy;
    using System;
    using System.Reflection;

    [CLSCompliant(true)]
    public class InterfaceInvocation : AbstractInvocation
    {
        public InterfaceInvocation(ICallable callable, object proxy, MethodInfo method, object newtarget) : base(callable, proxy, method, newtarget)
        {
        }
    }
}

