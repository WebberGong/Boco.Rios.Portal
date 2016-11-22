namespace Castle.DynamicProxy.Builder.CodeBuilder.SimpleAST
{
    using Castle.DynamicProxy.Builder.CodeBuilder.Utils;
    using System;
    using System.Reflection.Emit;

    [CLSCompliant(false)]
    public class FixedReference : TypeReference
    {
        private object _value;

        public FixedReference(object value) : base(value.GetType())
        {
            if (!value.GetType().IsPrimitive && !(value is string))
            {
                throw new ApplicationException("Invalid type to FixedReference");
            }
            this._value = value;
        }

        public override void Generate(ILGenerator gen)
        {
        }

        public override void LoadAddressOfReference(ILGenerator gen)
        {
            throw new NotSupportedException();
        }

        public override void LoadReference(ILGenerator gen)
        {
            OpCodeUtil.EmitLoadOpCodeForConstantValue(gen, this._value);
        }

        public override void StoreReference(ILGenerator gen)
        {
            throw new NotImplementedException("FixedReference.StoreReference");
        }
    }
}

