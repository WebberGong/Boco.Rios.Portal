namespace Castle.DynamicProxy.Builder.CodeBuilder
{
    using System;
    using System.Collections;

    [CLSCompliant(false)]
    public class PropertiesCollection : CollectionBase
    {
        public void Add(EasyProperty property)
        {
            base.InnerList.Add(property);
        }
    }
}

