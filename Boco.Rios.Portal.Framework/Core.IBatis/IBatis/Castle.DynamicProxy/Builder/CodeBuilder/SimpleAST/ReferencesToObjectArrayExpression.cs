namespace Castle.DynamicProxy.Builder.CodeBuilder.SimpleAST
{
    using Castle.DynamicProxy.Builder.CodeBuilder;
    using Castle.DynamicProxy.Builder.CodeBuilder.Utils;
    using System;
    using System.Reflection.Emit;

    [CLSCompliant(false)]
    public class ReferencesToObjectArrayExpression : Expression
    {
        private TypeReference[] _args;

        public ReferencesToObjectArrayExpression(params TypeReference[] args)
        {
            this._args = args;
        }

        public override void Emit(IEasyMember member, ILGenerator gen)
        {
            LocalBuilder local = gen.DeclareLocal(typeof(object[]));
            gen.Emit(OpCodes.Ldc_I4, this._args.Length);
            gen.Emit(OpCodes.Newarr, typeof(object));
            gen.Emit(OpCodes.Stloc, local);
            for (int i = 0; i < this._args.Length; i++)
            {
                gen.Emit(OpCodes.Ldloc, local);
                gen.Emit(OpCodes.Ldc_I4, i);
                TypeReference reference = this._args[i];
                ArgumentsUtil.EmitLoadOwnerAndReference(reference, gen);
                if (reference.Type.IsByRef)
                {
                    throw new NotSupportedException();
                }
                if (reference.Type.IsValueType)
                {
                    gen.Emit(OpCodes.Box, reference.Type.UnderlyingSystemType);
                }
                gen.Emit(OpCodes.Stelem_Ref);
            }
            gen.Emit(OpCodes.Ldloc, local);
        }
    }
}

