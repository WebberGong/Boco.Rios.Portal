using System.Collections.Generic;

namespace Boco.Rios.Portal.Report.DomainModel
{
    public class ReportDomainModel
    {
        public IList<Entity.Report> GetReports(string name)
        {
            IList<Entity.Report> result = new List<Entity.Report>();
            for (var i = 0; i < 1000; i++)
            {
                result.Add(new Entity.Report {Name = name + (i + 1)});
            }
            return result;
        }
    }
}