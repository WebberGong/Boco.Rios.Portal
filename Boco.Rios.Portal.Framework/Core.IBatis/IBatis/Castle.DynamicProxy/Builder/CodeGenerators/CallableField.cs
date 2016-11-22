namespace Castle.DynamicProxy.Builder.CodeGenerators
{
    using Castle.DynamicProxy.Builder.CodeBuilder;
    using Castle.DynamicProxy.Builder.CodeBuilder.SimpleAST;
    using System;
    using System.Reflection;

    internal class CallableField
    {
        private EasyCallable _callable;
        private MethodInfo _callback;
        private FieldReference _field;
        private int _sourceArgIndex;

        public CallableField(FieldReference field, EasyCallable callable, MethodInfo callback, int sourceArgIndex)
        {
            this._field = field;
            this._callable = callable;
            this._callback = callback;
            this._sourceArgIndex = sourceArgIndex;
        }

        public void WriteInitialization(AbstractCodeBuilder codebuilder, Reference targetArgument, Reference mixinArray)
        {
            NewInstanceExpression expression = null;
            if (this.SourceArgIndex == EmptyIndex)
            {
                expression = new NewInstanceExpression(this.Callable, new Expression[] { targetArgument.ToExpression(), new MethodPointerExpression(this._callback) });
            }
            else
            {
                expression = new NewInstanceExpression(this.Callable, new Expression[] { new LoadRefArrayElementExpression(this.SourceArgIndex, mixinArray), new MethodPointerExpression(this._callback) });
            }
            codebuilder.AddStatement(new AssignStatement(this.Field, expression));
        }

        public EasyCallable Callable
        {
            get
            {
                return this._callable;
            }
        }

        public static int EmptyIndex
        {
            get
            {
                return -1;
            }
        }

        public FieldReference Field
        {
            get
            {
                return this._field;
            }
        }

        public int SourceArgIndex
        {
            get
            {
                return this._sourceArgIndex;
            }
        }
    }
}

