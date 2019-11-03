using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecurityAPI;

namespace APITests {
    [TestClass]
    public class DataBaseTests {
        static void CheckFirstString(DataRow row) {
            Assert.AreEqual((long)1, row.ItemArray[0]);
            Assert.AreEqual("Строка", row.ItemArray[1] as string);
        }
        static void CheckSecondString(DataRow row) {
            Assert.AreEqual((long)2, row.ItemArray[0]);
            Assert.AreEqual("Вторая строка", row.ItemArray[1] as string);
        }

        [TestMethod]
        public void CreateTable() {
            const string tableName = "Тестовая таблица";
            DataBase db = new DataBase("createTable.sqlite");
            db.CreateTable(tableName, "[Первичный ключ] INTEGER PRIMARY KEY, Строка STRING");
            DataTable data = db.ExecuteSelectCommand("SELECT tbl_name FROM sqlite_master WHERE type='table'");
            Assert.AreEqual(1, data.Rows.Count);
            Assert.AreEqual(tableName, data.Rows[0].ItemArray[0] as string);
            db.ExecuteCommand(string.Format("DROP TABLE [{0}]", tableName));
        }

        [TestMethod]
        public void ReadDataWithoutCondition() {
            DataBase db = new DataBase("readData.sqlite");
            DataTable data = db.ReadData("Ключ, [Строковое поле]", "Таблица");
            var rows = data.Rows;
            Assert.AreEqual(2, rows.Count);
            CheckFirstString(rows[0]);
            CheckSecondString(rows[1]);
        }

        [TestMethod]
        public void ReadDataWithCondition() {
            DataBase db = new DataBase("readData.sqlite");
            DataTable data = db.ReadData(null, "Таблица", "Ключ=2");
            var rows = data.Rows;
            Assert.AreEqual(1, rows.Count);
            CheckSecondString(rows[0]);
            Assert.AreEqual(0.33, (double)rows[0].ItemArray[2]);
        }

        [TestMethod]
        public void ReadAllData() {
            DataBase db = new DataBase("readData.sqlite");
            DataTable data = db.ReadAllData("Таблица");
            var rows = data.Rows;
            Assert.AreEqual(2, rows.Count);
            CheckFirstString(rows[0]);
            Assert.AreEqual(1.55, rows[0].ItemArray[2]);
            CheckSecondString(rows[1]);
            Assert.AreEqual(0.33, (double)rows[1].ItemArray[2]);
        }

        [TestMethod]
        public void InsertData() {
            const string tableName = "Таблица";
            DataBase db = new DataBase("insertData.sqlite");
            db.InsertData(tableName, "'6', 'Rekt'");
            DataTable data = db.ReadAllData(tableName);
            Assert.AreEqual(1, data.Rows.Count);
            var row = data.Rows[0];
            Assert.AreEqual((long)6, row.ItemArray[0]);
            Assert.AreEqual("Rekt", row.ItemArray[1]);
            db.ExecuteCommand("DELETE FROM " + tableName);
        }
    }
}
