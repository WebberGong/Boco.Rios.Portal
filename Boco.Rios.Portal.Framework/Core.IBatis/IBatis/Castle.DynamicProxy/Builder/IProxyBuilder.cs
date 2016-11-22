namespace Castle.DynamicProxy.Builder
{
    using Castle.DynamicProxy;
    using System;

    public interface IProxyBuilder
    {
        Type CreateClassProxy(Type theClass);
        Type CreateClassProxy(Type theClass, Type[] interfaces);
        Type CreateCustomClassProxy(Type theClass, GeneratorContext context);
        Type CreateCustomInterfaceProxy(Type[] interfaces, Type target, GeneratorContext context);
        Type CreateInterfaceProxy(Type[] interfaces, Type target);
    }
}

