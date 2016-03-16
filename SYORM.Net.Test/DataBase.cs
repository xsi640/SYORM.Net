using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SYORM.Net.Factory;
using System.IO;

namespace SYORM.Net.UnitTest
{
    [TestClass]
    public class DataBase
    {
        [TestMethod]
        public void TestCreateDatabase()
        {
            if (File.Exists(@"d:\test_sqlite.db"))
                File.Delete(@"d:\test_sqlite.db");
            DBFactory.Connection.DbType = SYORM.Net.Enums.EDbType.SQLite;
            DBFactory.Connection.IsSingleConnection = true;
            DBFactory.Connection.DbFilePath = @"d:\test_sqlite.db";
            DBFactory.CreateDataBase();
            DBFactory.Connection.ConnectionClose();
            System.Diagnostics.Trace.Assert(true);


            if (File.Exists(@"d:\test_sqlce.db"))
                File.Delete(@"d:\test_sqlce.db");
            DBFactory.Connection.DbType = SYORM.Net.Enums.EDbType.SqlServerCe;
            DBFactory.Connection.IsSingleConnection = true;
            DBFactory.Connection.DbFilePath = @"d:\test_sqlite.db";
            DBFactory.CreateDataBase();
            DBFactory.Connection.ConnectionClose();
            System.Diagnostics.Trace.Assert(true);
        }

        [TestMethod]
        public void TestCreateTable()
        {
            TestCreateDatabase();

            DBFactory.CreateTable(typeof(Person));
            DBFactory.Connection.ConnectionClose();
            System.Diagnostics.Trace.Assert(true);
        }
    }
}
