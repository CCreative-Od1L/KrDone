using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Src.DBFunc.SQLiteHelper {
    public readonly struct DataColumn {
        public DataColumn() { }

        public DataColumn(string name, SqliteDataType type, bool isPrime, bool isNotNull) {
            ColumnName = name;
            ColumnType = type;
            IsPrimaryKey = isPrime;
            IsNotNull = isNotNull;
        }

        // * 项名称
        public string ColumnName { get; } = string.Empty;
        // * 项数据类型
        public SqliteDataType ColumnType { get; } = SqliteDataType.INTEGER;
        // * 是否为主键
        public bool IsPrimaryKey { get; } = false;
        // * 是否非空
        public bool IsNotNull { get; } = false;

        public override string ToString() {
            StringBuilder stringBuilder = new();
            stringBuilder.Append($"{ColumnName} {ColumnType} ");
            if(IsPrimaryKey) {
                stringBuilder.Append($"PRIMARY KEY ");
            }
            if(IsNotNull) {
                stringBuilder.Append($"NOT NULL ");
            }
            return stringBuilder.ToString();
        }
    }
}
