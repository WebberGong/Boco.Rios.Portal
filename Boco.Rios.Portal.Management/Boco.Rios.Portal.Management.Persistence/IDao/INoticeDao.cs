﻿using System.Collections.Generic;
using Boco.Rios.Portal.Management.Entity;

namespace Boco.Rios.Portal.Management.Persistence.IDao
{
    public interface INoticeDao
    {
        IList<Notice> GetNoticeByCondition(string title, string content);

        IList<Notice> GetLatestNotice(int topN);

        Notice GetNoticeById(string id);

        object InsertNotice(Notice notice);

        int UpdateNotice(Notice notice);

        int DeleteNotice(Notice notice);

        object[] BatchDeleteNotice(IList<object> ids);
    }
}