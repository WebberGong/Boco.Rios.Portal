namespace Castle.DynamicProxy.Builder.CodeBuilder.SimpleAST
{
    using Castle.DynamicProxy.Builder.CodeBuilder;
    using System;
    using System.Reflection;
    using System.Reflection.Emit;

    [CLSCompliant(false)]
    public class ConstructorInvocationExpression : Expression
    {
        private Expression[] _args;
        private ConstructorInfo _cmethod;

        public ConstructorInvocationExpression(ConstructorInfo method, params Expression[] args)
        {
            this._cmethod = method;
            this._args = args;
        }

        public override void Emit(IEasyMember member, ILGenerator gen)
        {
            gen.Emit(OpCodes.Ldarg_0);
            foreach (Expression expression in this._args)
            {
                expression.Emit(member, gen);
            }
            gen.Emit(OpCodes.Call, this._cmethod);
        }
    }
}

