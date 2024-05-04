using Core.Src.DBFunc.SQLiteHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Src.DBFunc.DBTables {
    public abstract class DbTableBase {
        public string TableName { get; set; } = string.Empty;
        public abstract DBEntiries.DbEntryBase? DbEntryMeta { get; set; }
        public List<DataColumn> ColumnsName { get; set; } = [];
        /// <summary>
        /// * 检查当前表格是否有效
        /// </summary>
        /// <returns></returns>
        public bool CheckIsValid() {
            return !string.IsNullOrEmpty(TableName) || ColumnsName.Count == 0;
        }
        /// <summary>
        /// * 获取当前表格的主键
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DataColumn> GetTablePrimeKey() {
            return from columnName in ColumnsName where columnName.IsPrimaryKey select columnName;
        }
    }
}
