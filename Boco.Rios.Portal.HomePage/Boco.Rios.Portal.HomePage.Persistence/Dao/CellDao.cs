using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;
using Boco.Rios.Framework.Persistence;
using Boco.Rios.Portal.HomePage.Entity;
using Boco.Rios.Portal.HomePage.Persistence.IDao;

namespace Boco.Rios.Portal.HomePage.Persistence.Dao
{
    public class CellDao : BaseSqlMapDao, ICellDao
    {
        public IList<Cell> GetCellsByCondition(string name, int lac, int ci)
        {
            Hashtable ht = new Hashtable();
            ht.Add("Name", name);
            ht.Add("Lac", lac);
            ht.Add("Ci", ci);
            return ExecuteQueryForList<Cell>("GetCellsByCondition", ht);
        }

        public IList<Cell> GetCellsByConditionForPaging(string name, int lac, int ci, int page, int pageSize)
        {
            Hashtable ht = new Hashtable();
            ht.Add("Name", name);
            ht.Add("Lac", lac);
            ht.Add("Ci", ci);
            ht.Add("Page", page);
            ht.Add("PageSize", pageSize);
            return ExecuteQueryForList<Cell>("GetCellsByConditionForPaging", ht);
        }

        public int GetCellsByConditionForCount(string name, int lac, int ci)
        {
            Hashtable ht = new Hashtable();
            ht.Add("Name", name);
            ht.Add("Lac", lac);
            ht.Add("Ci", ci);
            DataTable dt = ExecuteQueryForDataTable("GetCellsByConditionForCount", ht);
            return int.Parse(dt.Rows[0][0].ToString());
        }
    }
}
