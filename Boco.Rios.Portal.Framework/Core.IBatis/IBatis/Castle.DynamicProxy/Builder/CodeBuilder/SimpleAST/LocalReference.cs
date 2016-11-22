namespace Castle.DynamicProxy.Builder.CodeBuilder.SimpleAST
{
    using System;
    using System.Reflection.Emit;

    [CLSCompliant(false)]
    public class LocalReference : TypeReference
    {
        private LocalBuilder _localbuilder;

        public LocalReference(Type type) : base(type)
        {
        }

        public override void Generate(ILGenerator gen)
        {
            this._localbuilder = gen.DeclareLocal(base.Type);
        }

        public override void LoadAddressOfReference(ILGenerator gen)
        {
            gen.Emit(OpCodes.Ldloca, this._localbuilder);
        }

        public override void LoadReference(ILGenerator gen)
        {
            gen.Emit(OpCodes.Ldloc, this._localbuilder);
        }

        public override void StoreReference(ILGenerator gen)
        {
            gen.Emit(OpCodes.Stloc, this._localbuilder);
        }
    }
}

