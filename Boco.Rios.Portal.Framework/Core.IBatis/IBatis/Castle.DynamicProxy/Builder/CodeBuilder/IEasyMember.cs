namespace Castle.DynamicProxy.Builder.CodeBuilder
{
    using System;
    using System.Reflection;

    public interface IEasyMember
    {
        void EnsureValidCodeBlock();
        void Generate();

        MethodBase Member { get; }

        Type ReturnType { get; }
    }
}

