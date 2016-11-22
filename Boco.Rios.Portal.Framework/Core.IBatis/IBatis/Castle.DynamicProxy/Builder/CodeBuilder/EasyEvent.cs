namespace Castle.DynamicProxy.Builder.CodeBuilder
{
    using Castle.DynamicProxy.Builder.CodeBuilder.SimpleAST;
    using Castle.DynamicProxy.Builder.CodeBuilder.Utils;
    using System;
    using System.Reflection;
    using System.Reflection.Emit;

    [CLSCompliant(false)]
    public class EasyEvent : IEasyMember
    {
        private EasyMethod m_addOnMethod;
        private EventBuilder m_builder;
        private AbstractEasyType m_maintype;
        private string m_name;
        private EasyMethod m_removeOnMethod;

        public EasyEvent(AbstractEasyType maintype, string name, Type eventHandlerType)
        {
            this.m_name = name;
            this.m_maintype = maintype;
            this.m_builder = maintype.TypeBuilder.DefineEvent(name, EventAttributes.None, eventHandlerType);
        }

        public EasyMethod CreateAddOnMethod(MethodAttributes atts, params Type[] parameters)
        {
            if (this.m_addOnMethod == null)
            {
                this.m_addOnMethod = new EasyMethod(this.m_maintype, "add_" + this.Name, atts, new ReturnReferenceExpression(typeof(void)), ArgumentsUtil.ConvertToArgumentReference(parameters));
            }
            return this.m_addOnMethod;
        }

        public EasyMethod CreateRemoveOnMethod(MethodAttributes atts, params Type[] parameters)
        {
            if (this.m_removeOnMethod == null)
            {
                this.m_removeOnMethod = new EasyMethod(this.m_maintype, "remove_" + this.Name, atts, new ReturnReferenceExpression(typeof(void)), ArgumentsUtil.ConvertToArgumentReference(parameters));
            }
            return this.m_removeOnMethod;
        }

        public void EnsureValidCodeBlock()
        {
            if (this.m_addOnMethod != null)
            {
                this.m_addOnMethod.EnsureValidCodeBlock();
            }
            if (this.m_removeOnMethod != null)
            {
                this.m_removeOnMethod.EnsureValidCodeBlock();
            }
        }

        public void Generate()
        {
            if (this.m_addOnMethod != null)
            {
                this.m_addOnMethod.Generate();
                this.m_builder.SetAddOnMethod(this.m_addOnMethod.MethodBuilder);
            }
            if (this.m_removeOnMethod != null)
            {
                this.m_removeOnMethod.Generate();
                this.m_builder.SetRemoveOnMethod(this.m_removeOnMethod.MethodBuilder);
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
                return this.m_name;
            }
        }

        public Type ReturnType
        {
            get
            {
                throw new Exception("TBD");
            }
        }
    }
}

