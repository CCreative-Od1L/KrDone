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

            ColumnsName.Add(new SQLiteHelper.DataColumn(
                nameof(TodoInfoEntry.Id),
                SQLiteHelper.SqliteDataType.INTEGER,
                true,
                true));
            ColumnsName.Add(new SQLiteHelper.DataColumn(
                nameof(TodoInfoEntry.TodoDate), 
                SQLiteHelper.SqliteDataType.TEXT, 
                false, 
                true));
            ColumnsName.Add(new SQLiteHelper.DataColumn(
                nameof(TodoInfoEntry.IsDone),
                SQLiteHelper.SqliteDataType.INTEGER,
                false,
                true));
            ColumnsName.Add(new SQLiteHelper.DataColumn(
                nameof(TodoInfoEntry.TodoContent),
                SQLiteHelper.SqliteDataType.TEXT,
                false,
                false));
        }

        public void InsertDataIntoTable(List<TodoInfoEntry> dataList) {
            List<string> strings = [];
            for(int i = 0; i < dataList.Count; ++i) {
                strings.Add(dataList[i].ToString());
            }
            DbTools.Instance.InsertDataFromTable(this, strings);
        }
    }
}
