namespace Castle.DynamicProxy.Builder.CodeBuilder.SimpleAST
{
    using Castle.DynamicProxy.Builder.CodeBuilder;
    using System;
    using System.Reflection.Emit;

    [CLSCompliant(false)]
    public class NopStatement : Statement
    {
        public override void Emit(IEasyMember member, ILGenerator gen)
        {
            gen.Emit(OpCodes.Nop);
        }
    }
}

