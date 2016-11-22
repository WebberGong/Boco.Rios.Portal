namespace Castle.DynamicProxy.Builder.CodeBuilder.SimpleAST
{
    using System;
    using System.Reflection.Emit;

    [CLSCompliant(false)]
    public abstract class Reference
    {
        private Reference _owner;

        public Reference()
        {
            this._owner = SelfReference.Self;
        }

        public Reference(Reference owner)
        {
            this._owner = SelfReference.Self;
            this._owner = owner;
        }

        public virtual void Generate(ILGenerator gen)
        {
        }

        public abstract void LoadAddressOfReference(ILGenerator gen);
        public abstract void LoadReference(ILGenerator gen);
        public abstract void StoreReference(ILGenerator gen);
        public virtual Expression ToAddressOfExpression()
        {
            return new AddressOfReferenceExpression(this);
        }

        public virtual Expression ToExpression()
        {
            return new ReferenceExpression(this);
        }

        public Reference OwnerReference
        {
            get
            {
                return this._owner;
            }
            set
            {
                this._owner = value;
            }
        }
    }
}

