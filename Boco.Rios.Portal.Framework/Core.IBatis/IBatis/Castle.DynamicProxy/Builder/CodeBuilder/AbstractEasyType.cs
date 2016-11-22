namespace Castle.DynamicProxy.Builder.CodeBuilder
{
    using Castle.DynamicProxy.Builder.CodeBuilder.SimpleAST;
    using Castle.DynamicProxy.Builder.CodeBuilder.Utils;
    using System;
    using System.Reflection;
    using System.Reflection.Emit;

    [CLSCompliant(false)]
    public abstract class AbstractEasyType
    {
        protected ConstructorCollection _constructors = new ConstructorCollection();
        private int _counter;
        protected EventsCollection _events = new EventsCollection();
        protected MethodCollection _methods = new MethodCollection();
        protected NestedTypeCollection _nested = new NestedTypeCollection();
        protected PropertiesCollection _properties = new PropertiesCollection();
        protected System.Reflection.Emit.TypeBuilder _typebuilder;

        public virtual Type BuildType()
        {
            this.EnsureBuildersAreInAValidState();
            Type type = this._typebuilder.CreateType();
            foreach (EasyNested nested in this._nested)
            {
                nested.BuildType();
            }
            return type;
        }

        public EasyConstructor CreateConstructor(params ArgumentReference[] arguments)
        {
            EasyConstructor constructor = new EasyConstructor(this, arguments);
            this._constructors.Add(constructor);
            return constructor;
        }

        public void CreateDefaultConstructor()
        {
            this._constructors.Add(new EasyDefaultConstructor(this));
        }

        public EasyEvent CreateEvent(string name, Type eventHandlerType)
        {
            EasyEvent easyEvent = new EasyEvent(this, name, eventHandlerType);
            this._events.Add(easyEvent);
            return easyEvent;
        }

        public FieldReference CreateField(string name, Type fieldType)
        {
            return this.CreateField(name, fieldType, true);
        }

        public FieldReference CreateField(string name, Type fieldType, bool serializable)
        {
            FieldAttributes @public = FieldAttributes.Public;
            if (!serializable)
            {
                @public |= FieldAttributes.NotSerialized;
            }
            return new FieldReference(this._typebuilder.DefineField(name, fieldType, @public));
        }

        public EasyMethod CreateMethod(string name, ReturnReferenceExpression returnType, params ArgumentReference[] arguments)
        {
            EasyMethod method = new EasyMethod(this, name, returnType, arguments);
            this._methods.Add(method);
            return method;
        }

        public EasyMethod CreateMethod(string name, ReturnReferenceExpression returnType, MethodAttributes attributes, params ArgumentReference[] arguments)
        {
            EasyMethod method = new EasyMethod(this, name, attributes, returnType, arguments);
            this._methods.Add(method);
            return method;
        }

        public EasyMethod CreateMethod(string name, MethodAttributes attrs, ReturnReferenceExpression returnType, params Type[] args)
        {
            EasyMethod method = new EasyMethod(this, name, attrs, returnType, ArgumentsUtil.ConvertToArgumentReference(args));
            this._methods.Add(method);
            return method;
        }

        public EasyProperty CreateProperty(PropertyInfo property)
        {
            EasyProperty property2 = new EasyProperty(this, property.Name, property.PropertyType);
            property2.IndexParameters = property.GetIndexParameters();
            this._properties.Add(property2);
            return property2;
        }

        public EasyProperty CreateProperty(string name, Type returnType)
        {
            EasyProperty property = new EasyProperty(this, name, returnType);
            this._properties.Add(property);
            return property;
        }

        public EasyConstructor CreateRuntimeConstructor(params ArgumentReference[] arguments)
        {
            EasyRuntimeConstructor constructor = new EasyRuntimeConstructor(this, arguments);
            this._constructors.Add(constructor);
            return constructor;
        }

        public EasyRuntimeMethod CreateRuntimeMethod(string name, ReturnReferenceExpression returnType, params ArgumentReference[] arguments)
        {
            EasyRuntimeMethod method = new EasyRuntimeMethod(this, name, returnType, arguments);
            this._methods.Add(method);
            return method;
        }

        protected virtual void EnsureBuildersAreInAValidState()
        {
            if (this._constructors.Count == 0)
            {
                this.CreateDefaultConstructor();
            }
            foreach (IEasyMember member in this._properties)
            {
                member.EnsureValidCodeBlock();
                member.Generate();
            }
            foreach (IEasyMember member2 in this._events)
            {
                member2.EnsureValidCodeBlock();
                member2.Generate();
            }
            foreach (IEasyMember member3 in this._constructors)
            {
                member3.EnsureValidCodeBlock();
                member3.Generate();
            }
            foreach (IEasyMember member4 in this._methods)
            {
                member4.EnsureValidCodeBlock();
                member4.Generate();
            }
        }

        internal Type BaseType
        {
            get
            {
                return this.TypeBuilder.BaseType;
            }
        }

        public ConstructorCollection Constructors
        {
            get
            {
                return this._constructors;
            }
        }

        internal int IncrementAndGetCounterValue
        {
            get
            {
                return ++this._counter;
            }
        }

        public MethodCollection Methods
        {
            get
            {
                return this._methods;
            }
        }

        public System.Reflection.Emit.TypeBuilder TypeBuilder
        {
            get
            {
                return this._typebuilder;
            }
        }
    }
}

