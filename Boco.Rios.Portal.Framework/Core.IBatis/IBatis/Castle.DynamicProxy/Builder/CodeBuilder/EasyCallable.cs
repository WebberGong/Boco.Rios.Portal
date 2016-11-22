namespace Castle.DynamicProxy.Builder.CodeBuilder
{
    using Castle.DynamicProxy.Builder.CodeBuilder.SimpleAST;
    using System;
    using System.Reflection;

    [CLSCompliant(false)]
    public class EasyCallable : EasyNested
    {
        private ArgumentReference[] _args;
        private EasyMethod _callmethod;
        private EasyConstructor _constructor;
        private int _id;
        private EasyRuntimeMethod _invokeMethod;
        private ReturnReferenceExpression _returnType;

        public EasyCallable(AbstractEasyType type, int id, ReturnReferenceExpression returnType, params ArgumentReference[] args) : base(type, string.Format("__delegate_{0}", id), typeof(MulticastDelegate), new Type[] { typeof(ICallable) }, returnType, args)
        {
            this._id = id;
            this._args = args;
            this._returnType = returnType;
            this.GenerateConstructor();
            this.GenerateInvoke();
            this.GenerateCallableImplementation();
        }

        private void GenerateCall()
        {
            ArgumentReference arrayReference = new ArgumentReference(typeof(object[]));
            this._callmethod = base.CreateMethod("Call", new ReturnReferenceExpression(typeof(object)), new ArgumentReference[] { arrayReference });
            TypeReference[] referenceArray = IndirectReference.WrapIfByRef(this._args);
            LocalReference[] referenceArray2 = new LocalReference[this._args.Length];
            Expression[] args = new Expression[this._args.Length];
            for (int i = 0; i < this._args.Length; i++)
            {
                if (this._args[i].Type.IsByRef)
                {
                    referenceArray2[i] = this._callmethod.CodeBuilder.DeclareLocal(referenceArray[i].Type);
                    this._callmethod.CodeBuilder.AddStatement(new AssignStatement(referenceArray2[i], new ConvertExpression(referenceArray[i].Type, new LoadRefArrayElementExpression(i, arrayReference))));
                    args[i] = referenceArray2[i].ToAddressOfExpression();
                }
                else
                {
                    args[i] = new ConvertExpression(referenceArray[i].Type, new LoadRefArrayElementExpression(i, arrayReference));
                }
            }
            MethodInvocationExpression expression = new MethodInvocationExpression(this._invokeMethod, args);
            Expression instance = null;
            if (this._returnType.Type == typeof(void))
            {
                this._callmethod.CodeBuilder.AddStatement(new ExpressionStatement(expression));
                instance = NullExpression.Instance;
            }
            else
            {
                LocalReference target = this._callmethod.CodeBuilder.DeclareLocal(typeof(object));
                this._callmethod.CodeBuilder.AddStatement(new AssignStatement(target, new ConvertExpression(typeof(object), this._returnType.Type, expression)));
                instance = target.ToExpression();
            }
            for (int j = 0; j < this._args.Length; j++)
            {
                if (this._args[j].Type.IsByRef)
                {
                    this._callmethod.CodeBuilder.AddStatement(new AssignArrayStatement(arrayReference, j, new ConvertExpression(typeof(object), referenceArray[j].Type, referenceArray2[j].ToExpression())));
                }
            }
            this._callmethod.CodeBuilder.AddStatement(new ReturnStatement(instance));
        }

        private void GenerateCallableImplementation()
        {
            this.GenerateCall();
            this.GenerateTargetProperty();
        }

        private void GenerateConstructor()
        {
            ArgumentReference reference = new ArgumentReference(typeof(object));
            ArgumentReference reference2 = new ArgumentReference(typeof(IntPtr));
            this._constructor = base.CreateRuntimeConstructor(new ArgumentReference[] { reference, reference2 });
        }

        private void GenerateInvoke()
        {
            this._invokeMethod = base.CreateRuntimeMethod("Invoke", this._returnType, this._args);
        }

        private void GenerateTargetProperty()
        {
            EasyMethod method = base.CreateProperty("Target", typeof(object)).CreateGetMethod();
            MethodInfo info = typeof(MulticastDelegate).GetMethod("get_Target");
            method.CodeBuilder.AddStatement(new ReturnStatement(new MethodInvocationExpression(info, new Expression[0])));
        }

        public EasyMethod Callmethod
        {
            get
            {
                return this._callmethod;
            }
        }

        public ConstructorInfo Constructor
        {
            get
            {
                return this._constructor.Builder;
            }
        }

        public int ID
        {
            get
            {
                return this._id;
            }
        }

        public EasyMethod InvokeMethod
        {
            get
            {
                return this._invokeMethod;
            }
        }
    }
}

