using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Boco.Rios.Portal.Management.Entity
{
    public class Notice
    {
        public bool Selected { get; set; }
        public string Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string StartValidTime { get; set; }
        public string EndValidTime { get; set; }
        public string ReleaseTime { get; set; }
        public string ReleaseUserId { get; set; }
        public bool ReleaseState { get; set; }
        public string CreateTime { get; set; }
        public string CreateUserId { get; set; }
    }
}
