namespace Castle.DynamicProxy.Builder.CodeBuilder.SimpleAST
{
    using Castle.DynamicProxy.Builder.CodeBuilder;
    using System;
    using System.Collections;
    using System.Reflection.Emit;

    [CLSCompliant(false)]
    public class ConditionExpression : Expression
    {
        private ArrayList _falseStmts;
        private Expression _left;
        private OpCode _operation;
        private Expression _right;
        private ArrayList _trueStmts;

        public ConditionExpression(Expression left) : this(OpCodes.Brfalse_S, left)
        {
        }

        public ConditionExpression(OpCode operation, Expression left) : this(operation, left, null)
        {
        }

        public ConditionExpression(OpCode operation, Expression left, Expression right)
        {
            this._operation = OpCodes.Brfalse_S;
            this._trueStmts = new ArrayList();
            this._falseStmts = new ArrayList();
            this._operation = operation;
            this._left = left;
            this._right = right;
        }

        public void AddFalseStatement(Statement stmt)
        {
            this._falseStmts.Add(stmt);
        }

        public void AddTrueStatement(Statement stmt)
        {
            this._trueStmts.Add(stmt);
        }

        public override void Emit(IEasyMember member, ILGenerator gen)
        {
            if ((OpCodes.Brfalse.Equals(this._operation) || OpCodes.Brfalse_S.Equals(this._operation)) || (OpCodes.Brtrue.Equals(this._operation) || OpCodes.Brtrue_S.Equals(this._operation)))
            {
                this._left.Emit(member, gen);
            }
            else
            {
                this._left.Emit(member, gen);
                this._right.Emit(member, gen);
            }
            Label label = gen.DefineLabel();
            Label label2 = gen.DefineLabel();
            gen.Emit(this._operation, label);
            if (this._falseStmts.Count != 0)
            {
                foreach (Statement statement in this._falseStmts)
                {
                    statement.Emit(member, gen);
                }
            }
            gen.Emit(OpCodes.Br_S, label2);
            gen.MarkLabel(label);
            if (this._trueStmts.Count != 0)
            {
                foreach (Statement statement2 in this._trueStmts)
                {
                    statement2.Emit(member, gen);
                }
            }
            gen.MarkLabel(label2);
        }
    }
}

