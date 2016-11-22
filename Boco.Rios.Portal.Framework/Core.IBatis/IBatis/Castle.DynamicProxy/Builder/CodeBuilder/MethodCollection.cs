namespace Castle.DynamicProxy.Builder.CodeBuilder
{
    using System;
    using System.Collections;

    [CLSCompliant(false)]
    public class MethodCollection : CollectionBase
    {
        public void Add(EasyMethod method)
        {
            base.InnerList.Add(method);
        }
    }
}

