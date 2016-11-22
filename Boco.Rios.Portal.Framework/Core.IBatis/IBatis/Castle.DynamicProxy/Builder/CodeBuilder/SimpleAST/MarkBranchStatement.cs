namespace Castle.DynamicProxy.Builder.CodeBuilder.SimpleAST
{
    using Castle.DynamicProxy.Builder.CodeBuilder;
    using System;
    using System.Reflection.Emit;

    [CLSCompliant(false)]
    public class MarkBranchStatement : Statement
    {
        private LabelReference _label;

        public MarkBranchStatement(LabelReference label)
        {
            this._label = label;
        }

        public override void Emit(IEasyMember member, ILGenerator gen)
        {
            gen.MarkLabel(this._label.Reference);
        }
    }
}

