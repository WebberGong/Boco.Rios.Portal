namespace Castle.DynamicProxy.Builder.CodeBuilder.SimpleAST
{
    using Castle.DynamicProxy.Builder.CodeBuilder;
    using Castle.DynamicProxy.Builder.CodeBuilder.Utils;
    using System;
    using System.Reflection;
    using System.Reflection.Emit;

    [CLSCompliant(false)]
    public class MethodInvocationExpression : Expression
    {
        protected Expression[] _args;
        protected MethodInfo _method;
        protected Reference _owner;

        public MethodInvocationExpression(EasyMethod method, params Expression[] args) : this(SelfReference.Self, method.MethodBuilder, args)
        {
        }

        public MethodInvocationExpression(MethodInfo method, params Expression[] args) : this(SelfReference.Self, method, args)
        {
        }

        public MethodInvocationExpression(Reference owner, EasyMethod method, params Expression[] args) : this(owner, method.MethodBuilder, args)
        {
        }

        public MethodInvocationExpression(Reference owner, MethodInfo method, params Expression[] args)
        {
            this._owner = owner;
            this._method = method;
            this._args = args;
        }

        public override void Emit(IEasyMember member, ILGenerator gen)
        {
            ArgumentsUtil.EmitLoadOwnerAndReference(this._owner, gen);
            foreach (Expression expression in this._args)
            {
                expression.Emit(member, gen);
            }
            gen.Emit(OpCodes.Call, this._method);
        }
    }
}

