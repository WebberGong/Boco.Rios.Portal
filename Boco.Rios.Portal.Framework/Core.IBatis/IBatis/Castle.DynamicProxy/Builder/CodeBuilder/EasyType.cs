namespace Castle.DynamicProxy.Builder.CodeBuilder
{
    using Castle.DynamicProxy.Builder.CodeBuilder.SimpleAST;
    using Castle.DynamicProxy.Builder.CodeBuilder.Utils;
    using Castle.DynamicProxy.Builder.CodeGenerators;
    using System;
    using System.Collections;
    using System.Reflection;

    [CLSCompliant(false)]
    public class EasyType : AbstractEasyType
    {
        private static IDictionary assemblySigning = new Hashtable();

        protected EasyType()
        {
        }

        public EasyType(ModuleScope modulescope, string name) : this(modulescope, name, typeof(object), new Type[0])
        {
        }

        public EasyType(ModuleScope modulescope, string name, Type baseType, Type[] interfaces) : this(modulescope, name, baseType, interfaces, false)
        {
        }

        public EasyType(ModuleScope modulescope, string name, Type baseType, Type[] interfaces, bool serializable) : this()
        {
            TypeAttributes attr = TypeAttributes.Serializable | TypeAttributes.Public;
            if (serializable)
            {
                attr |= TypeAttributes.Serializable;
            }
            bool signStrongName = this.IsAssemblySigned(baseType);
            base._typebuilder = modulescope.ObtainDynamicModule(signStrongName).DefineType(name, attr, baseType, interfaces);
        }

        public EasyCallable CreateCallable(ReturnReferenceExpression returnType, params ArgumentReference[] args)
        {
            EasyCallable nested = new EasyCallable(this, base.IncrementAndGetCounterValue, returnType, args);
            base._nested.Add(nested);
            return nested;
        }

        public EasyCallable CreateCallable(Type returnType, params ParameterInfo[] args)
        {
            EasyCallable nested = new EasyCallable(this, base.IncrementAndGetCounterValue, new ReturnReferenceExpression(returnType), ArgumentsUtil.ConvertToArgumentReference(args));
            base._nested.Add(nested);
            return nested;
        }

        private bool IsAssemblySigned(Type baseType)
        {
            lock (assemblySigning)
            {
                if (!assemblySigning.Contains(baseType.Assembly))
                {
                    bool flag = baseType.Assembly.GetName().GetPublicKey().Length != 0;
                    assemblySigning.Add(baseType.Assembly, flag);
                }
                return (bool) assemblySigning[baseType.Assembly];
            }
        }
    }
}

