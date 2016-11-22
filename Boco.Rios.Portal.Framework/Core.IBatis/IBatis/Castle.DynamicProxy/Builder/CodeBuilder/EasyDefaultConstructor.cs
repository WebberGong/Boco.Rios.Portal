namespace Castle.DynamicProxy.Builder.CodeBuilder
{
    using System;
    using System.Reflection;

    [CLSCompliant(false)]
    public class EasyDefaultConstructor : EasyConstructor
    {
        internal EasyDefaultConstructor(AbstractEasyType maintype)
        {
            maintype.TypeBuilder.DefineDefaultConstructor(MethodAttributes.Public);
        }

        public override void EnsureValidCodeBlock()
        {
        }

        public override void Generate()
        {
        }
    }
}

