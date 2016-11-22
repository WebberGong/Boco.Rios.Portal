namespace Castle.DynamicProxy.Builder.CodeBuilder
{
    using Castle.DynamicProxy.Builder.CodeBuilder.SimpleAST;
    using Castle.DynamicProxy.Builder.CodeBuilder.Utils;
    using System;
    using System.Reflection;

    [CLSCompliant(false)]
    public class EasyRuntimeConstructor : EasyConstructor
    {
        public EasyRuntimeConstructor(AbstractEasyType maintype, params ArgumentReference[] arguments)
        {
            Type[] parameterTypes = ArgumentsUtil.InitializeAndConvert(arguments);
            base._builder = maintype.TypeBuilder.DefineConstructor(MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.Public, CallingConventions.Standard, parameterTypes);
            base._builder.SetImplementationFlags(MethodImplAttributes.CodeTypeMask);
        }

        public override void EnsureValidCodeBlock()
        {
        }

        public override void Generate()
        {
        }
    }
}

