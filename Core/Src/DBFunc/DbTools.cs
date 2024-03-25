using Core.Src.DBFunc.DBEntiries;
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

        public void CreateTable(string tableName, DbEntityBase tableColumns)
        {
            StringBuilder tableColumnsBuilder = new();
            foreach (var (name, type) in tableColumns.columnsName) {
                tableColumnsBuilder.AppendFormat("{0} {1},", name, type);
            }
            tableColumnsBuilder.Remove(tableColumnsBuilder.Length - 2, 1);

            using (_dbConnection = ConnectToDataBase())
            {
                _dbConnection.Open();

                StringBuilder sqlBuilder = new();
                sqlBuilder.Append("create table if not exists ");
                sqlBuilder.Append(tableName);
                sqlBuilder.Append(string.Format("({0})", tableColumnsBuilder.ToString()));

                var command = _dbConnection.CreateCommand();
                command.CommandText = sqlBuilder.ToString();
                _ = command.ExecuteNonQuery();
            }
        }

    }
}
