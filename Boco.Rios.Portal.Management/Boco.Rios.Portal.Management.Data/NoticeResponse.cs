using Boco.Rios.Portal.Management.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.ServiceInterface.ServiceModel;

namespace Boco.Rios.Portal.Management.Data
{
    public class NoticeListResponse : ListResultResponse<Notice>
    {
    }

    public class NoticeResponse : ResultResponse<Notice>
    {
    }

    public class ResultResponse<T>
    {
        public T Data { get; set; }
        public ResponseStatus ResponseStatus { get; set; }
    }

    public class ListResultResponse<T>
    {
        public IList<T> Data { get; set; }
        public int TotalCount { get; set; }
        public ResponseStatus ResponseStatus { get; set; }
    }
}
