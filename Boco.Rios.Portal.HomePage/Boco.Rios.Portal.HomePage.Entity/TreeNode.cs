using System.Collections.Generic;

namespace Boco.Rios.Portal.HomePage.Entity
{
    public class TreeNode
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IList<TreeNode> ChildNodes { get; set; }
    }
}