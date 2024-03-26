using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Src.DBFunc.DBEntiries {
    public class TodoInfoEntry {
        public DateOnly TodoDate { get; set; }
        public bool IsDone { get; set; } = false;
        public string TodoContent { get; set; } = string.Empty;
    }
}
