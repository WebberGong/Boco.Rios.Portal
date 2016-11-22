namespace Castle.DynamicProxy.Builder.CodeBuilder.SimpleAST
{
    using Castle.DynamicProxy.Builder.CodeBuilder;
    using System;
    using System.Reflection.Emit;

    [CLSCompliant(false)]
    public class GotoStatement : Statement
    {
        private LabelReference _label;

        public GotoStatement(LabelReference label)
        {
            this._label = label;
        }

        public override void Emit(IEasyMember member, ILGenerator gen)
        {
            gen.Emit(OpCodes.Br_S, this._label.Reference);
        }
    }
}

