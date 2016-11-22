namespace Castle.DynamicProxy.Builder.CodeBuilder
{
    using System;
    using System.Collections;

    [CLSCompliant(false)]
    public class NestedTypeCollection : CollectionBase
    {
        public void Add(EasyNested nested)
        {
            base.InnerList.Add(nested);
        }
    }
}

