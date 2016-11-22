using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Boco.Rios.Portal.HomePage.Entity;

namespace Boco.Rios.Portal.HomePage.Data
{
    public class TreeNodeResponse
    {
        public TreeNodeResponse()
        {
            Data = new List<TreeNode>();
        }
        public IList<TreeNode> Data { get; set; }
        public int TotalCount
        {
            get
            {
                return Data == null ? 0 : Data.Count;
            }
        }
    }
}
