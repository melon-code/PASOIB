using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.Data;

namespace SecurityAPI {
    public class DataBase {
        static string CheckMultipleWordName(string name) {
            if (name.Contains(" "))
                return "[" + name + "]";
            return name;
        }

        readonly SQLiteConnection connection;
        readonly SQLiteCommand command;

        public DataBase(string SQLiteDataBasePath) {
            connection = new SQLiteConnection("Data Source=" + SQLiteDataBasePath + ";Version=3;");
            connection.Open();
            command = new SQLiteCommand(connection);
        }

        public DataTable ExecuteSelectCommand(string commandText) {
            if (commandText.Contains("SELECT")) {
                DataTable data = new DataTable();
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(commandText, connection);
                adapter.Fill(data);
                return data;
            }
            return null;
        }
        public void ExecuteCommand(string commandText) {
            if (!commandText.Contains("SELECT")) {
                command.CommandText = commandText;
                command.ExecuteNonQuery();
            }
        }
        public void CreateTable(string tableName, string tableFields) { //may be return bool
            ExecuteCommand(string.Format("CREATE TABLE IF NOT EXISTS {0} ({1})", CheckMultipleWordName(tableName), tableFields));
        }
        public DataTable ReadAllData(string tableName) {
            return ReadData(null, tableName, null);
        }
        public DataTable ReadData(string fieldNames, string tableName) {
             return ReadData(fieldNames, tableName, null);
        }
        public DataTable ReadData(string fieldNames, string tableName, string selectCondition) {
            string query = string.Format("SELECT {0} FROM {1}", string.IsNullOrEmpty(fieldNames) ? "*" : fieldNames, CheckMultipleWordName(tableName));
            if (!string.IsNullOrEmpty(selectCondition))
                query += " WHERE " + selectCondition;
            return ExecuteSelectCommand(query);
        }
        public void InsertData(string tableName, string fieldValues) {
            ExecuteCommand(string.Format("INSERT INTO {0} VALUES({1})", CheckMultipleWordName(tableName), fieldValues));
        }
    }
}
