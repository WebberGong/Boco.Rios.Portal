namespace Castle.DynamicProxy
{
    using System;

    [CLSCompliant(true)]
    public interface ICallable
    {
        object Call(params object[] args);

        object Target { get; }
    }
}

