namespace Castle.DynamicProxy.Builder.CodeBuilder.SimpleAST
{
    using System;

    [CLSCompliant(false)]
    public abstract class TypeReference : Reference
    {
        private System.Type _type;

        public TypeReference(System.Type argumentType) : this(null, argumentType)
        {
        }

        public TypeReference(Reference owner, System.Type type) : base(owner)
        {
            this._type = type;
        }

        public System.Type Type
        {
            get
            {
                return this._type;
            }
        }
    }
}

