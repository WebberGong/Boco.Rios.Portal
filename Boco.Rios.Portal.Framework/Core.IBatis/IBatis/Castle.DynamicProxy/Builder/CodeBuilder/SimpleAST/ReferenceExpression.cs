namespace Castle.DynamicProxy.Builder.CodeBuilder.SimpleAST
{
    using Castle.DynamicProxy.Builder.CodeBuilder;
    using Castle.DynamicProxy.Builder.CodeBuilder.Utils;
    using System;
    using System.Reflection.Emit;

    [CLSCompliant(false)]
    public class ReferenceExpression : Expression
    {
        private Reference _reference;

        public ReferenceExpression(Reference reference)
        {
            this._reference = reference;
        }

        public override void Emit(IEasyMember member, ILGenerator gen)
        {
            ArgumentsUtil.EmitLoadOwnerAndReference(this._reference, gen);
        }
    }
}

