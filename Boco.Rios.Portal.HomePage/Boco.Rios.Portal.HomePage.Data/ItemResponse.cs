using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Boco.Rios.Portal.HomePage.Entity;

namespace Boco.Rios.Portal.HomePage.Data
{
    public class ItemResponse
    {
        public ItemResponse()
        {
            Data = new List<Item>();
        }
        public IList<Item> Data { get; set; }
        public int TotalCount
        {
            get
            {
                return Data == null ? 0 : Data.Count;
            }
        }
    }
}
