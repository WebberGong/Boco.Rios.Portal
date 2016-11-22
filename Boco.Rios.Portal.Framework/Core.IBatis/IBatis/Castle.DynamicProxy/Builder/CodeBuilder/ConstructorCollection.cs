namespace Castle.DynamicProxy.Builder.CodeBuilder
{
    using System;
    using System.Collections;

    [CLSCompliant(false)]
    public class ConstructorCollection : CollectionBase
    {
        public void Add(EasyConstructor constructor)
        {
            base.InnerList.Add(constructor);
        }
    }
}

