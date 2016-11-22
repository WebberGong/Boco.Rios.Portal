namespace Castle.DynamicProxy.Builder.CodeBuilder
{
    using Castle.DynamicProxy.Builder.CodeBuilder.SimpleAST;
    using Castle.DynamicProxy.Builder.CodeBuilder.Utils;
    using System;
    using System.Reflection;
    using System.Reflection.Emit;

    [CLSCompliant(false)]
    public class EasyConstructor : IEasyMember
    {
        protected ConstructorBuilder _builder;
        private ConstructorCodeBuilder _codebuilder;
        private AbstractEasyType _maintype;

        protected internal EasyConstructor()
        {
        }

        internal EasyConstructor(AbstractEasyType maintype, params ArgumentReference[] arguments)
        {
            this._maintype = maintype;
            Type[] parameterTypes = ArgumentsUtil.InitializeAndConvert(arguments);
            this._builder = maintype.TypeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, parameterTypes);
        }

        public virtual void EnsureValidCodeBlock()
        {
            if (this.CodeBuilder.IsEmpty)
            {
                this.CodeBuilder.InvokeBaseConstructor();
                this.CodeBuilder.AddStatement(new ReturnStatement());
            }
        }

        public virtual void Generate()
        {
            this._codebuilder.Generate(this, this._builder.GetILGenerator());
        }

        internal ConstructorBuilder Builder
        {
            get
            {
                return this._builder;
            }
        }

        public virtual ConstructorCodeBuilder CodeBuilder
        {
            get
            {
                if (this._codebuilder == null)
                {
                    this._codebuilder = new ConstructorCodeBuilder(this._maintype.BaseType, this._builder.GetILGenerator());
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

        public Type ReturnType
        {
            get
            {
                return typeof(void);
            }
        }
    }
}

