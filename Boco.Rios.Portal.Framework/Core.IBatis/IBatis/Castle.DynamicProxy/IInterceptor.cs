namespace Castle.DynamicProxy
{
    using System;

    public interface IInterceptor
    {
        object Intercept(IInvocation invocation, params object[] args);
    }
}

