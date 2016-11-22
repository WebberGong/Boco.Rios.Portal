namespace Castle.DynamicProxy.Builder.CodeBuilder
{
    using System;
    using System.Reflection.Emit;

    [CLSCompliant(false)]
    public class MethodCodeBuilder : AbstractCodeBuilder
    {
        private Type _baseType;
        private MethodBuilder _methodbuilder;

        public MethodCodeBuilder(Type baseType, MethodBuilder methodbuilder, ILGenerator generator) : base(generator)
        {
            this._baseType = baseType;
            this._methodbuilder = methodbuilder;
        }

        private MethodBuilder Builder
        {
            get
            {
                return this._methodbuilder;
            }
        }
    }
}

