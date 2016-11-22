namespace Castle.DynamicProxy.Builder.CodeBuilder
{
    using Castle.DynamicProxy.Builder.CodeBuilder.SimpleAST;
    using Castle.DynamicProxy.Builder.CodeBuilder.Utils;
    using System;
    using System.Reflection;
    using System.Reflection.Emit;

    [CLSCompliant(false)]
    public class ConstructorCodeBuilder : AbstractCodeBuilder
    {
        private Type _baseType;

        public ConstructorCodeBuilder(Type baseType, ILGenerator generator) : base(generator)
        {
            this._baseType = baseType;
        }

        public void InvokeBaseConstructor()
        {
            this.InvokeBaseConstructor(this.ObtainAvailableConstructor());
        }

        internal void InvokeBaseConstructor(ConstructorInfo constructor)
        {
            base.AddStatement(new ExpressionStatement(new ConstructorInvocationExpression(constructor, new Expression[0])));
        }

        internal void InvokeBaseConstructor(ConstructorInfo constructor, params ArgumentReference[] arguments)
        {
            base.AddStatement(new ExpressionStatement(new ConstructorInvocationExpression(constructor, ArgumentsUtil.ConvertArgumentReferenceToExpression(arguments))));
        }

        internal ConstructorInfo ObtainAvailableConstructor()
        {
            return this._baseType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, new Type[0], null);
        }
    }
}

