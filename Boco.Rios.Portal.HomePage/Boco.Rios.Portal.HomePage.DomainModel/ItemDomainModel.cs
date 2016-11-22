using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Boco.Rios.Portal.HomePage.Entity;

namespace Boco.Rios.Portal.HomePage.DomainModel
{
    public class ItemDomainModel
    {
        private static ItemDomainModel instance;
        private static object syncObj = new object();

        private ItemDomainModel()
        {
        }

        public static ItemDomainModel Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncObj)
                    {
                        if (instance == null)
                        {
                            instance = new ItemDomainModel();
                        }
                    }
                }
                return instance;
            }
        }

        public IList<Item> GetItems()
        {
            IList<Item> result = new List<Item>();
            result.Add(new Item() { Name = "A1", Value = 1 });
            result.Add(new Item() { Name = "A2", Value = 2 });
            result.Add(new Item() { Name = "B1", Value = 3 });
            result.Add(new Item() { Name = "B2", Value = 4 });
            result.Add(new Item() { Name = "C1", Value = 5 });
            result.Add(new Item() { Name = "C2", Value = 6 });
            return result;
        }
    }
}
