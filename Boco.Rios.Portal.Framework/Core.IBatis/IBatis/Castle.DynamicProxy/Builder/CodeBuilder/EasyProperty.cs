namespace Castle.DynamicProxy.Builder.CodeBuilder
{
    using Castle.DynamicProxy.Builder.CodeBuilder.SimpleAST;
    using Castle.DynamicProxy.Builder.CodeBuilder.Utils;
    using System;
    using System.Reflection;
    using System.Reflection.Emit;

    [CLSCompliant(false)]
    public class EasyProperty : IEasyMember
    {
        private PropertyBuilder _builder;
        private EasyMethod _getMethod;
        private ParameterInfo[] _indexParameters;
        private AbstractEasyType _maintype;
        private EasyMethod _setMethod;

        public EasyProperty(AbstractEasyType maintype, string name, Type returnType)
        {
            this._maintype = maintype;
            this._builder = maintype.TypeBuilder.DefineProperty(name, PropertyAttributes.None, returnType, new Type[0]);
        }

        public EasyMethod CreateGetMethod()
        {
            return this.CreateGetMethod(MethodAttributes.SpecialName | MethodAttributes.Virtual | MethodAttributes.Public, new Type[0]);
        }

        public EasyMethod CreateGetMethod(MethodAttributes attrs, params Type[] parameters)
        {
            if (this._getMethod == null)
            {
                this._getMethod = new EasyMethod(this._maintype, "get_" + this._builder.Name, attrs, new ReturnReferenceExpression(this.ReturnType), ArgumentsUtil.ConvertToArgumentReference(parameters));
            }
            return this._getMethod;
        }

        public EasyMethod CreateSetMethod(Type arg)
        {
            return this.CreateSetMethod(MethodAttributes.SpecialName | MethodAttributes.Virtual | MethodAttributes.Public, new Type[] { arg });
        }

        public EasyMethod CreateSetMethod(MethodAttributes attrs, params Type[] parameters)
        {
            if (this._setMethod == null)
            {
                this._setMethod = new EasyMethod(this._maintype, "set_" + this._builder.Name, attrs, new ReturnReferenceExpression(typeof(void)), ArgumentsUtil.ConvertToArgumentReference(parameters));
            }
            return this._setMethod;
        }

        public void EnsureValidCodeBlock()
        {
            if (this._setMethod != null)
            {
                this._setMethod.EnsureValidCodeBlock();
            }
            if (this._getMethod != null)
            {
                this._getMethod.EnsureValidCodeBlock();
            }
        }

        public void Generate()
        {
            if (this._setMethod != null)
            {
                this._setMethod.Generate();
                this._builder.SetSetMethod(this._setMethod.MethodBuilder);
            }
            if (this._getMethod != null)
            {
                this._getMethod.Generate();
                this._builder.SetGetMethod(this._getMethod.MethodBuilder);
            }
        }

        public ParameterInfo[] IndexParameters
        {
            get
            {
                return this._indexParameters;
            }
            set
            {
                this._indexParameters = value;
            }
        }

        public MethodBase Member
        {
            get
            {
                return null;
            }
        }

        public string Name
        {
            get
            {
                return this._builder.Name;
            }
        }

        public Type ReturnType
        {
            get
            {
                return this._builder.PropertyType;
            }
        }
    }
}

