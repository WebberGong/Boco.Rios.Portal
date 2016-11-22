namespace Castle.DynamicProxy.Builder.CodeBuilder.SimpleAST
{
    using System;
    using System.Reflection.Emit;

    [CLSCompliant(false)]
    public class ArgumentReference : TypeReference
    {
        private int _position;

        public ArgumentReference(Type argumentType) : base(argumentType)
        {
            this._position = -1;
        }

        public override void LoadAddressOfReference(ILGenerator gen)
        {
            throw new NotSupportedException();
        }

        public override void LoadReference(ILGenerator gen)
        {
            switch (this.Position)
            {
                case 0:
                    gen.Emit(OpCodes.Ldarg_0);
                    return;

                case 1:
                    gen.Emit(OpCodes.Ldarg_1);
                    return;

                case 2:
                    gen.Emit(OpCodes.Ldarg_2);
                    return;

                case 3:
                    gen.Emit(OpCodes.Ldarg_3);
                    return;

                case -1:
                    throw new ApplicationException("ArgumentReference unitialized");
            }
            gen.Emit(OpCodes.Ldarg, this.Position);
        }

        public override void StoreReference(ILGenerator gen)
        {
            throw new NotImplementedException();
        }

        internal int Position
        {
            get
            {
                return this._position;
            }
            set
            {
                this._position = value;
            }
        }
    }
}

