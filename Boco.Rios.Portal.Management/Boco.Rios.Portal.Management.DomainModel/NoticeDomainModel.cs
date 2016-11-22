using Boco.Rios.Portal.Management.Entity;
using Boco.Rios.Portal.Management.Persistence.DaoService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Boco.Rios.Portal.Management.DomainModel
{
    public class NoticeDomainModel
    {
        private static NoticeDomainModel instance;
        private static object syncObj = new object();
        private NoticeDaoService daoService = new NoticeDaoService();

        private NoticeDomainModel()
        {
        }

        public static NoticeDomainModel Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncObj)
                    {
                        if (instance == null)
                        {
                            instance = new NoticeDomainModel();
                        }
                    }
                }
                return instance;
            }
        }

        public IList<Notice> GetNoticeByCondition(string title, string content)
        {
            return daoService.GetNoticeByCondition(title, content);
        }

        public IList<Notice> GetLatestNotice(int topN)
        {
            return daoService.GetLatestNotice(topN);
        }

        public Notice GetNoticeById(string id)
        {
            return daoService.GetNoticeById(id);
        }

        public object InsertNotice(Notice request)
        {
            return daoService.InsertNotice(request);
        }

        public int UpdateNotice(Notice request)
        {
            return daoService.UpdateNotice(request);
        }

        public int DeleteNotice(Notice request)
        {
            return daoService.DeleteNotice(request);
        }

        public object[] BatchDeleteNotice(IList<object> ids)
        {
            return daoService.BatchDeleteNotice(ids);
        }
    }
}
