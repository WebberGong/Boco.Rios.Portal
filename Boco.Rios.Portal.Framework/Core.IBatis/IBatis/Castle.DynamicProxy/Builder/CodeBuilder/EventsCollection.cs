namespace Castle.DynamicProxy.Builder.CodeBuilder
{
    using System;
    using System.Collections;

    [CLSCompliant(false)]
    public class EventsCollection : CollectionBase
    {
        public void Add(EasyEvent easyEvent)
        {
            base.InnerList.Add(easyEvent);
        }
    }
}

