namespace Castle.DynamicProxy.Builder.CodeBuilder.SimpleAST
{
    using Castle.DynamicProxy.Builder.CodeBuilder.Utils;
    using System;
    using System.Reflection.Emit;

    [CLSCompliant(false)]
    public class IndirectReference : TypeReference
    {
        public IndirectReference(TypeReference byRefReference) : base(byRefReference, byRefReference.Type.GetElementType())
        {
            if (!byRefReference.Type.IsByRef)
            {
                throw new ArgumentException("Expected a reference whose type IsByRef", "byRefReference");
            }
        }

        public override void LoadAddressOfReference(ILGenerator gen)
        {
        }

        public override void LoadReference(ILGenerator gen)
        {
            OpCodeUtil.EmitLoadIndirectOpCodeForType(gen, base.Type);
        }

        public override void StoreReference(ILGenerator gen)
        {
            OpCodeUtil.EmitStoreIndirectOpCodeForType(gen, base.Type);
        }

        public static TypeReference WrapIfByRef(TypeReference reference)
        {
            if (!reference.Type.IsByRef)
            {
                return reference;
            }
            return new IndirectReference(reference);
        }

        public static TypeReference[] WrapIfByRef(TypeReference[] references)
        {
            TypeReference[] referenceArray = new TypeReference[references.Length];
            for (int i = 0; i < references.Length; i++)
            {
                referenceArray[i] = WrapIfByRef(references[i]);
            }
            return referenceArray;
        }
    }
}

