namespace Castle.DynamicProxy.Serialization
{
    using Castle.DynamicProxy;
    using Castle.DynamicProxy.Builder.CodeGenerators;
    using System;
    using System.Collections;
    using System.Reflection;
    using System.Runtime.Serialization;

    [Serializable]
    public class ProxyObjectReference : IObjectReference, ISerializable
    {
        private Type _baseType;
        private object[] _data;
        private IInterceptor _interceptor;
        private Type[] _interfaces;
        private object[] _mixins;
        private object _proxy;
        private static ModuleScope _scope = new ModuleScope();

        protected ProxyObjectReference(SerializationInfo info, StreamingContext context)
        {
            this._interceptor = (IInterceptor) info.GetValue("__interceptor", typeof(IInterceptor));
            this._baseType = (Type) info.GetValue("__baseType", typeof(Type));
            this._mixins = (object[]) info.GetValue("__mixins", typeof(object[]));
            string[] strArray = (string[]) info.GetValue("__interfaces", typeof(string[]));
            this._interfaces = new Type[strArray.Length];
            for (int i = 0; i < strArray.Length; i++)
            {
                this._interfaces[i] = Type.GetType(strArray[i]);
            }
            this._proxy = this.RecreateProxy(info, context);
            this.InvokeCallback(this._proxy);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
        }

        public object GetRealObject(StreamingContext context)
        {
            return this._proxy;
        }

        protected void InvokeCallback(object target)
        {
            if (target is IDeserializationCallback)
            {
                (target as IDeserializationCallback).OnDeserialization(this);
            }
        }

        public object RecreateClassProxy(SerializationInfo info, StreamingContext context)
        {
            Type type;
            bool boolean = info.GetBoolean("__delegateToBase");
            if (!boolean)
            {
                this._data = (object[]) info.GetValue("__data", typeof(object[]));
            }
            object obj2 = null;
            GeneratorContext context2 = new GeneratorContext();
            if (this._mixins.Length != 0)
            {
                foreach (object obj3 in this._mixins)
                {
                    context2.AddMixinInstance(obj3);
                }
            }
            ClassProxyGenerator generator = new ClassProxyGenerator(_scope, context2);
            if (this._mixins.Length == 0)
            {
                type = generator.GenerateCode(this._baseType, this._interfaces);
            }
            else
            {
                type = generator.GenerateCustomCode(this._baseType, this._interfaces);
            }
            if (boolean)
            {
                return Activator.CreateInstance(type, new object[] { info, context });
            }
            if (this._mixins.Length == 0)
            {
                obj2 = Activator.CreateInstance(type, new object[] { this._interceptor });
            }
            else
            {
                ArrayList list = new ArrayList();
                list.Add(this._interceptor);
                list.Add(context2.MixinsAsArray());
                obj2 = Activator.CreateInstance(type, list.ToArray());
            }
            MemberInfo[] serializableMembers = FormatterServices.GetSerializableMembers(this._baseType);
            FormatterServices.PopulateObjectMembers(obj2, serializableMembers, this._data);
            return obj2;
        }

        public object RecreateInterfaceProxy(SerializationInfo info, StreamingContext context)
        {
            GeneratorContext context2 = new GeneratorContext();
            foreach (object obj3 in this._mixins)
            {
                context2.AddMixinInstance(obj3);
            }
            InterfaceProxyGenerator generator = new InterfaceProxyGenerator(_scope, context2);
            object obj4 = info.GetValue("__target", typeof(object));
            if (this._mixins.Length == 0)
            {
                return Activator.CreateInstance(generator.GenerateCode(this._interfaces, obj4.GetType()), new object[] { this._interceptor, obj4 });
            }
            return Activator.CreateInstance(generator.GenerateCode(this._interfaces, obj4.GetType()), new object[] { this._interceptor, obj4, context2.MixinsAsArray() });
        }

        protected virtual object RecreateProxy(SerializationInfo info, StreamingContext context)
        {
            if (this._baseType == typeof(object))
            {
                return this.RecreateInterfaceProxy(info, context);
            }
            return this.RecreateClassProxy(info, context);
        }

        public static void ResetScope()
        {
            _scope = new ModuleScope();
        }
    }
}

