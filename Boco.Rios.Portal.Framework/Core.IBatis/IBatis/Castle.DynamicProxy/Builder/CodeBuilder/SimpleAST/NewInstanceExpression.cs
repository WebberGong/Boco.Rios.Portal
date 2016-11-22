namespace Castle.DynamicProxy.Builder.CodeBuilder.SimpleAST
{
    using Castle.DynamicProxy.Builder.CodeBuilder;
    using System;
    using System.Reflection;
    using System.Reflection.Emit;

    [CLSCompliant(false)]
    public class NewInstanceExpression : Expression
    {
        private Expression[] _arguments;
        private ConstructorInfo _constructor;
        private Type[] _constructor_args;
        private Type _type;

        public NewInstanceExpression(EasyCallable callable, params Expression[] args) : this(callable.Constructor, args)
        {
        }

        public NewInstanceExpression(ConstructorInfo constructor, params Expression[] args)
        {
            this._constructor = constructor;
            this._arguments = args;
        }

        public NewInstanceExpression(Type target, Type[] constructor_args, params Expression[] args)
        {
            this._type = target;
            this._constructor_args = constructor_args;
            this._arguments = args;
        }

        public override void Emit(IEasyMember member, ILGenerator gen)
        {
            foreach (Expression expression in this._arguments)
            {
                expression.Emit(member, gen);
            }
            if (this._constructor == null)
            {
                this._constructor = this._type.GetConstructor(this._constructor_args);
            }
            if (this._constructor == null)
            {
                throw new ApplicationException("Could not find constructor matching specified arguments");
            }
            gen.Emit(OpCodes.Newobj, this._constructor);
        }
    }
}

