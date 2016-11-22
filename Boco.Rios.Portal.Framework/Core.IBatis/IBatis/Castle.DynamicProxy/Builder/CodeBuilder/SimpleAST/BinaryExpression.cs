namespace Castle.DynamicProxy.Builder.CodeBuilder.SimpleAST
{
    using Castle.DynamicProxy.Builder.CodeBuilder;
    using System;
    using System.Reflection.Emit;

    [CLSCompliant(false)]
    public class BinaryExpression : Expression
    {
        private Expression _left;
        private OpCode _operation;
        private Expression _right;
        public static readonly OpCode Add = OpCodes.Add;

        public BinaryExpression(OpCode operation, Expression left, Expression right)
        {
            this._operation = operation;
            this._left = left;
            this._right = right;
        }

        public override void Emit(IEasyMember member, ILGenerator gen)
        {
            this._left.Emit(member, gen);
            this._right.Emit(member, gen);
            gen.Emit(this._operation);
        }
    }
}

