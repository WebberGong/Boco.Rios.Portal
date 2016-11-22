namespace Castle.DynamicProxy.Builder.CodeBuilder.SimpleAST
{
    using Castle.DynamicProxy.Builder.CodeBuilder;
    using System;
    using System.Reflection.Emit;

    public interface IEmitter
    {
        void Emit(IEasyMember member, ILGenerator gen);
    }
}

