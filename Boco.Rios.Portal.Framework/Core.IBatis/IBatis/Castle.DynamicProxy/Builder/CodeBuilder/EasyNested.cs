namespace Castle.DynamicProxy.Builder.CodeBuilder
{
    using Castle.DynamicProxy.Builder.CodeBuilder.SimpleAST;
    using System;
    using System.Reflection;

    [CLSCompliant(false)]
    public class EasyNested : AbstractEasyType
    {
        public EasyNested(AbstractEasyType maintype, string name, Type baseType, Type[] interfaces, ReturnReferenceExpression returnType, params ArgumentReference[] args)
        {
            base._typebuilder = maintype.TypeBuilder.DefineNestedType(name, TypeAttributes.Sealed | TypeAttributes.NestedPublic, baseType, interfaces);
        }
    }
}

