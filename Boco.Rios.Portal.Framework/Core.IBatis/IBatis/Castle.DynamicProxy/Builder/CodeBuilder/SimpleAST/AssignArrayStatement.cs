namespace Castle.DynamicProxy.Builder.CodeBuilder.SimpleAST
{
    using Castle.DynamicProxy.Builder.CodeBuilder;
    using Castle.DynamicProxy.Builder.CodeBuilder.Utils;
    using System;
    using System.Reflection.Emit;

    [CLSCompliant(false)]
    public class AssignArrayStatement : Statement
    {
        private Reference _targetArray;
        private int _targetPosition;
        private Expression _value;

        public AssignArrayStatement(Reference targetArray, int targetPosition, Expression value)
        {
            this._targetArray = targetArray;
            this._targetPosition = targetPosition;
            this._value = value;
        }

        public override void Emit(IEasyMember member, ILGenerator il)
        {
            ArgumentsUtil.EmitLoadOwnerAndReference(this._targetArray, il);
            il.Emit(OpCodes.Ldc_I4, this._targetPosition);
            this._value.Emit(member, il);
            il.Emit(OpCodes.Stelem_Ref);
        }
    }
}

