using Core.Src.DBFunc.SQLiteHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Src.DBFunc.DBEntiries {
    public class DbEntityBase {
        public List<(string, SqliteDataType)> columnsName;
    }
}
