namespace Castle.DynamicProxy
{
    using System;
    using System.Reflection;

    public interface IInvocation
    {
        object Proceed(params object[] args);

        object InvocationTarget { get; set; }

        MethodInfo Method { get; }

        MethodInfo MethodInvocationTarget { get; }

        object Proxy { get; }
    }
}

