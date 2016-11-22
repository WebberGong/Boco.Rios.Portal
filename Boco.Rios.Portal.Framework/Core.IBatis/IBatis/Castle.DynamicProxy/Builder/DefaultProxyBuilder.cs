namespace Castle.DynamicProxy.Builder
{
    using Castle.DynamicProxy;
    using Castle.DynamicProxy.Builder.CodeGenerators;
    using System;

    [CLSCompliant(false)]
    public class DefaultProxyBuilder : IProxyBuilder
    {
        private Castle.DynamicProxy.Builder.CodeGenerators.ModuleScope _scope = new Castle.DynamicProxy.Builder.CodeGenerators.ModuleScope();

        public virtual Type CreateClassProxy(Type theClass)
        {
            ClassProxyGenerator generator = new ClassProxyGenerator(this._scope);
            return generator.GenerateCode(theClass);
        }

        public Type CreateClassProxy(Type theClass, Type[] interfaces)
        {
            ClassProxyGenerator generator = new ClassProxyGenerator(this._scope);
            return generator.GenerateCode(theClass, interfaces);
        }

        public virtual Type CreateCustomClassProxy(Type theClass, GeneratorContext context)
        {
            ClassProxyGenerator generator = new ClassProxyGenerator(this._scope, context);
            return generator.GenerateCustomCode(theClass, null);
        }

        public virtual Type CreateCustomInterfaceProxy(Type[] interfaces, Type type, GeneratorContext context)
        {
            InterfaceProxyGenerator generator = new InterfaceProxyGenerator(this._scope, context);
            return generator.GenerateCode(interfaces, type);
        }

        public virtual Type CreateInterfaceProxy(Type[] interfaces, Type type)
        {
            InterfaceProxyGenerator generator = new InterfaceProxyGenerator(this._scope);
            return generator.GenerateCode(interfaces, type);
        }

        protected Castle.DynamicProxy.Builder.CodeGenerators.ModuleScope ModuleScope
        {
            get
            {
                return this._scope;
            }
        }
    }
}

