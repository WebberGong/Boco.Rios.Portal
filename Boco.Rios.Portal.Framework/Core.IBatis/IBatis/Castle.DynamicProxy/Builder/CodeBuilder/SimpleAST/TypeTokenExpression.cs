namespace Castle.DynamicProxy.Builder.CodeBuilder.SimpleAST
{
    using Castle.DynamicProxy.Builder.CodeBuilder;
    using System;
    using System.Reflection.Emit;

    [CLSCompliant(false)]
    public class TypeTokenExpression : Expression
    {
        private Type _type;

        public TypeTokenExpression(Type type)
        {
            this._type = type;
        }

        public override void Emit(IEasyMember member, ILGenerator gen)
        {
            gen.Emit(OpCodes.Ldtoken, this._type);
            gen.Emit(OpCodes.Call, typeof(Type).GetMethod("GetTypeFromHandle"));
        }
    }
}

