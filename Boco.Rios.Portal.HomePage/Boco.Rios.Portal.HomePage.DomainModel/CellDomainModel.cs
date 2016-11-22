using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Boco.Rios.Portal.HomePage.Entity;
using Boco.Rios.Portal.HomePage.Persistence.DaoService;

namespace Boco.Rios.Portal.HomePage.DomainModel
{
    public class CellDomainModel
    {
        private static CellDomainModel instance;
        private static object syncObj = new object();
        private CellDaoService daoService = new CellDaoService();

        private CellDomainModel()
        {
        }

        public static CellDomainModel Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncObj)
                    {
                        if (instance == null)
                        {
                            instance = new CellDomainModel();
                        }
                    }
                }
                return instance;
            }
        }

        public IList<Cell> GetCellsByCondition(string name, int lac, int ci)
        {
            return daoService.GetCellsByCondition(name, lac, ci);
        }

        public IList<Cell> GetCellsByConditionForPaging(string name, int lac, int ci, int page, int pageSize)
        {
            return daoService.GetCellsByConditionForPaging(name, lac, ci, page, pageSize);
        }

        public int GetCellsByConditionForCount(string name, int lac, int ci)
        {
            return daoService.GetCellsByConditionForCount(name, lac, ci);
        }
    }
}
