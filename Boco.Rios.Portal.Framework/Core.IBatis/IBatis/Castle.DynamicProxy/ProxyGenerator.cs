namespace Castle.DynamicProxy
{
    using Castle.DynamicProxy.Builder;
    using System;
    using System.Collections;

    [CLSCompliant(true)]
    public class ProxyGenerator
    {
        private IProxyBuilder _builder;

        public ProxyGenerator() : this(new DefaultProxyBuilder())
        {
        }

        public ProxyGenerator(IProxyBuilder builder)
        {
            this._builder = builder;
        }

        public virtual object CreateClassProxy(Type baseClass, IInterceptor interceptor, params object[] argumentsForConstructor)
        {
            return this.CreateClassProxy(baseClass, interceptor, true, argumentsForConstructor);
        }

        public virtual object CreateClassProxy(Type baseClass, IInterceptor interceptor, bool checkAbstract, params object[] argumentsForConstructor)
        {
            AssertUtil.IsClass(baseClass, "baseClass", checkAbstract);
            AssertUtil.NotNull(interceptor, "interceptor");
            Type type = this.ProxyBuilder.CreateClassProxy(baseClass);
            return this.CreateClassProxyInstance(type, interceptor, argumentsForConstructor);
        }

        public virtual object CreateClassProxy(Type baseClass, Type[] interfaces, IInterceptor interceptor, params object[] argumentsForConstructor)
        {
            return this.CreateClassProxy(baseClass, interfaces, interceptor, true, argumentsForConstructor);
        }

        public virtual object CreateClassProxy(Type baseClass, Type[] interfaces, IInterceptor interceptor, bool checkAbstract, params object[] argumentsForConstructor)
        {
            AssertUtil.IsClass(baseClass, "baseClass", checkAbstract);
            AssertUtil.NotNull(interceptor, "interceptor");
            AssertUtil.NotNull(interfaces, "interfaces");
            Type type = this.ProxyBuilder.CreateClassProxy(baseClass, interfaces);
            return this.CreateClassProxyInstance(type, interceptor, argumentsForConstructor);
        }

        protected virtual object CreateClassProxyInstance(Type type, IInterceptor interceptor, params object[] argumentsForConstructor)
        {
            ArrayList list = new ArrayList();
            list.Add(interceptor);
            list.AddRange(argumentsForConstructor);
            return Activator.CreateInstance(type, list.ToArray());
        }

        public virtual object CreateCustomClassProxy(Type baseClass, IInterceptor interceptor, GeneratorContext context, params object[] argumentsForConstructor)
        {
            return this.CreateCustomClassProxy(baseClass, interceptor, context, true, argumentsForConstructor);
        }

        public virtual object CreateCustomClassProxy(Type baseClass, IInterceptor interceptor, GeneratorContext context, bool checkAbstract, params object[] argumentsForConstructor)
        {
            AssertUtil.IsClass(baseClass, "baseClass", checkAbstract);
            AssertUtil.NotNull(interceptor, "interceptor");
            AssertUtil.NotNull(context, "context");
            Type type = this.ProxyBuilder.CreateCustomClassProxy(baseClass, context);
            return this.CreateCustomClassProxyInstance(type, interceptor, context, argumentsForConstructor);
        }

        protected virtual object CreateCustomClassProxyInstance(Type type, IInterceptor interceptor, GeneratorContext context, object target)
        {
            return this.CreateProxyInstance(type, interceptor, target);
        }

        protected virtual object CreateCustomClassProxyInstance(Type type, IInterceptor interceptor, GeneratorContext context, params object[] argumentsForConstructor)
        {
            if (context.HasMixins)
            {
                ArrayList list = new ArrayList();
                list.Add(interceptor);
                list.Add(context.MixinsAsArray());
                list.AddRange(argumentsForConstructor);
                return Activator.CreateInstance(type, list.ToArray());
            }
            return this.CreateClassProxyInstance(type, interceptor, argumentsForConstructor);
        }

        public virtual object CreateCustomProxy(Type theInterface, IInterceptor interceptor, object target, GeneratorContext context)
        {
            return this.CreateCustomProxy(new Type[] { theInterface }, interceptor, target, context);
        }

        public virtual object CreateCustomProxy(Type[] interfaces, IInterceptor interceptor, object target, GeneratorContext context)
        {
            AssertUtil.IsInterface(interfaces, "interfaces");
            AssertUtil.NotNull(interceptor, "interceptor");
            AssertUtil.NotNull(target, "target");
            AssertUtil.NotNull(context, "context");
            Type type = this.ProxyBuilder.CreateCustomInterfaceProxy(interfaces, target.GetType(), context);
            return this.CreateCustomProxyInstance(type, interceptor, target, context);
        }

        protected virtual object CreateCustomProxyInstance(Type type, IInterceptor interceptor, object target, GeneratorContext context)
        {
            if (context.HasMixins)
            {
                return Activator.CreateInstance(type, new object[] { interceptor, target, context.MixinsAsArray() });
            }
            return this.CreateProxyInstance(type, interceptor, target);
        }

        public virtual object CreateProxy(Type[] interfaces, IInterceptor interceptor, object target)
        {
            AssertUtil.IsInterface(interfaces, "interfaces");
            AssertUtil.NotNull(interceptor, "interceptor");
            AssertUtil.NotNull(target, "target");
            Type type = this.ProxyBuilder.CreateInterfaceProxy(interfaces, target.GetType());
            return this.CreateProxyInstance(type, interceptor, target);
        }

        public virtual object CreateProxy(Type theInterface, IInterceptor interceptor, object target)
        {
            return this.CreateProxy(new Type[] { theInterface }, interceptor, target);
        }

        protected virtual object CreateProxyInstance(Type type, IInterceptor interceptor, object target)
        {
            return Activator.CreateInstance(type, new object[] { interceptor, target });
        }

        public IProxyBuilder ProxyBuilder
        {
            get
            {
                return this._builder;
            }
            set
            {
                this._builder = value;
            }
        }
    }
}

