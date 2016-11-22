namespace Castle.DynamicProxy.Builder.CodeBuilder.SimpleAST
{
    using System;
    using System.Reflection.Emit;

    [CLSCompliant(false)]
    public class LabelReference : Castle.DynamicProxy.Builder.CodeBuilder.SimpleAST.Reference
    {
        private Label _label;

        public override void Generate(ILGenerator gen)
        {
            this._label = gen.DefineLabel();
        }

        public override void LoadAddressOfReference(ILGenerator gen)
        {
            throw new NotSupportedException();
        }

        public override void LoadReference(ILGenerator gen)
        {
            gen.MarkLabel(this._label);
        }

        public override void StoreReference(ILGenerator gen)
        {
            throw new NotImplementedException();
        }

        public override Expression ToExpression()
        {
            throw new NotImplementedException();
        }

        public Label Reference
        {
            get
            {
                return this._label;
            }
        }
    }
}

