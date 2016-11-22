using Boco.Rios.Portal.Management.Entity;

namespace Boco.Rios.Portal.Management.Data
{
    public class GetNoticeByConditionRequest
    {
        public string Title { get; set; }
        public string Content { get; set; }
    }

    public class GetLatestNoticeRequest
    {
        public int TopN { get; set; }
    }

    public class GetNoticeByIdRequest
    {
        public string Id { get; set; }
    }

    public class UpdateNoticeRequest : Notice
    {
    }

    public class InsertNoticeRequest : Notice
    {
    }

    public class DeleteNoticeRequest : Notice
    {
    }

    public class BatchDeleteNoticeRequest
    {
        public string[] Ids { get; set; }
    }
}