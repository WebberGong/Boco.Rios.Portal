namespace Castle.DynamicProxy.Builder.CodeBuilder.SimpleAST
{
    using Castle.DynamicProxy.Builder.CodeBuilder;
    using Castle.DynamicProxy.Builder.CodeBuilder.Utils;
    using System;
    using System.Reflection.Emit;

    [CLSCompliant(false)]
    public class ReturnStatement : Statement
    {
        private Expression _expression;
        private Reference _reference;

        public ReturnStatement()
        {
        }

        public ReturnStatement(Expression expression)
        {
            this._expression = expression;
        }

        public ReturnStatement(Reference reference)
        {
            this._reference = reference;
        }

        public override void Emit(IEasyMember member, ILGenerator gen)
        {
            if (this._reference != null)
            {
                ArgumentsUtil.EmitLoadOwnerAndReference(this._reference, gen);
            }
            else if (this._expression != null)
            {
                this._expression.Emit(member, gen);
            }
            else if (member.ReturnType != typeof(void))
            {
                OpCodeUtil.EmitLoadOpCodeForDefaultValueOfType(gen, member.ReturnType);
            }
            gen.Emit(OpCodes.Ret);
        }
    }
}

