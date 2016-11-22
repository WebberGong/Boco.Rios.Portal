namespace Castle.DynamicProxy.Builder.CodeBuilder.SimpleAST
{
    using Castle.DynamicProxy.Builder.CodeBuilder;
    using System;
    using System.Reflection.Emit;

    [CLSCompliant(false)]
    public class NullExpression : Expression
    {
        public static readonly NullExpression Instance = new NullExpression();

        protected NullExpression()
        {
        }

        public override void Emit(IEasyMember member, ILGenerator gen)
        {
            gen.Emit(OpCodes.Ldnull);
        }
    }
}

