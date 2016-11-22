namespace Castle.DynamicProxy.Builder.CodeGenerators
{
    using Castle.DynamicProxy;
    using Castle.DynamicProxy.Builder.CodeBuilder;
    using Castle.DynamicProxy.Builder.CodeBuilder.SimpleAST;
    using System;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading;

    [CLSCompliant(false)]
    public class InterfaceProxyGenerator : BaseCodeGenerator
    {
        protected FieldReference _targetField;
        protected Type _targetType;

        public InterfaceProxyGenerator(ModuleScope scope) : base(scope)
        {
        }

        public InterfaceProxyGenerator(ModuleScope scope, GeneratorContext context) : base(scope, context)
        {
        }

        protected override void CustomizeGetObjectData(AbstractCodeBuilder codebuilder, ArgumentReference arg1, ArgumentReference arg2)
        {
            Type[] types = new Type[] { typeof(string), typeof(object) };
            MethodInfo method = typeof(SerializationInfo).GetMethod("AddValue", types);
            codebuilder.AddStatement(new ExpressionStatement(new VirtualMethodInvocationExpression(arg1, method, new Expression[] { new FixedReference("__target").ToExpression(), this._targetField.ToExpression() })));
        }

        protected override MethodInfo GenerateCallbackMethodIfNecessary(MethodInfo method, Reference invocationTarget)
        {
            if (base.Context.HasMixins && base._interface2mixinIndex.Contains(method.DeclaringType))
            {
                return method;
            }
            if (method.IsAbstract)
            {
                method = this.GetCorrectMethod(method);
            }
            return base.GenerateCallbackMethodIfNecessary(method, this._targetField);
        }

        public virtual Type GenerateCode(Type[] interfaces, Type targetType)
        {
            Type type2;
            if (base.Context.HasMixins)
            {
                base._mixins = base.Context.MixinsAsArray();
                Type[] mixinInterfaces = base.InspectAndRegisterInterfaces(base._mixins);
                interfaces = this.Join(interfaces, mixinInterfaces);
            }
            interfaces = this.AddISerializable(interfaces);
            ReaderWriterLock rWLock = base.ModuleScope.RWLock;
            rWLock.AcquireReaderLock(-1);
            Type fromCache = base.GetFromCache(targetType, interfaces);
            if (fromCache != null)
            {
                rWLock.ReleaseReaderLock();
                return fromCache;
            }
            rWLock.UpgradeToWriterLock(-1);
            try
            {
                this._targetType = targetType;
                this.CreateTypeBuilder(this.GenerateTypeName(targetType, interfaces), typeof(object), interfaces);
                this.GenerateFields();
                this.ImplementGetObjectData(interfaces);
                this.ImplementCacheInvocationCache();
                base.GenerateInterfaceImplementation(interfaces);
                this.GenerateConstructor();
                type2 = this.CreateType();
            }
            finally
            {
                rWLock.ReleaseWriterLock();
            }
            return type2;
        }

        protected override EasyConstructor GenerateConstructor()
        {
            EasyConstructor constructor;
            ArgumentReference interceptorArg = new ArgumentReference(base.Context.Interceptor);
            ArgumentReference reference2 = new ArgumentReference(typeof(object));
            ArgumentReference mixinArray = new ArgumentReference(typeof(object[]));
            if (base.Context.HasMixins)
            {
                constructor = base.MainTypeBuilder.CreateConstructor(new ArgumentReference[] { interceptorArg, reference2, mixinArray });
            }
            else
            {
                constructor = base.MainTypeBuilder.CreateConstructor(new ArgumentReference[] { interceptorArg, reference2 });
            }
            this.GenerateConstructorCode(constructor.CodeBuilder, interceptorArg, SelfReference.Self, mixinArray);
            constructor.CodeBuilder.InvokeBaseConstructor();
            constructor.CodeBuilder.AddStatement(new AssignStatement(this._targetField, reference2.ToExpression()));
            constructor.CodeBuilder.AddStatement(new ReturnStatement());
            return constructor;
        }

        protected override void GenerateFields()
        {
            base.GenerateFields();
            this._targetField = base.MainTypeBuilder.CreateField("__target", typeof(object));
        }

        protected override string GenerateTypeName(Type type, Type[] interfaces)
        {
            StringBuilder builder = new StringBuilder();
            foreach (Type type2 in interfaces)
            {
                builder.Append('_');
                builder.Append(type2.Name);
            }
            return string.Format("ProxyInterface{2}{0}{1}", type.Name, builder.ToString(), base.NormalizeNamespaceName(type.Namespace));
        }

        protected override MethodInfo GetCorrectMethod(MethodInfo method)
        {
            if (base.Context.HasMixins && base._interface2mixinIndex.Contains(method.DeclaringType))
            {
                return method;
            }
            ParameterInfo[] parameters = method.GetParameters();
            Type[] types = new Type[parameters.Length];
            for (int i = 0; i < types.Length; i++)
            {
                types[i] = parameters[i].ParameterType;
            }
            MethodInfo info = this._targetType.GetMethod(method.Name, types);
            if (info == null)
            {
                info = method;
            }
            return info;
        }

        protected override Expression GetPseudoInvocationTarget(MethodInfo method)
        {
            if (base.Context.HasMixins && base._interface2mixinIndex.Contains(method.DeclaringType))
            {
                return base.GetPseudoInvocationTarget(method);
            }
            return this._targetField.ToExpression();
        }

        protected Type[] Join(Type[] interfaces, Type[] mixinInterfaces)
        {
            Type[] destinationArray = new Type[interfaces.Length + mixinInterfaces.Length];
            Array.Copy(interfaces, 0, destinationArray, 0, interfaces.Length);
            Array.Copy(mixinInterfaces, 0, destinationArray, interfaces.Length, mixinInterfaces.Length);
            return destinationArray;
        }

        protected override Type InvocationType
        {
            get
            {
                return base.Context.InterfaceInvocation;
            }
        }
    }
}

