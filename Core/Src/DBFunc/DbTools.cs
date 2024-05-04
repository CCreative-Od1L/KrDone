using Core.Src.DBFunc.DBEntiries;
using Core.Src.DBFunc.DBTables;
using Core.Src.DBFunc.SQLiteHelper;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

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
        /// <summary>
        /// * 根据主键值删除表中数据
        /// </summary>
        /// <param name="TableEntry"></param>
        /// <param name="primeConditions"></param>
        public void DeleteDataFromTable(DbTableBase TableEntry, List<string> primeConditions) {
            if(!TableEntry.CheckIsValid()) { return; }

            using (_dbConnection = ConnectToDataBase()) {
                _dbConnection.Open();

                for(int i = 0; i < primeConditions.Count; ++i) {
                    StringBuilder sqlBuilder = new();
                    sqlBuilder.Append($"delete from {TableEntry.TableName} where {primeConditions[i]}");
                    var command = _dbConnection.CreateCommand();
                    command.CommandText = sqlBuilder.ToString();
                    _ = command.ExecuteNonQuery();
                }
            }
        }
        /// <summary>
        /// * 查询表中数据
        /// </summary>
        /// <param name="tableEntry"></param>
        /// <param name="queryCondSql"></param>
        /// <param name="queryCondValue"></param>
        /// <returns></returns>
        public List<DbEntryBase> QueryObjectFromTable(DbTableBase tableEntry, List<string> queryCondSql, List<object> queryCondValue) {
            if(!tableEntry.CheckIsValid()) { return []; }

            using (_dbConnection = ConnectToDataBase()) {
                _dbConnection.Open();
                // * 构建SQL语句
                StringBuilder sqlBuilder = new();
                sqlBuilder.Append($"select * from {tableEntry.TableName} where ");
                
                for(int i = 0; i < queryCondSql.Count; ++i) {
                    sqlBuilder.Append($"{queryCondSql[i]} = ${queryCondSql[i]}");
                }

                var command = _dbConnection.CreateCommand();
                command.CommandText = sqlBuilder.ToString();
                // * 数值填充
                for(int i = 0; i < queryCondValue.Count; ++i) {
                    command.Parameters.AddWithValue($"${queryCondSql[i]}", queryCondValue[i]);
                }
                // * 获取查询结果
                List<DbEntryBase> queryResult  = [];
                using (var reader = command.ExecuteReader()){
                    while (reader.Read()) {
                        // * 缓冲区
                        object[] entryValuesTmp = new object[tableEntry.ColumnsName.Count];
                        // * 获取原始数据
                        for(int i = 0; i < tableEntry.ColumnsName.Count; ++i) {
                            entryValuesTmp[i] = reader.GetValue(i);
                        }
                        // * 数据装填
                        tableEntry.DbEntryMeta!.PushValues(entryValuesTmp);
                        // * 将项添加到结果数组中
                        queryResult.Add(tableEntry.DbEntryMeta);
                    }
                }
                return queryResult;
            }
        }
    }
}
