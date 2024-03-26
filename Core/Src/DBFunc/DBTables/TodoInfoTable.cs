using Core.Src.DBFunc.DBEntiries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Src.DBFunc.DBTables {
    public class TodoInfoTable : DbTableBase {
        public TodoInfoTable() {
            TableName = "TodoInfo";
            ColumnsName = [
                (nameof(TodoInfoEntry.TodoDate), SQLiteHelper.SqliteDataType.TEXT),
                (nameof(TodoInfoEntry.TodoContent), SQLiteHelper.SqliteDataType.TEXT),
                (nameof(TodoInfoEntry.IsDone), SQLiteHelper.SqliteDataType.INTEGER),
            ];
        }
    }
}
