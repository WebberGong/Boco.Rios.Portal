using System.ComponentModel.Composition;
using Boco.Rios.Portal.Report.Data;
using Boco.Rios.Portal.Report.DomainModel;

namespace Boco.Rios.Portal.Report.Service
{
    [Export("Service", typeof (ServiceStack.ServiceInterface.Service))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class ReportService : ServiceStack.ServiceInterface.Service
    {
        private readonly ReportDomainModel _reportDomainModel = new ReportDomainModel();

        public ReportResponse Post(ReportRequest request)
        {
            return new ReportResponse {Data = _reportDomainModel.GetReports(request.Name)};
        }

        public ReportResponse Get(ReportRequest request)
        {
            return new ReportResponse {Data = _reportDomainModel.GetReports(request.Name)};
        }
    }
}