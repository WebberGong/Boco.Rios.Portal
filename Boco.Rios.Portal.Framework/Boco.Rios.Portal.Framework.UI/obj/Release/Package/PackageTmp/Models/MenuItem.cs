﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Boco.Rios.Portal.Framework.UI.Models
{
    public class MenuItem
    {
        public string Text { get; set; }
        public string Url { get; set; }
        public decimal Order { get; set; }
        public string Owner { get; set; }
    }
}