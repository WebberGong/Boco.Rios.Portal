namespace Castle.DynamicProxy.Builder.CodeGenerators
{
    using System;
    using System.Collections;

    internal class Set : DictionaryBase
    {
        public void Add(object item)
        {
            if (!base.Dictionary.Contains(item))
            {
                base.Dictionary.Add(item, string.Empty);
            }
        }

        public void AddArray(object[] items)
        {
            foreach (object obj2 in items)
            {
                this.Add(obj2);
            }
        }

        public void Remove(object item)
        {
            base.Dictionary.Remove(item);
        }

        public Array ToArray(Type elementType)
        {
            Array array = Array.CreateInstance(elementType, base.Dictionary.Keys.Count);
            int num = 0;
            foreach (object obj2 in base.Dictionary.Keys)
            {
                array.SetValue(obj2, num++);
            }
            return array;
        }
    }
}

