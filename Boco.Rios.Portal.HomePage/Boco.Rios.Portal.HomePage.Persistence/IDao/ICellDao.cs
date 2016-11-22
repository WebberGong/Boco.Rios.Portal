﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;
using Boco.Rios.Portal.HomePage.Entity;

namespace Boco.Rios.Portal.HomePage.Persistence.IDao
{
    public interface ICellDao
    {
        IList<Cell> GetCellsByCondition(string name, int lac, int ci);

        IList<Cell> GetCellsByConditionForPaging(string name, int lac, int ci, int page, int pageSize);

        int GetCellsByConditionForCount(string name, int lac, int ci);
    }
}