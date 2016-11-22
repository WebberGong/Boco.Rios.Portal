namespace Castle.DynamicProxy.Builder.CodeBuilder.SimpleAST
{
    using Castle.DynamicProxy.Builder.CodeBuilder;
    using System;
    using System.Reflection.Emit;

    [CLSCompliant(false)]
    public class NewArrayExpression : Expression
    {
        private Type _arrayType;
        private int _size;

        public NewArrayExpression(int size, Type arrayType)
        {
            this._size = size;
            this._arrayType = arrayType;
        }

        public override void Emit(IEasyMember member, ILGenerator gen)
        {
            gen.Emit(OpCodes.Ldc_I4, this._size);
            gen.Emit(OpCodes.Newarr, this._arrayType);
        }
    }
}

