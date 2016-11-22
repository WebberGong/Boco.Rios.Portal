using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Boco.Rios.Portal.Report.Data
{
    public class ReportResponse
    {
        public ReportResponse()
        {
            Data = new List<Entity.Report>();
        }
        public IList<Entity.Report> Data { get; set; }
        public int TotalCount
        {
            get
            {
                return Data == null ? 0 : Data.Count;
            }
        }
    }
}
