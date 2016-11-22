namespace Castle.DynamicProxy.Builder.CodeBuilder
{
    using Castle.DynamicProxy.Builder.CodeBuilder.SimpleAST;
    using System;
    using System.Collections;
    using System.Reflection.Emit;

    [CLSCompliant(false)]
    public abstract class AbstractCodeBuilder
    {
        private ILGenerator _generator;
        private ArrayList _ilmarkers = new ArrayList();
        private bool _isEmpty;
        private ArrayList _stmts = new ArrayList();

        protected AbstractCodeBuilder(ILGenerator generator)
        {
            this._generator = generator;
            this._isEmpty = true;
        }

        public void AddStatement(Statement stmt)
        {
            this.SetNonEmpty();
            this._stmts.Add(stmt);
        }

        public LabelReference CreateLabel()
        {
            LabelReference reference = new LabelReference();
            this._ilmarkers.Add(reference);
            return reference;
        }

        public LocalReference DeclareLocal(Type type)
        {
            LocalReference reference = new LocalReference(type);
            this._ilmarkers.Add(reference);
            return reference;
        }

        internal void Generate(IEasyMember member, ILGenerator il)
        {
            foreach (Reference reference in this._ilmarkers)
            {
                reference.Generate(il);
            }
            foreach (Statement statement in this._stmts)
            {
                statement.Emit(member, il);
            }
        }

        protected internal void SetNonEmpty()
        {
            this._isEmpty = false;
        }

        protected ILGenerator Generator
        {
            get
            {
                return this._generator;
            }
        }

        internal bool IsEmpty
        {
            get
            {
                return this._isEmpty;
            }
        }
    }
}

