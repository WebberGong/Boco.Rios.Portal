namespace Castle.DynamicProxy.Builder.CodeGenerators
{
    using Castle.DynamicProxy;
    using Castle.DynamicProxy.Builder.CodeBuilder;
    using Castle.DynamicProxy.Builder.CodeBuilder.SimpleAST;
    using Castle.DynamicProxy.Builder.CodeBuilder.Utils;
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading;

    [CLSCompliant(false)]
    public class ClassProxyGenerator : BaseCodeGenerator
    {
        private bool _delegateToBaseGetObjectData;
        protected ConstructorInfo _serializationConstructor;

        public ClassProxyGenerator(ModuleScope scope) : base(scope)
        {
        }

        public ClassProxyGenerator(ModuleScope scope, GeneratorContext context) : base(scope, context)
        {
        }

        protected override void CustomizeGetObjectData(AbstractCodeBuilder codebuilder, ArgumentReference arg1, ArgumentReference arg2)
        {
            Type[] types = new Type[] { typeof(string), typeof(object) };
            Type[] typeArray2 = new Type[] { typeof(string), typeof(bool) };
            MethodInfo method = typeof(SerializationInfo).GetMethod("AddValue", types);
            MethodInfo info2 = typeof(SerializationInfo).GetMethod("AddValue", typeArray2);
            codebuilder.AddStatement(new ExpressionStatement(new VirtualMethodInvocationExpression(arg1, info2, new Expression[] { new FixedReference("__delegateToBase").ToExpression(), new FixedReference(this._delegateToBaseGetObjectData ? 1 : 0).ToExpression() })));
            if (this._delegateToBaseGetObjectData)
            {
                MethodInfo info3 = base._baseType.GetMethod("GetObjectData", new Type[] { typeof(SerializationInfo), typeof(StreamingContext) });
                codebuilder.AddStatement(new ExpressionStatement(new MethodInvocationExpression(info3, new Expression[] { arg1.ToExpression(), arg2.ToExpression() })));
            }
            else
            {
                LocalReference target = codebuilder.DeclareLocal(typeof(MemberInfo[]));
                LocalReference reference2 = codebuilder.DeclareLocal(typeof(object[]));
                MethodInfo info4 = typeof(FormatterServices).GetMethod("GetSerializableMembers", new Type[] { typeof(Type) });
                MethodInfo info5 = typeof(FormatterServices).GetMethod("GetObjectData", new Type[] { typeof(object), typeof(MemberInfo[]) });
                codebuilder.AddStatement(new AssignStatement(target, new MethodInvocationExpression(null, info4, new Expression[] { new TypeTokenExpression(base._baseType) })));
                codebuilder.AddStatement(new AssignStatement(reference2, new MethodInvocationExpression(null, info5, new Expression[] { SelfReference.Self.ToExpression(), target.ToExpression() })));
                codebuilder.AddStatement(new ExpressionStatement(new VirtualMethodInvocationExpression(arg1, method, new Expression[] { new FixedReference("__data").ToExpression(), reference2.ToExpression() })));
            }
        }

        public virtual Type GenerateCode(Type baseClass)
        {
            return this.GenerateCode(baseClass, new Type[0]);
        }

        public virtual Type GenerateCode(Type baseClass, Type[] interfaces)
        {
            Type type2;
            if (baseClass.IsSerializable)
            {
                this._delegateToBaseGetObjectData = this.VerifyIfBaseImplementsGetObjectData(baseClass);
                interfaces = this.AddISerializable(interfaces);
            }
            ReaderWriterLock rWLock = base.ModuleScope.RWLock;
            rWLock.AcquireReaderLock(-1);
            Type fromCache = base.GetFromCache(baseClass, interfaces);
            if (fromCache != null)
            {
                rWLock.ReleaseReaderLock();
                return fromCache;
            }
            rWLock.UpgradeToWriterLock(-1);
            try
            {
                this.CreateTypeBuilder(this.GenerateTypeName(baseClass, interfaces), baseClass, interfaces);
                this.GenerateFields();
                if (baseClass.IsSerializable)
                {
                    this.ImplementGetObjectData(interfaces);
                }
                this.ImplementCacheInvocationCache();
                base.GenerateTypeImplementation(baseClass, true);
                base.GenerateInterfaceImplementation(interfaces);
                this.GenerateConstructors(baseClass);
                if (this._delegateToBaseGetObjectData)
                {
                    this.GenerateSerializationConstructor();
                }
                type2 = this.CreateType();
            }
            finally
            {
                rWLock.ReleaseWriterLock();
            }
            return type2;
        }

        protected virtual EasyConstructor GenerateConstructor(ConstructorInfo baseConstructor)
        {
            ArrayList list = new ArrayList();
            ArgumentReference reference = new ArgumentReference(base.Context.Interceptor);
            ArgumentReference reference2 = new ArgumentReference(typeof(object[]));
            list.Add(reference);
            ParameterInfo[] parameters = baseConstructor.GetParameters();
            if (base.Context.HasMixins)
            {
                list.Add(reference2);
            }
            ArgumentReference[] c = ArgumentsUtil.ConvertToArgumentReference(parameters);
            list.AddRange(c);
            EasyConstructor constructor = base.MainTypeBuilder.CreateConstructor((ArgumentReference[]) list.ToArray(typeof(ArgumentReference)));
            this.GenerateConstructorCode(constructor.CodeBuilder, reference, SelfReference.Self, reference2);
            constructor.CodeBuilder.InvokeBaseConstructor(baseConstructor, c);
            constructor.CodeBuilder.AddStatement(new ReturnStatement());
            return constructor;
        }

        protected virtual void GenerateConstructors(Type baseClass)
        {
            foreach (ConstructorInfo info in baseClass.GetConstructors(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
            {
                if (!info.IsPrivate && (!info.IsAssembly || base.IsInternalToDynamicProxy(baseClass.Assembly)))
                {
                    this.GenerateConstructor(info);
                }
            }
        }

        public virtual Type GenerateCustomCode(Type baseClass, Type[] interfaces)
        {
            if (!base.Context.HasMixins)
            {
                return this.GenerateCode(baseClass);
            }
            base._mixins = base.Context.MixinsAsArray();
            Type[] mixinInterfaces = base.InspectAndRegisterInterfaces(base._mixins);
            interfaces = this.Join(interfaces, mixinInterfaces);
            return this.GenerateCode(baseClass, mixinInterfaces);
        }

        protected void GenerateSerializationConstructor()
        {
            ArgumentReference owner = new ArgumentReference(typeof(SerializationInfo));
            ArgumentReference reference2 = new ArgumentReference(typeof(StreamingContext));
            EasyConstructor constructor = base.MainTypeBuilder.CreateConstructor(new ArgumentReference[] { owner, reference2 });
            constructor.CodeBuilder.AddStatement(new ExpressionStatement(new ConstructorInvocationExpression(this._serializationConstructor, new Expression[] { owner.ToExpression(), reference2.ToExpression() })));
            Type[] types = new Type[] { typeof(string), typeof(Type) };
            MethodInfo method = typeof(SerializationInfo).GetMethod("GetValue", types);
            VirtualMethodInvocationExpression expression = new VirtualMethodInvocationExpression(owner, method, new Expression[] { new FixedReference("__interceptor").ToExpression(), new TypeTokenExpression(base.Context.Interceptor) });
            VirtualMethodInvocationExpression expression2 = new VirtualMethodInvocationExpression(owner, method, new Expression[] { new FixedReference("__mixins").ToExpression(), new TypeTokenExpression(typeof(object[])) });
            constructor.CodeBuilder.AddStatement(new AssignStatement(base.InterceptorField, expression));
            constructor.CodeBuilder.AddStatement(new AssignStatement(base.CacheField, new NewInstanceExpression(typeof(HybridDictionary).GetConstructor(new Type[0]), new Expression[0])));
            constructor.CodeBuilder.AddStatement(new AssignStatement(base.MixinField, expression2));
            foreach (CallableField field in base._cachedFields)
            {
                field.WriteInitialization(constructor.CodeBuilder, SelfReference.Self, base.MixinField);
            }
            constructor.CodeBuilder.AddStatement(new ReturnStatement());
        }

        protected override string GenerateTypeName(Type type, Type[] interfaces)
        {
            StringBuilder builder = new StringBuilder();
            foreach (Type type2 in interfaces)
            {
                builder.Append('_');
                builder.Append(type2.Name);
            }
            return string.Format("CProxyType{0}{3}{1}{2}", new object[] { type.Name, builder.ToString(), interfaces.Length, base.NormalizeNamespaceName(type.Namespace) });
        }

        protected Type[] Join(Type[] interfaces, Type[] mixinInterfaces)
        {
            if (interfaces == null)
            {
                interfaces = new Type[0];
            }
            if (mixinInterfaces == null)
            {
                mixinInterfaces = new Type[0];
            }
            Type[] destinationArray = new Type[interfaces.Length + mixinInterfaces.Length];
            Array.Copy(interfaces, 0, destinationArray, 0, interfaces.Length);
            Array.Copy(mixinInterfaces, 0, destinationArray, interfaces.Length, mixinInterfaces.Length);
            return destinationArray;
        }

        protected void SkipDefaultInterfaceImplementation(Type[] interfaces)
        {
            foreach (Type type in interfaces)
            {
                base.Context.AddInterfaceToSkip(type);
            }
        }

        protected bool VerifyIfBaseImplementsGetObjectData(Type baseType)
        {
            if (!typeof(ISerializable).IsAssignableFrom(baseType))
            {
                return false;
            }
            MethodInfo method = baseType.GetMethod("GetObjectData", new Type[] { typeof(SerializationInfo), typeof(StreamingContext) });
            if (method == null)
            {
                return false;
            }
            if (!method.IsVirtual || method.IsFinal)
            {
                throw new ProxyGenerationException(string.Format("The type {0} implements ISerializable, but GetObjectData is not marked as virtual", baseType.FullName));
            }
            base.Context.AddMethodToSkip(method);
            this._serializationConstructor = baseType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(SerializationInfo), typeof(StreamingContext) }, null);
            if (this._serializationConstructor == null)
            {
                throw new ProxyGenerationException(string.Format("The type {0} implements ISerializable, but failed to provide a deserialization constructor", baseType.FullName));
            }
            return true;
        }

        protected override Type InvocationType
        {
            get
            {
                return base.Context.SameClassInvocation;
            }
        }
    }
}

