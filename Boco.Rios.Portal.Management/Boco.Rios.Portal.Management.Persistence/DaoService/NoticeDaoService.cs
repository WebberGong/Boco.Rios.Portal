using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Collections;
using Boco.Rios.Framework.Persistence;
using Boco.Rios.Portal.Management.Entity;
using Boco.Rios.Portal.Management.Persistence.IDao;

namespace Boco.Rios.Portal.Management.Persistence.DaoService
{
    public class NoticeDaoService : DAOServiceBase<INoticeDao>
    {
        public IList<Notice> GetNoticeByCondition(string title, string content)
        {
            return Dao.GetNoticeByCondition(title, content);
        }

        public IList<Notice> GetLatestNotice(int topN)
        {
            return Dao.GetLatestNotice(topN);
        }

        public Notice GetNoticeById(string id)
        {
            return Dao.GetNoticeById(id);
        }

        public object InsertNotice(Notice notice)
        {
            return Dao.InsertNotice(notice);
        }

        public int UpdateNotice(Notice notice)
        {
            return Dao.UpdateNotice(notice);
        }

        public int DeleteNotice(Notice notice)
        {
            return Dao.DeleteNotice(notice);
        }

        public object[] BatchDeleteNotice(IList<object> ids)
        {
            return Dao.BatchDeleteNotice(ids);
        }
    }
}
