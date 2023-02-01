using System;
using System.Collections.Generic;
using System.Text;

namespace Xpos307.ViewModels
{
    public class VMPage
    {
        public string Name { get; set; }
        public string orderBy { get; set; }
        public int? pageNumber { get; set; }
        public int? showData { get; set; }
    }
}
