using System.Collections.Generic;
using Boco.Rios.Portal.HomePage.Entity;

namespace Boco.Rios.Portal.HomePage.Data
{
    public class CellResponse
    {
        public CellResponse()
        {
            Data = new List<Cell>();
        }

        public IList<Cell> Data { get; set; }

        public int TotalCount
        {
            get { return Data == null ? 0 : Data.Count; }
        }
    }
}