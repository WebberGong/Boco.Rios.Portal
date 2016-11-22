namespace Castle.DynamicProxy.Builder.CodeBuilder.SimpleAST
{
    using Castle.DynamicProxy.Builder.CodeBuilder;
    using Castle.DynamicProxy.Builder.CodeBuilder.Utils;
    using System;
    using System.Reflection.Emit;

    [CLSCompliant(false)]
    public class ConvertExpression : Expression
    {
        private Type _fromType;
        private Expression _right;
        private Type _target;

        public ConvertExpression(Type targetType, Expression right) : this(targetType, typeof(object), right)
        {
        }

        public ConvertExpression(Type targetType, Type fromType, Expression right)
        {
            this._target = targetType;
            this._fromType = fromType;
            this._right = right;
        }

        public override void Emit(IEasyMember member, ILGenerator gen)
        {
            this._right.Emit(member, gen);
            if (this._fromType != this._target)
            {
                if (this._fromType.IsByRef)
                {
                    throw new NotSupportedException("Cannot convert from ByRef types");
                }
                if (this._target.IsByRef)
                {
                    throw new NotSupportedException("Cannot convert to ByRef types");
                }
                if (this._target.IsValueType)
                {
                    if (this._fromType.IsValueType)
                    {
                        throw new NotImplementedException("Cannot convert between distinct value types at the moment");
                    }
                    gen.Emit(OpCodes.Unbox, this._target);
                    OpCodeUtil.EmitLoadIndirectOpCodeForType(gen, this._target);
                }
                else if (this._fromType.IsValueType)
                {
                    gen.Emit(OpCodes.Box, this._fromType);
                    this.EmitCastIfNeeded(typeof(object), this._target, gen);
                }
                else
                {
                    this.EmitCastIfNeeded(this._fromType, this._target, gen);
                }
            }
        }

        private void EmitCastIfNeeded(Type from, Type target, ILGenerator gen)
        {
            if (target.IsSubclassOf(from))
            {
                gen.Emit(OpCodes.Castclass, target);
            }
        }
    }
}

