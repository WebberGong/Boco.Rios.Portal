namespace Castle.DynamicProxy.Builder.CodeBuilder.SimpleAST
{
    using Castle.DynamicProxy.Builder.CodeBuilder;
    using System;
    using System.Reflection;
    using System.Reflection.Emit;

    [CLSCompliant(false)]
    public class MethodTokenExpression : Expression
    {
        private MethodInfo _method;

        public MethodTokenExpression(MethodInfo method)
        {
            this._method = method;
        }

        public override void Emit(IEasyMember member, ILGenerator gen)
        {
            gen.Emit(OpCodes.Ldtoken, this._method);
            gen.Emit(OpCodes.Ldtoken, this._method.DeclaringType);
            MethodInfo meth = typeof(MethodBase).GetMethod("GetMethodFromHandle", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(RuntimeMethodHandle), typeof(RuntimeTypeHandle) }, null);
            gen.Emit(OpCodes.Call, meth);
            gen.Emit(OpCodes.Castclass, typeof(MethodInfo));
        }
    }
}

