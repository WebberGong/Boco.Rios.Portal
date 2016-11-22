using System.ComponentModel;
using Boco.Rios.Portal.Management.Data;
using Boco.Rios.Portal.Management.DomainModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

namespace Boco.Rios.Portal.Management.Service
{
    [Export("Service", typeof(ServiceStack.ServiceInterface.Service))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class ManagementService : ServiceStack.ServiceInterface.Service
    {
        public NoticeListResponse Any(GetNoticeByConditionRequest request)
        {
            if (request.Title != null)
            {
                request.Title = request.Title.Trim();
            }
            if (request.Content != null)
            {
                request.Content = request.Content.Trim();
            }
            var result = NoticeDomainModel.Instance.GetNoticeByCondition(request.Title, request.Content);
            return new NoticeListResponse() { Data = result, TotalCount = result.Count };
        }

        public NoticeListResponse Any(GetLatestNoticeRequest request)
        {
            var result = NoticeDomainModel.Instance.GetLatestNotice(request.TopN);
            return new NoticeListResponse() { Data = result, TotalCount = result.Count };
        }

        public NoticeResponse Any(GetNoticeByIdRequest request)
        {
            return new NoticeResponse { Data = NoticeDomainModel.Instance.GetNoticeById(request.Id) };
        }

        public object Any(UpdateNoticeRequest request)
        {
            return NoticeDomainModel.Instance.UpdateNotice(request);
        }

        public object Any(InsertNoticeRequest request)
        {
            return NoticeDomainModel.Instance.InsertNotice(request);
        }

        public object Any(DeleteNoticeRequest request)
        {
            return NoticeDomainModel.Instance.DeleteNotice(request);
        }

        public object[] Any(BatchDeleteNoticeRequest request)
        {
            IList<object> param = new List<object>();
            foreach (var id in request.Ids)
            {
                param.Add(id);
            }
            return NoticeDomainModel.Instance.BatchDeleteNotice(param);
        }
    }
}
