using Core.Src.DBFunc.SQLiteHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Src.DBFunc.DBTables {
    public class DbTableBase {
        public string TableName { get; set; } = string.Empty;
        public List<(string, SqliteDataType)> ColumnsName { get; set; } = [];
        public bool CheckIsValid() {
            return !string.IsNullOrEmpty(TableName) || ColumnsName.Count == 0;
        }
    }
}
