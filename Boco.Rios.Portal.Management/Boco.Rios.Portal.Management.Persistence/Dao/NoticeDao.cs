﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;
using Boco.Rios.Framework.Persistence;
using Boco.Rios.Portal.Management.Entity;
using Boco.Rios.Portal.Management.Persistence.IDao;

namespace Boco.Rios.Portal.Management.Persistence.Dao
{
    public class NoticeDao : BaseSqlMapDao, INoticeDao
    {
        public IList<Notice> GetNoticeByCondition(string title, string content)
        {
            var ht = new Hashtable();
            ht.Add("Title", title);
            ht.Add("Content", content);
            return ExecuteQueryForList<Notice>("GetNoticeByCondition", ht);
        }

        public IList<Notice> GetLatestNotice(int topN)
        {
            return ExecuteQueryForList<Notice>("GetLatestNotice", topN);
        }

        public Notice GetNoticeById(string id)
        {
            return ExecuteQueryForObject<Notice>("GetNoticeById", id);
        }

        public object InsertNotice(Notice notice)
        {
            object result = ExecuteInsert("InsertNotice", notice);
            return result;
        }

        public int UpdateNotice(Notice notice)
        {
            int result = ExecuteUpdate("UpdateNotice", notice);
            return result;
        }

        public int DeleteNotice(Notice notice)
        {
            int result = ExecuteDelete("DeleteNotice", notice);
            return result;
        }

        public object[] BatchDeleteNotice(IList<object> ids)
        {
            IList<string> sqls = new List<string>();
            foreach (var id in ids)
            {
                sqls.Add("DeleteNotice");
            }
            return ExecuteBatchDelete(sqls, ids);
        }
    }
}