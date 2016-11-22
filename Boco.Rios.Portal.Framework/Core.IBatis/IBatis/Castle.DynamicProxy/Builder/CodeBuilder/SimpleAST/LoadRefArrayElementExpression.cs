namespace Castle.DynamicProxy.Builder.CodeBuilder.SimpleAST
{
    using Castle.DynamicProxy.Builder.CodeBuilder;
    using Castle.DynamicProxy.Builder.CodeBuilder.Utils;
    using System;
    using System.Reflection.Emit;

    [CLSCompliant(false)]
    public class LoadRefArrayElementExpression : Expression
    {
        private Reference _arrayReference;
        private FixedReference _index;

        public LoadRefArrayElementExpression(FixedReference index, Reference arrayReference)
        {
            this._index = index;
            this._arrayReference = arrayReference;
        }

        public LoadRefArrayElementExpression(int index, Reference arrayReference) : this(new FixedReference(index), arrayReference)
        {
        }

        public override void Emit(IEasyMember member, ILGenerator gen)
        {
            ArgumentsUtil.EmitLoadOwnerAndReference(this._arrayReference, gen);
            ArgumentsUtil.EmitLoadOwnerAndReference(this._index, gen);
            gen.Emit(OpCodes.Ldelem_Ref);
        }
    }
}

