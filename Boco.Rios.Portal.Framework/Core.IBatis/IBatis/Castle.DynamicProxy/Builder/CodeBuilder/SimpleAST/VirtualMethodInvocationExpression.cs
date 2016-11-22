namespace Castle.DynamicProxy.Builder.CodeBuilder.SimpleAST
{
    using Castle.DynamicProxy.Builder.CodeBuilder;
    using Castle.DynamicProxy.Builder.CodeBuilder.Utils;
    using System;
    using System.Reflection;
    using System.Reflection.Emit;

    [CLSCompliant(false)]
    public class VirtualMethodInvocationExpression : MethodInvocationExpression
    {
        public VirtualMethodInvocationExpression(EasyMethod method, params Expression[] args) : base(method, args)
        {
        }

        public VirtualMethodInvocationExpression(MethodInfo method, params Expression[] args) : base(method, args)
        {
        }

        public VirtualMethodInvocationExpression(Reference owner, EasyMethod method, params Expression[] args) : base(owner, method, args)
        {
        }

        public VirtualMethodInvocationExpression(Reference owner, MethodInfo method, params Expression[] args) : base(owner, method, args)
        {
        }

        public override void Emit(IEasyMember member, ILGenerator gen)
        {
            ArgumentsUtil.EmitLoadOwnerAndReference(base._owner, gen);
            foreach (Expression expression in base._args)
            {
                expression.Emit(member, gen);
            }
            gen.Emit(OpCodes.Callvirt, base._method);
        }
    }
}

