using Core.Src.DBFunc.DBEntiries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Src.DBFunc.DBTables {
    public class TodoInfoTable : DbTableBase {
        public TodoInfoTable() {
            // * 数据库表名称
            TableName = "TodoInfo";
            // * 添加元数据项目
            DbEntryMeta = new TodoInfoEntry();
            // * 数据库列添加
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
        /// <summary>
        /// * 插入数据
        /// </summary>
        /// <param name="dataList"></param>
        public void InsertDataIntoTable(List<TodoInfoEntry> dataList) {
            List<string> entryValues = [];
            for(int i = 0; i < dataList.Count; ++i) {
                entryValues.Add(dataList[i].ToString());
            }
            DbTools.Instance.InsertDataFromTable(this, entryValues);
        }
        /// <summary>
        /// * 删除数据
        /// </summary>
        /// <param name="dataList"></param>
        public void DeleteDataFromTable(List<TodoInfoEntry> dataList) {
            List<string> entryPrimeConditions = [];
            StringBuilder sb = new();
            var primeKeys = GetTablePrimeKey().ToArray();
            for(int i = 0; i < dataList.Count; ++i) {
                var primeKeyValues = dataList[i].GetPrimeKeyString();
                for(int j = 0; j < primeKeyValues.Count; ++j) {
                    sb.Append(string.Format("{0} = {1} ", primeKeys[j].ColumnName, primeKeyValues[j]));
                    if(j != primeKeyValues.Count - 1) {
                        sb.Append("AND ");
                    }
                }
                entryPrimeConditions.Add(sb.ToString());
                sb.Clear();
            }
            DbTools.Instance.DeleteDataFromTable(this, entryPrimeConditions);
        }
        public List<TodoInfoEntry> QueryDataFromTable(List<string> queryCondSql, List<object> queryCondValue) {
            List<TodoInfoEntry> queryRes = [];
            foreach(var item in DbTools.Instance.QueryObjectFromTable(this, queryCondSql, queryCondValue)) {
                queryRes.Add((TodoInfoEntry)item);
            }
            return queryRes;
        }
    }
}
