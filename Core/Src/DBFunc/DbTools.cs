using Core.Src.DBFunc.DBTables;
using Core.Src.DBFunc.SQLiteHelper;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Src.DBFunc
{
    internal class DbTools
    {
        private readonly string _dbFileName = "Krdb.db";
        public static DbTools Instance { get; private set; } = new();

        private SqliteConnection? _dbConnection = null;

        private SqliteConnection ConnectToDataBase()
        {
            return new SqliteConnection(string.Format("Data Source={0}", _dbFileName));
        }
        /// <summary>
        /// * 创建表格
        /// </summary>
        /// <param name="tableEntry"></param>
        public void CreateTable(DbTableBase tableEntry)
        {
            if (!tableEntry.CheckIsValid()) { return; }

            StringBuilder tableColumnsBuilder = new();
            foreach (var column in tableEntry.ColumnsName!) {
                tableColumnsBuilder.AppendFormat("{0},", column.ToString());
            }
            tableColumnsBuilder.Remove(tableColumnsBuilder.Length - 1, 1);

            using (_dbConnection = ConnectToDataBase())
            {
                _dbConnection.Open();

                StringBuilder sqlBuilder = new();
                sqlBuilder.Append("create table if not exists ");
                sqlBuilder.Append(tableEntry.TableName);
                sqlBuilder.Append(string.Format("({0})", tableColumnsBuilder.ToString()));

                var command = _dbConnection.CreateCommand();
                command.CommandText = sqlBuilder.ToString();
                _ = command.ExecuteNonQuery();
            }
        }
        /// <summary>
        /// * 插入数据
        /// </summary>
        /// <param name="tableEntry">Table Entry</param>
        /// <param name="values">VALUES（字符串类型）</param>
        public void InsertDataFromTable(DbTableBase tableEntry, List<string> values) {
            if (!tableEntry.CheckIsValid()) { return; }

            StringBuilder tableColumnsBuilder = new();
            foreach (var column in tableEntry.ColumnsName!) {
                tableColumnsBuilder.AppendFormat("{0},", column.ColumnName);
            }
            tableColumnsBuilder.Remove(tableColumnsBuilder.Length - 1, 1);

            using (_dbConnection = ConnectToDataBase()) {
                _dbConnection.Open();

                foreach (var value in values) {
                    StringBuilder sqlBuilder = new();
                    sqlBuilder.Append("insert into ");
                    sqlBuilder.Append(tableEntry.TableName + " ");
                    sqlBuilder.Append(string.Format("({0}) values ({1})", tableColumnsBuilder.ToString(), value));

                    var command = _dbConnection.CreateCommand();
                    command.CommandText = sqlBuilder.ToString();
                    _ = command.ExecuteNonQuery();
                }
            }
        }
    }
}
