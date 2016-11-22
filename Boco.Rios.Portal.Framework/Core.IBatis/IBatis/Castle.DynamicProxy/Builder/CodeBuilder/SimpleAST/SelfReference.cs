namespace Castle.DynamicProxy.Builder.CodeBuilder.SimpleAST
{
    using System;
    using System.Reflection.Emit;

    [CLSCompliant(false)]
    public class SelfReference : Reference
    {
        public static readonly SelfReference Self = new SelfReference();

        protected SelfReference() : base(null)
        {
        }

        public override void LoadAddressOfReference(ILGenerator gen)
        {
            throw new NotSupportedException();
        }

        public override void LoadReference(ILGenerator gen)
        {
            gen.Emit(OpCodes.Ldarg_0);
        }

        public override void StoreReference(ILGenerator gen)
        {
            gen.Emit(OpCodes.Ldarg_0);
        }
    }
}

