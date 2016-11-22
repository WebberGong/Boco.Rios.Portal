using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Boco.Rios.Portal.HomePage.Entity;

namespace Boco.Rios.Portal.HomePage.DomainModel
{
    public class TreeNodeDomainModel
    {
        private static TreeNodeDomainModel instance;
        private static object syncObj = new object();

        private TreeNodeDomainModel()
        {
        }

        public static TreeNodeDomainModel Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncObj)
                    {
                        if (instance == null)
                        {
                            instance = new TreeNodeDomainModel();
                        }
                    }
                }
                return instance;
            }
        }

        public IList<TreeNode> GetTreeNodes()
        {
            IList<TreeNode> result = new List<TreeNode>();
            result.Add(new TreeNode() { Id = 1, Name = "A1", 
                ChildNodes = new List<TreeNode>()
                {
                    new TreeNode(){ Id = 11, Name = "A11" },
                    new TreeNode(){ Id = 12, Name = "A12" },
                    new TreeNode(){ Id = 13, Name = "A13" }
                }});
            result.Add(new TreeNode() { Id = 2, Name = "A2", 
                ChildNodes = new List<TreeNode>()
                {
                    new TreeNode(){ Id = 21, Name = "A21" },
                    new TreeNode(){ Id = 22, Name = "A22" },
                    new TreeNode(){ Id = 23, Name = "A23" }
                }});
            result.Add(new TreeNode() { Id = 3, Name = "A3" });
            result.Add(new TreeNode() { Id = 4, Name = "A4" });
            result.Add(new TreeNode() { Id = 5, Name = "A5" });
            return result;
        }
    }
}
