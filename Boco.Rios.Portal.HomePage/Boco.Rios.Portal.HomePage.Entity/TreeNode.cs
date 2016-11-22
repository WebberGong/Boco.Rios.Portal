using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Boco.Rios.Portal.HomePage.Entity
{
    public class TreeNode
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IList<TreeNode> ChildNodes { get; set; }
    }
}
