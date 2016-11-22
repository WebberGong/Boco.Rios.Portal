namespace Castle.DynamicProxy.Builder.CodeBuilder
{
    using Castle.DynamicProxy.Builder.CodeBuilder.SimpleAST;
    using Castle.DynamicProxy.Builder.CodeBuilder.Utils;
    using System;
    using System.Reflection;
    using System.Reflection.Emit;

    [CLSCompliant(false)]
    public class EasyMethod : IEasyMember
    {
        protected ArgumentReference[] _arguments;
        protected System.Reflection.Emit.MethodBuilder _builder;
        private MethodCodeBuilder _codebuilder;
        private AbstractEasyType _maintype;

        protected internal EasyMethod()
        {
        }

        internal EasyMethod(AbstractEasyType maintype, string name, ReturnReferenceExpression returnRef, params ArgumentReference[] arguments) : this(maintype, name, MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Public, returnRef, arguments)
        {
        }

        internal EasyMethod(AbstractEasyType maintype, string name, MethodAttributes attrs, ReturnReferenceExpression returnRef, params ArgumentReference[] arguments)
        {
            this._maintype = maintype;
            this._arguments = arguments;
            Type returnType = returnRef.Type;
            Type[] parameterTypes = ArgumentsUtil.InitializeAndConvert(arguments);
            this._builder = maintype.TypeBuilder.DefineMethod(name, attrs, returnType, parameterTypes);
        }

        public void DefineParameters(ParameterInfo[] info)
        {
            foreach (ParameterInfo info2 in info)
            {
                this._builder.DefineParameter(info2.Position + 1, info2.Attributes, info2.Name);
            }
        }

        public virtual void EnsureValidCodeBlock()
        {
            if (this.CodeBuilder.IsEmpty)
            {
                this.CodeBuilder.AddStatement(new NopStatement());
                this.CodeBuilder.AddStatement(new ReturnStatement());
            }
        }

        public virtual void Generate()
        {
            this._codebuilder.Generate(this, this._builder.GetILGenerator());
        }

        public ArgumentReference[] Arguments
        {
            get
            {
                return this._arguments;
            }
        }

        public virtual MethodCodeBuilder CodeBuilder
        {
            get
            {
                if (this._codebuilder == null)
                {
                    this._codebuilder = new MethodCodeBuilder(this._maintype.BaseType, this._builder, this._builder.GetILGenerator());
                }
                return this._codebuilder;
            }
        }

        public MethodBase Member
        {
            get
            {
                return this._builder;
            }
        }

        internal System.Reflection.Emit.MethodBuilder MethodBuilder
        {
            get
            {
                return this._builder;
            }
        }

        public Type ReturnType
        {
            get
            {
                return this._builder.ReturnType;
            }
        }
    }
}

