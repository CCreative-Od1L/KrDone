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
    public class DbTools
    {
        private readonly string _dbFileName = "Krdb.db";
        public static DbTools Instance { get; private set; } = new();

        private SqliteConnection? _dbConnection = null;

        private SqliteConnection ConnectToDataBase()
        {
            return new SqliteConnection(string.Format("Data Source={0}", _dbFileName));
        }

        public void CreateTable(DbTableBase tableEntry)
        {
            if (!tableEntry.CheckIsValid()) { return; }

            StringBuilder tableColumnsBuilder = new();
            foreach (var (name, type) in tableEntry.ColumnsName!) {
                tableColumnsBuilder.AppendFormat("{0} {1},", name, type);
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
        public void InsertDataFromTable(DbTableBase tableEntry, List<string> datas) {
            if (!tableEntry.CheckIsValid()) { return; }

            StringBuilder tableColumnsBuilder = new();
            foreach (var (name, _) in tableEntry.ColumnsName!) {
                tableColumnsBuilder.AppendFormat("{0},", name);
            }
            tableColumnsBuilder.Remove(tableColumnsBuilder.Length - 1, 1);

            using (_dbConnection = ConnectToDataBase()) {
                _dbConnection.Open();

                foreach (var data in datas) {
                    StringBuilder sqlBuilder = new();
                    sqlBuilder.Append("insert into ");
                    sqlBuilder.Append(tableEntry.TableName + " ");
                    sqlBuilder.Append(string.Format("({0}) values ({1})", tableColumnsBuilder.ToString(), data));

                    var command = _dbConnection.CreateCommand();
                    command.CommandText = sqlBuilder.ToString();
                    _ = command.ExecuteNonQuery();
                }
            }
        }
    }
}
