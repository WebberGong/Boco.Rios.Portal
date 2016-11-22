namespace Castle.DynamicProxy.Builder.CodeGenerators
{
    using Castle.DynamicProxy;
    using Castle.DynamicProxy.Builder.CodeBuilder;
    using Castle.DynamicProxy.Builder.CodeBuilder.SimpleAST;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;
    using System.Threading;

    [CLSCompliant(false)]
    public abstract class BaseCodeGenerator
    {
        protected Type _baseType;
        protected ArrayList _cachedFields;
        private FieldReference _cacheField;
        private GeneratorContext _context;
        private IList _generated;
        private FieldReference _interceptorField;
        protected HybridDictionary _interface2mixinIndex;
        protected Hashtable _method2Delegate;
        protected EasyMethod _method2Invocation;
        private FieldReference _mixinField;
        protected object[] _mixins;
        private Castle.DynamicProxy.Builder.CodeGenerators.ModuleScope _moduleScope;
        private EasyType _typeBuilder;
        private IDictionary<Assembly, bool> internalsToDynProxy;
        private ReaderWriterLock internalsToDynProxyLock;

        protected BaseCodeGenerator(Castle.DynamicProxy.Builder.CodeGenerators.ModuleScope moduleScope) : this(moduleScope, new GeneratorContext())
        {
        }

        protected BaseCodeGenerator(Castle.DynamicProxy.Builder.CodeGenerators.ModuleScope moduleScope, GeneratorContext context)
        {
            this._generated = new ArrayList();
            this.internalsToDynProxyLock = new ReaderWriterLock();
            this.internalsToDynProxy = new Dictionary<Assembly, bool>();
            this._baseType = typeof(object);
            this._mixins = new object[0];
            this._cachedFields = new ArrayList();
            this._method2Delegate = new Hashtable();
            this._interface2mixinIndex = new HybridDictionary();
            this._moduleScope = moduleScope;
            this._context = context;
        }

        protected virtual Type[] AddISerializable(Type[] interfaces)
        {
            if (Array.IndexOf<Type>(interfaces, typeof(ISerializable)) != -1)
            {
                return interfaces;
            }
            int length = interfaces.Length;
            Type[] destinationArray = new Type[length + 1];
            Array.Copy(interfaces, destinationArray, length);
            destinationArray[length] = typeof(ISerializable);
            return destinationArray;
        }

        protected EasyProperty CreateProperty(PropertyInfo property)
        {
            return this._typeBuilder.CreateProperty(property);
        }

        protected virtual Type CreateType()
        {
            Type generatedType = this.MainTypeBuilder.BuildType();
            this.RegisterInCache(generatedType);
            return generatedType;
        }

        protected virtual EasyType CreateTypeBuilder(string typeName, Type baseType, Type[] interfaces)
        {
            this._baseType = baseType;
            this._typeBuilder = new EasyType(this.ModuleScope, typeName, baseType, interfaces, true);
            return this._typeBuilder;
        }

        protected virtual void CustomizeGetObjectData(AbstractCodeBuilder codebuilder, ArgumentReference arg1, ArgumentReference arg2)
        {
        }

        protected Type[] Filter(Type[] mixinInterfaces)
        {
            ArrayList list = new ArrayList();
            foreach (Type type in mixinInterfaces)
            {
                if (!this.Context.ShouldSkip(type))
                {
                    list.Add(type);
                }
            }
            return (Type[]) list.ToArray(typeof(Type));
        }

        protected virtual MethodInfo GenerateCallbackMethodIfNecessary(MethodInfo method, Reference invocationTarget)
        {
            if (this.Context.HasMixins && this._interface2mixinIndex.Contains(method.DeclaringType))
            {
                return method;
            }
            string name = string.Format("callback__{0}", method.Name);
            ParameterInfo[] parameters = method.GetParameters();
            ArgumentReference[] arguments = new ArgumentReference[parameters.Length];
            for (int i = 0; i < arguments.Length; i++)
            {
                arguments[i] = new ArgumentReference(parameters[i].ParameterType);
            }
            EasyMethod method2 = this.MainTypeBuilder.CreateMethod(name, new ReturnReferenceExpression(method.ReturnType), MethodAttributes.HideBySig | MethodAttributes.Public, arguments);
            Expression[] args = new Expression[parameters.Length];
            for (int j = 0; j < arguments.Length; j++)
            {
                args[j] = arguments[j].ToExpression();
            }
            if (invocationTarget == null)
            {
                method2.CodeBuilder.AddStatement(new ReturnStatement(new MethodInvocationExpression(method, args)));
            }
            else
            {
                method2.CodeBuilder.AddStatement(new ReturnStatement(new MethodInvocationExpression(invocationTarget, method, args)));
            }
            return method2.MethodBuilder;
        }

        protected virtual EasyConstructor GenerateConstructor()
        {
            return null;
        }

        protected virtual void GenerateConstructorCode(ConstructorCodeBuilder codebuilder, Reference interceptorArg, Reference targetArgument, Reference mixinArray)
        {
            codebuilder.AddStatement(new AssignStatement(this.InterceptorField, interceptorArg.ToExpression()));
            int length = this.Context.MixinsAsArray().Length;
            codebuilder.AddStatement(new AssignStatement(this.MixinField, new NewArrayExpression(length, typeof(object))));
            if (this.Context.HasMixins)
            {
                for (int i = 0; i < length; i++)
                {
                    codebuilder.AddStatement(new AssignArrayStatement(this.MixinField, i, new LoadRefArrayElementExpression(i, mixinArray)));
                }
            }
            codebuilder.AddStatement(new AssignStatement(this.CacheField, new NewInstanceExpression(typeof(HybridDictionary).GetConstructor(new Type[0]), new Expression[0])));
            foreach (CallableField field in this._cachedFields)
            {
                field.WriteInitialization(codebuilder, targetArgument, mixinArray);
            }
        }

        protected virtual void GenerateFields()
        {
            this._interceptorField = this._typeBuilder.CreateField("__interceptor", this.Context.Interceptor);
            this._cacheField = this._typeBuilder.CreateField("__cache", typeof(HybridDictionary), false);
            this._mixinField = this._typeBuilder.CreateField("__mixin", typeof(object[]));
        }

        protected void GenerateInterfaceImplementation(Type[] interfaces)
        {
            foreach (Type type in interfaces)
            {
                this.GenerateTypeImplementation(type, false);
            }
        }

        protected void GenerateMethodImplementation(MethodInfo method, EasyProperty[] properties)
        {
            if (!this.Context.ShouldSkip(method))
            {
                ParameterInfo[] parameters = method.GetParameters();
                Type[] args = new Type[parameters.Length];
                for (int i = 0; i < parameters.Length; i++)
                {
                    args[i] = parameters[i].ParameterType;
                }
                MethodAttributes attrs = this.ObtainMethodAttributes(method);
                this.PreProcessMethod(method);
                EasyMethod builder = null;
                bool flag = method.Name.StartsWith("set_");
                bool flag2 = method.Name.StartsWith("get_");
                if (!flag && !flag2)
                {
                    builder = this._typeBuilder.CreateMethod(method.Name, attrs, new ReturnReferenceExpression(method.ReturnType), args);
                }
                else if (flag || flag2)
                {
                    foreach (EasyProperty property in properties)
                    {
                        if (property == null)
                        {
                            break;
                        }
                        if (property.Name.Equals(method.Name.Substring(4)))
                        {
                            if (property.IndexParameters != null)
                            {
                                bool flag3 = true;
                                int length = parameters.Length;
                                if (flag)
                                {
                                    length--;
                                }
                                if (length != property.IndexParameters.Length)
                                {
                                    goto Label_015D;
                                }
                                for (int j = 0; j < property.IndexParameters.Length; j++)
                                {
                                    if (property.IndexParameters[j].ParameterType != parameters[j].ParameterType)
                                    {
                                        flag3 = false;
                                        break;
                                    }
                                }
                                if (!flag3)
                                {
                                    goto Label_015D;
                                }
                            }
                            if (flag)
                            {
                                builder = property.CreateSetMethod(attrs, args);
                            }
                            else
                            {
                                builder = property.CreateGetMethod(attrs, args);
                            }
                            break;
                        Label_015D:;
                        }
                    }
                }
                builder.DefineParameters(parameters);
                this.WriteInterceptorInvocationMethod(method, builder);
                this.PostProcessMethod(method);
            }
        }

        protected virtual void GenerateMethods(Type inter, EasyProperty[] properties)
        {
            foreach (MethodInfo info in inter.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
            {
                if (((((!info.IsPrivate && info.IsVirtual) && !info.IsFinal) && (!info.IsAssembly || this.IsInternalToDynamicProxy(inter.Assembly))) && (!info.DeclaringType.Equals(typeof(object)) || info.IsVirtual)) && (!info.DeclaringType.Equals(typeof(object)) || !"Finalize".Equals(info.Name)))
                {
                    this.GenerateMethodImplementation(info, properties);
                }
            }
        }

        protected virtual EasyProperty[] GenerateProperties(Type inter)
        {
            PropertyInfo[] properties = inter.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            EasyProperty[] propertyArray = new EasyProperty[properties.Length];
            for (int i = 0; i < properties.Length; i++)
            {
                propertyArray[i] = this.CreateProperty(properties[i]);
            }
            return propertyArray;
        }

        protected void GenerateTypeImplementation(Type type, bool ignoreInterfaces)
        {
            if (!this._generated.Contains(type) && !this.Context.ShouldSkip(type))
            {
                this._generated.Add(type);
                if (!ignoreInterfaces)
                {
                    Type[] interfaces = type.FindInterfaces(new TypeFilter(BaseCodeGenerator.NoFilterImpl), type);
                    this.GenerateInterfaceImplementation(interfaces);
                }
                EasyProperty[] properties = this.GenerateProperties(type);
                this.GenerateMethods(type, properties);
            }
        }

        protected abstract string GenerateTypeName(Type type, Type[] interfaces);
        protected virtual MethodInfo GetCorrectMethod(MethodInfo method)
        {
            return method;
        }

        protected Type GetFromCache(Type baseClass, Type[] interfaces)
        {
            return this.ModuleScope[this.GenerateTypeName(baseClass, interfaces)];
        }

        protected virtual Expression GetPseudoInvocationTarget(MethodInfo method)
        {
            return NullExpression.Instance;
        }

        protected virtual void ImplementCacheInvocationCache()
        {
            MethodInfo method = typeof(HybridDictionary).GetMethod("get_Item", new Type[] { typeof(object) });
            MethodInfo info2 = typeof(HybridDictionary).GetMethod("Add", new Type[] { typeof(object), typeof(object) });
            Type[] types = new Type[] { typeof(ICallable), typeof(object), typeof(MethodInfo), typeof(object) };
            ArgumentReference reference = new ArgumentReference(typeof(ICallable));
            ArgumentReference reference2 = new ArgumentReference(typeof(MethodInfo));
            ArgumentReference reference3 = new ArgumentReference(typeof(object));
            this._method2Invocation = this.MainTypeBuilder.CreateMethod("_Method2Invocation", new ReturnReferenceExpression(this.Context.Invocation), MethodAttributes.HideBySig | MethodAttributes.Family, new ArgumentReference[] { reference, reference2, reference3 });
            LocalReference target = this._method2Invocation.CodeBuilder.DeclareLocal(this.Context.Invocation);
            LockBlockExpression expression = new LockBlockExpression(SelfReference.Self);
            expression.AddStatement(new AssignStatement(target, new ConvertExpression(this.Context.Invocation, new VirtualMethodInvocationExpression(this.CacheField, method, new Expression[] { reference2.ToExpression() }))));
            ConditionExpression expression2 = new ConditionExpression(OpCodes.Brfalse_S, target.ToExpression());
            expression2.AddTrueStatement(new AssignStatement(target, new NewInstanceExpression(this.InvocationType.GetConstructor(types), new Expression[] { reference.ToExpression(), SelfReference.Self.ToExpression(), reference2.ToExpression(), reference3.ToExpression() })));
            expression2.AddTrueStatement(new ExpressionStatement(new VirtualMethodInvocationExpression(this.CacheField, info2, new Expression[] { reference2.ToExpression(), target.ToExpression() })));
            expression.AddStatement(new ExpressionStatement(expression2));
            this._method2Invocation.CodeBuilder.AddStatement(new ExpressionStatement(expression));
            this._method2Invocation.CodeBuilder.AddStatement(new ReturnStatement(target));
        }

        protected virtual void ImplementGetObjectData(Type[] interfaces)
        {
            this._generated.Add(typeof(ISerializable));
            Type[] types = new Type[] { typeof(string), typeof(bool), typeof(bool) };
            Type[] typeArray2 = new Type[] { typeof(string), typeof(object) };
            MethodInfo info = typeof(SerializationInfo).GetMethod("AddValue", typeArray2);
            ArgumentReference owner = new ArgumentReference(typeof(SerializationInfo));
            ArgumentReference reference2 = new ArgumentReference(typeof(StreamingContext));
            EasyMethod method = this.MainTypeBuilder.CreateMethod("GetObjectData", new ReturnReferenceExpression(typeof(void)), new ArgumentReference[] { owner, reference2 });
            LocalReference target = method.CodeBuilder.DeclareLocal(typeof(Type));
            method.CodeBuilder.AddStatement(new AssignStatement(target, new MethodInvocationExpression(null, typeof(Type).GetMethod("GetType", types), new Expression[] { new FixedReference(this.Context.ProxyObjectReference.AssemblyQualifiedName).ToExpression(), new FixedReference(1).ToExpression(), new FixedReference(0).ToExpression() })));
            method.CodeBuilder.AddStatement(new ExpressionStatement(new VirtualMethodInvocationExpression(owner, typeof(SerializationInfo).GetMethod("SetType"), new Expression[] { target.ToExpression() })));
            method.CodeBuilder.AddStatement(new ExpressionStatement(new VirtualMethodInvocationExpression(owner, info, new Expression[] { new FixedReference("__interceptor").ToExpression(), this.InterceptorField.ToExpression() })));
            method.CodeBuilder.AddStatement(new ExpressionStatement(new VirtualMethodInvocationExpression(owner, info, new Expression[] { new FixedReference("__mixins").ToExpression(), this.MixinField.ToExpression() })));
            LocalReference reference4 = method.CodeBuilder.DeclareLocal(typeof(string[]));
            method.CodeBuilder.AddStatement(new AssignStatement(reference4, new NewArrayExpression(interfaces.Length, typeof(string))));
            for (int i = 0; i < interfaces.Length; i++)
            {
                method.CodeBuilder.AddStatement(new AssignArrayStatement(reference4, i, new FixedReference(interfaces[i].AssemblyQualifiedName).ToExpression()));
            }
            method.CodeBuilder.AddStatement(new ExpressionStatement(new VirtualMethodInvocationExpression(owner, info, new Expression[] { new FixedReference("__interfaces").ToExpression(), reference4.ToExpression() })));
            method.CodeBuilder.AddStatement(new ExpressionStatement(new VirtualMethodInvocationExpression(owner, info, new Expression[] { new FixedReference("__baseType").ToExpression(), new TypeTokenExpression(this._baseType) })));
            this.CustomizeGetObjectData(method.CodeBuilder, owner, reference2);
            method.CodeBuilder.AddStatement(new ReturnStatement());
        }

        protected Type[] InspectAndRegisterInterfaces(object[] mixins)
        {
            if (mixins == null)
            {
                return new Type[0];
            }
            Set set = new Set();
            for (int i = 0; i < mixins.Length; i++)
            {
                object obj2 = mixins[i];
                Type[] interfaces = obj2.GetType().GetInterfaces();
                interfaces = this.Filter(interfaces);
                set.AddArray(interfaces);
                foreach (Type type in interfaces)
                {
                    this._interface2mixinIndex.Add(type, i);
                }
            }
            return (Type[]) set.ToArray(typeof(Type));
        }

        protected bool IsInternalToDynamicProxy(Assembly asm)
        {
            bool flag2;
            this.internalsToDynProxyLock.AcquireReaderLock(-1);
            try
            {
                if (!this.internalsToDynProxy.ContainsKey(asm))
                {
                    this.internalsToDynProxyLock.UpgradeToWriterLock(-1);
                    InternalsVisibleToAttribute[] customAttributes = (InternalsVisibleToAttribute[]) asm.GetCustomAttributes(typeof(InternalsVisibleToAttribute), false);
                    bool flag = false;
                    foreach (InternalsVisibleToAttribute attribute in customAttributes)
                    {
                        if (attribute.AssemblyName.Contains(Castle.DynamicProxy.Builder.CodeGenerators.ModuleScope.ASSEMBLY_NAME))
                        {
                            flag = true;
                            break;
                        }
                    }
                    this.internalsToDynProxy.Add(asm, flag);
                }
                flag2 = this.internalsToDynProxy[asm];
            }
            finally
            {
                this.internalsToDynProxyLock.ReleaseLock();
            }
            return flag2;
        }

        public static bool NoFilterImpl(Type type, object criteria)
        {
            return true;
        }

        protected string NormalizeNamespaceName(string nsName)
        {
            if ((nsName == null) || (nsName == string.Empty))
            {
                return string.Empty;
            }
            string[] strArray = nsName.Split(new char[] { '.' });
            return strArray[strArray.Length - 1];
        }

        protected ConstructorInfo ObtainAvailableConstructor(Type target)
        {
            return target.GetConstructor(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, new Type[0], null);
        }

        protected FieldReference ObtainCallableFieldBuilderDelegate(EasyCallable builder)
        {
            foreach (CallableField field in this._cachedFields)
            {
                if (field.Callable == builder)
                {
                    return field.Field;
                }
            }
            return null;
        }

        private MethodAttributes ObtainMethodAttributes(MethodInfo method)
        {
            MethodAttributes @virtual = MethodAttributes.Virtual;
            if (method.IsPublic)
            {
                @virtual |= MethodAttributes.Public;
            }
            if (this.IsInternalToDynamicProxy(method.DeclaringType.Assembly) && method.IsAssembly)
            {
                @virtual |= MethodAttributes.Assembly;
            }
            if (method.IsHideBySig)
            {
                @virtual |= MethodAttributes.HideBySig;
            }
            if (method.IsFamilyAndAssembly)
            {
                @virtual |= MethodAttributes.FamANDAssem;
            }
            else if (method.IsFamilyOrAssembly)
            {
                @virtual |= MethodAttributes.FamORAssem;
            }
            else if (method.IsFamily)
            {
                @virtual |= MethodAttributes.Family;
            }
            if (!method.Name.StartsWith("set_") && !method.Name.StartsWith("get_"))
            {
                return @virtual;
            }
            return (@virtual | MethodAttributes.SpecialName);
        }

        protected virtual void PostProcessMethod(MethodInfo method)
        {
        }

        protected virtual void PreProcessMethod(MethodInfo method)
        {
            MethodInfo callbackMethod = this.GenerateCallbackMethodIfNecessary(method, null);
            EasyCallable builder = this.MainTypeBuilder.CreateCallable(method.ReturnType, method.GetParameters());
            this._method2Delegate[method] = builder;
            FieldReference field = this.MainTypeBuilder.CreateField(string.Format("_cached_{0}", builder.ID), builder.TypeBuilder);
            this.RegisterDelegateFieldToBeInitialized(method, field, builder, callbackMethod);
        }

        protected void RegisterDelegateFieldToBeInitialized(MethodInfo method, FieldReference field, EasyCallable builder, MethodInfo callbackMethod)
        {
            int emptyIndex = CallableField.EmptyIndex;
            if (this.Context.HasMixins && this._interface2mixinIndex.Contains(method.DeclaringType))
            {
                emptyIndex = (int) this._interface2mixinIndex[method.DeclaringType];
            }
            this._cachedFields.Add(new CallableField(field, builder, callbackMethod, emptyIndex));
        }

        protected void RegisterInCache(Type generatedType)
        {
            this.ModuleScope[generatedType.Name] = generatedType;
        }

        protected virtual void WriteInterceptorInvocationMethod(MethodInfo method, EasyMethod builder)
        {
            ArgumentReference[] arguments = builder.Arguments;
            TypeReference[] args = IndirectReference.WrapIfByRef(builder.Arguments);
            LocalReference target = builder.CodeBuilder.DeclareLocal(this.Context.Invocation);
            EasyCallable callable = this._method2Delegate[method] as EasyCallable;
            FieldReference reference2 = this.ObtainCallableFieldBuilderDelegate(callable);
            builder.CodeBuilder.AddStatement(new AssignStatement(target, new MethodInvocationExpression(this._method2Invocation, new Expression[] { reference2.ToExpression(), new MethodTokenExpression(this.GetCorrectMethod(method)), this.GetPseudoInvocationTarget(method) })));
            LocalReference reference3 = builder.CodeBuilder.DeclareLocal(typeof(object));
            LocalReference reference4 = builder.CodeBuilder.DeclareLocal(typeof(object[]));
            builder.CodeBuilder.AddStatement(new AssignStatement(reference4, new ReferencesToObjectArrayExpression(args)));
            builder.CodeBuilder.AddStatement(new AssignStatement(reference3, new VirtualMethodInvocationExpression(this.InterceptorField, this.Context.Interceptor.GetMethod("Intercept"), new Expression[] { target.ToExpression(), reference4.ToExpression() })));
            for (int i = 0; i < arguments.Length; i++)
            {
                if (arguments[i].Type.IsByRef)
                {
                    builder.CodeBuilder.AddStatement(new AssignStatement(args[i], new ConvertExpression(args[i].Type, new LoadRefArrayElementExpression(i, reference4))));
                }
            }
            if (builder.ReturnType == typeof(void))
            {
                builder.CodeBuilder.AddStatement(new ReturnStatement());
            }
            else
            {
                builder.CodeBuilder.AddStatement(new ReturnStatement(new ConvertExpression(builder.ReturnType, reference3.ToExpression())));
            }
        }

        protected FieldReference CacheField
        {
            get
            {
                return this._cacheField;
            }
        }

        protected GeneratorContext Context
        {
            get
            {
                return this._context;
            }
        }

        protected FieldReference InterceptorField
        {
            get
            {
                return this._interceptorField;
            }
        }

        protected abstract Type InvocationType { get; }

        protected EasyType MainTypeBuilder
        {
            get
            {
                return this._typeBuilder;
            }
        }

        protected FieldReference MixinField
        {
            get
            {
                return this._mixinField;
            }
        }

        protected Castle.DynamicProxy.Builder.CodeGenerators.ModuleScope ModuleScope
        {
            get
            {
                return this._moduleScope;
            }
        }
    }
}

