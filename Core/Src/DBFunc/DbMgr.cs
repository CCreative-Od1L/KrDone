using Core.Src.DBFunc.DBTables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Src.DBFunc {
    public static class DbMgr {
        public static TodoInfoTable TodoInfo { get; private set; } = new();

        public static void CreateTable() {
            DbTools.Instance.CreateTable(TodoInfo);
        }
    }
}
