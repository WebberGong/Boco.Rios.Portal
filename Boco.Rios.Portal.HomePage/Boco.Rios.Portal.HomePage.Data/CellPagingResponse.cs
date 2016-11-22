using System.Collections.Generic;
using Boco.Rios.Portal.HomePage.Entity;

namespace Boco.Rios.Portal.HomePage.Data
{
    public class CellPagingResponse
    {
        public CellPagingResponse()
        {
            Data = new List<Cell>();
            TotalCount = 0;
        }

        public IList<Cell> Data { get; set; }
        public int TotalCount { get; set; }
    }
}