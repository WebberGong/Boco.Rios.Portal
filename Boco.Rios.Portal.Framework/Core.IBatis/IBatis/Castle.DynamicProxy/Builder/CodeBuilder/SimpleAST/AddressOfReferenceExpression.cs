namespace Castle.DynamicProxy.Builder.CodeBuilder.SimpleAST
{
    using Castle.DynamicProxy.Builder.CodeBuilder;
    using Castle.DynamicProxy.Builder.CodeBuilder.Utils;
    using System;
    using System.Reflection.Emit;

    [CLSCompliant(false)]
    public class AddressOfReferenceExpression : Expression
    {
        private Reference _reference;

        public AddressOfReferenceExpression(Reference reference)
        {
            this._reference = reference;
        }

        public override void Emit(IEasyMember member, ILGenerator gen)
        {
            ArgumentsUtil.EmitLoadOwnerAndReference(this._reference.OwnerReference, gen);
            this._reference.LoadAddressOfReference(gen);
        }
    }
}

