using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Collections;
using Boco.Rios.Framework.Persistence;
using Boco.Rios.Portal.HomePage.Entity;
using Boco.Rios.Portal.HomePage.Persistence.IDao;

namespace Boco.Rios.Portal.HomePage.Persistence.DaoService
{
    public class CellDaoService : DAOServiceBase<ICellDao>
    {
        public IList<Cell> GetCellsByCondition(string name, int lac, int ci)
        {
            return Dao.GetCellsByCondition(name, lac, ci);
        }

        public IList<Cell> GetCellsByConditionForPaging(string name, int lac, int ci, int page, int pageSize)
        {
            return Dao.GetCellsByConditionForPaging(name, lac, ci, page, pageSize);
        }

        public int GetCellsByConditionForCount(string name, int lac, int ci)
        {
            return Dao.GetCellsByConditionForCount(name, lac, ci);
        }
    }
}
