namespace Castle.DynamicProxy
{
    using System;
    using System.Collections;
    using System.Reflection;
    using System.Runtime.Serialization;

    [CLSCompliant(true)]
    public sealed class GeneratorContext : DictionaryBase
    {
        private Type _interceptor = typeof(IInterceptor);
        private Type _interfaceInvocation = typeof(Castle.DynamicProxy.Invocation.InterfaceInvocation);
        private Type _invocation = typeof(IInvocation);
        private ArrayList _mixins = new ArrayList();
        private Type _proxyObjectReference = typeof(Castle.DynamicProxy.Serialization.ProxyObjectReference);
        private Type _sameClassInvocation = typeof(Castle.DynamicProxy.Invocation.SameClassInvocation);
        private IList _skipInterfaces = new ArrayList();
        private IList _skipMethods = new ArrayList();

        public GeneratorContext()
        {
            this.AddInterfaceToSkip(typeof(ISerializable));
            this.AddMethodToSkip(typeof(ISerializable).GetMethod("GetObjectData"));
        }

        public void AddInterfaceToSkip(Type interfaceType)
        {
            this._skipInterfaces.Add(interfaceType);
        }

        public void AddMethodToSkip(MethodInfo method)
        {
            this._skipMethods.Add(method);
        }

        public void AddMixinInstance(object instance)
        {
            this._mixins.Add(instance);
        }

        public object[] MixinsAsArray()
        {
            return this._mixins.ToArray();
        }

        public bool ShouldSkip(MethodInfo method)
        {
            return this._skipMethods.Contains(method);
        }

        public bool ShouldSkip(Type interfaceType)
        {
            return this._skipInterfaces.Contains(interfaceType);
        }

        public bool HasMixins
        {
            get
            {
                return (this._mixins.Count != 0);
            }
        }

        public Type Interceptor
        {
            get
            {
                return this._interceptor;
            }
            set
            {
                this._interceptor = value;
            }
        }

        public Type InterfaceInvocation
        {
            get
            {
                return this._interfaceInvocation;
            }
            set
            {
                this._interfaceInvocation = value;
            }
        }

        public Type Invocation
        {
            get
            {
                return this._invocation;
            }
            set
            {
                this._invocation = value;
            }
        }

        public object this[string key]
        {
            get
            {
                return base.Dictionary[key];
            }
            set
            {
                base.Dictionary[key] = value;
            }
        }

        public Type ProxyObjectReference
        {
            get
            {
                return this._proxyObjectReference;
            }
            set
            {
                this._proxyObjectReference = value;
            }
        }

        public Type SameClassInvocation
        {
            get
            {
                return this._sameClassInvocation;
            }
            set
            {
                this._sameClassInvocation = value;
            }
        }
    }
}

