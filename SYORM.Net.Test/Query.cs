using SYORM.Net.Factory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SYORM.Net.UnitTest
{
    [TestClass]
    public class Query
    {
        private void Init()
        {
            DBFactory.Connection.ConnectionClose();
            if (File.Exists(@"d:\test_sqlce.db"))
                File.Delete(@"d:\test_sqlce.db");
            DBFactory.Connection.DbType = SYORM.Net.Enums.EDbType.SqlServerCe;
            DBFactory.Connection.IsSingleConnection = true;
            DBFactory.Connection.DbFilePath = @"d:\test_sqlce.db";
            DBFactory.CreateDataBase();
            DBFactory.CreateTable(typeof(Person));
        }


        [TestMethod]
        public void Insert()
        {
            Init();

            Person p = new Person();
            p.Name = "张三";
            p.Age = 18;
            p.Address = "北京";
            int result = DBFactory.Query<Person>().Insert(p);
            Assert.AreEqual(result, 1);
        }

        [TestMethod]
        public void InsertAll()
        {
            Init();

            Person p = new Person();
            p.PersonId = Guid.NewGuid();
            p.Name = "张三";
            p.Age = 22;
            p.Address = "上海";

            Person p1 = new Person();
            p1.PersonId = Guid.NewGuid();
            p1.Name = "李四";
            p1.Age = 21;
            p1.Address = "上海";

            Person p2 = new Person();
            p2.PersonId = Guid.NewGuid();
            p2.Name = "李四";
            p2.Age = 18;
            p2.Address = "上海";

            IList<Person> lists = new List<Person>() { p, p1, p2 };
            Assert.AreEqual(DBFactory.Query<Person>().Insert(lists), 3);
            Assert.AreEqual(DBFactory.Query<Person>().CountAll(), 3);
        }

        [TestMethod]
        public void InsertOrUpdate()
        {
            Init();

            Guid id = Guid.NewGuid();
            Person p = new Person();
            p.PersonId = id;
            p.Name = "李四";
            p.Age = 21;
            p.Address = "上海";
            Assert.AreEqual(DBFactory.Query<Person>().Insert(p), 1);

            p.Name = "王五";
            Assert.AreEqual(DBFactory.Query<Person>().InsertOrUpdate(p), 1);

            p = DBFactory.Query<Person>().Get(id);
            Assert.AreEqual(p.Name, "王五");

            p.PersonId = Guid.NewGuid();
            Assert.AreEqual(DBFactory.Query<Person>().InsertOrUpdate(p), 1);

            Assert.AreEqual(DBFactory.Query<Person>().CountAll(), 2);
        }

        [TestMethod]
        public void InsertOrUpdateAll()
        {
            Init();

            Person p = new Person();
            p.PersonId = Guid.NewGuid();
            p.Name = "张三";
            p.Age = 22;
            p.Address = "上海";

            Person p1 = new Person();
            p1.PersonId = Guid.NewGuid();
            p1.Name = "李四";
            p1.Age = 21;
            p1.Address = "上海";

            Person p2 = new Person();
            p2.PersonId = Guid.NewGuid();
            p2.Name = "李四";
            p2.Age = 18;
            p2.Address = "上海";

            IList<Person> lists = new List<Person>() { p, p1, p2 };
            Assert.AreEqual(DBFactory.Query<Person>().Insert(lists), 3);

            p.Name = "abc";
            p1.Age = 100;
            p2.Address = "北京海淀";
            Assert.AreEqual(DBFactory.Query<Person>().InsertOrUpdate(lists), 3);
            Assert.AreEqual(DBFactory.Query<Person>().CountAll(), 3);

            lists = DBFactory.Query<Person>().SelectAll("Age DESC");
            Assert.AreEqual(lists[0].Name, "李四");
            Assert.AreEqual(lists[1].Age, 22);
            Assert.AreEqual(lists[2].Address, "北京海淀");
        }

        [TestMethod]
        public void Update()
        {
            Init();

            Guid id = Guid.NewGuid();
            Person p = new Person();
            p.PersonId = id;
            p.Name = "李四";
            p.Age = 21;
            p.Address = "上海";
            Assert.AreEqual(DBFactory.Query<Person>().Insert(p), 1);

            p.Name = "王五";
            Assert.AreEqual(DBFactory.Query<Person>().Update(p), 1);

            p = DBFactory.Query<Person>().Get(id);
            Assert.AreEqual(p.Name, "王五");
        }

        [TestMethod]
        public void Delete()
        {
            Init();

            Guid id = Guid.NewGuid();
            Person p = new Person();
            p.PersonId = id;
            p.Name = "李四";
            p.Age = 21;
            p.Address = "上海";
            Assert.AreEqual(DBFactory.Query<Person>().Insert(p), 1);
            Assert.AreEqual(DBFactory.Query<Person>().CountAll(), 1);

            Assert.AreEqual(DBFactory.Query<Person>().Delete(id), 1);
            Assert.AreEqual(DBFactory.Query<Person>().CountAll(), 0);
        }

        [TestMethod]
        public void DeleteAll()
        {
            Init();

            Person p = new Person();
            p.PersonId = Guid.NewGuid();
            p.Name = "李四";
            p.Age = 21;
            p.Address = "上海";
            Assert.AreEqual(DBFactory.Query<Person>().Insert(p), 1);

            Person p1 = new Person();
            p1.PersonId = Guid.NewGuid();
            p1.Name = "李四";
            p1.Age = 21;
            p1.Address = "上海";
            Assert.AreEqual(DBFactory.Query<Person>().Insert(p1), 1);

            Person p2 = new Person();
            p2.PersonId = Guid.NewGuid();
            p2.Name = "李四";
            p2.Age = 21;
            p2.Address = "上海";
            Assert.AreEqual(DBFactory.Query<Person>().Insert(p2), 1);

            Assert.AreEqual(DBFactory.Query<Person>().CountAll(), 3);

            Assert.AreEqual(DBFactory.Query<Person>().DeleteAll(), 3);

            Assert.AreEqual(DBFactory.Query<Person>().CountAll(), 0);
        }

        [TestMethod]
        public void DeleteByIdLists()
        {
            Init();

            Person p = new Person();
            p.PersonId = Guid.NewGuid();
            p.Name = "李四";
            p.Age = 21;
            p.Address = "上海";
            Assert.AreEqual(DBFactory.Query<Person>().Insert(p), 1);

            Person p1 = new Person();
            p1.PersonId = Guid.NewGuid();
            p1.Name = "李四";
            p1.Age = 21;
            p1.Address = "上海";
            Assert.AreEqual(DBFactory.Query<Person>().Insert(p1), 1);

            Person p2 = new Person();
            p2.PersonId = Guid.NewGuid();
            p2.Name = "李四";
            p2.Age = 21;
            p2.Address = "上海";
            Assert.AreEqual(DBFactory.Query<Person>().Insert(p2), 1);

            Assert.AreEqual(DBFactory.Query<Person>().CountAll(), 3);

            Assert.AreEqual(DBFactory.Query<Person>().Delete(new List<object>() { p.PersonId, p1.PersonId, p2.PersonId }), 3);

            Assert.AreEqual(DBFactory.Query<Person>().CountAll(), 0);
        }

        [TestMethod]
        public void SelectByWhere()
        {
            Init();

            Person p = new Person();
            p.PersonId = Guid.NewGuid();
            p.Name = "张三";
            p.Age = 22;
            p.Address = "上海";
            Assert.AreEqual(DBFactory.Query<Person>().Insert(p), 1);

            Person p1 = new Person();
            p1.PersonId = Guid.NewGuid();
            p1.Name = "李四";
            p1.Age = 21;
            p1.Address = "上海";
            Assert.AreEqual(DBFactory.Query<Person>().Insert(p1), 1);

            Person p2 = new Person();
            p2.PersonId = Guid.NewGuid();
            p2.Name = "李四";
            p2.Age = 18;
            p2.Address = "上海";
            Assert.AreEqual(DBFactory.Query<Person>().Insert(p2), 1);

            IList<Person> lists = DBFactory.Query<Person>().Select("Name='李四'", "Age ASC");
            Assert.AreEqual(lists.Count, 2);
            Assert.AreEqual(lists[0].Age, 18);
            Assert.AreEqual(lists[1].Age, 21);

            lists = DBFactory.Query<Person>().Select("Name='李四'", "Age DESC");
            Assert.AreEqual(lists.Count, 2);
            Assert.AreEqual(lists[0].Age, 21);
            Assert.AreEqual(lists[1].Age, 18);

            lists = DBFactory.Query<Person>().Select("Age >= 21", "Age ASC");
            Assert.AreEqual(lists.Count, 2);
            Assert.AreEqual(lists[0].Name, "李四");
            Assert.AreEqual(lists[1].Name, "张三");

            lists = DBFactory.Query<Person>().Select("Age >= 21", "Age DESC");
            Assert.AreEqual(lists.Count, 2);
            Assert.AreEqual(lists[0].Name, "张三");
            Assert.AreEqual(lists[1].Name, "李四");

            lists = DBFactory.Query<Person>().Select("Age >= 21", 0, 1, "Age DESC");
            Assert.AreEqual(lists.Count, 1);
            Assert.AreEqual(lists[0].Name, "张三");
        }

        [TestMethod]
        public void SelectAll()
        {
            Init();

            Person p = new Person();
            p.PersonId = Guid.NewGuid();
            p.Name = "张三";
            p.Age = 22;
            p.Address = "上海";
            Assert.AreEqual(DBFactory.Query<Person>().Insert(p), 1);

            Person p1 = new Person();
            p1.PersonId = Guid.NewGuid();
            p1.Name = "李四";
            p1.Age = 21;
            p1.Address = "上海";
            Assert.AreEqual(DBFactory.Query<Person>().Insert(p1), 1);

            Person p2 = new Person();
            p2.PersonId = Guid.NewGuid();
            p2.Name = "李四";
            p2.Age = 18;
            p2.Address = "上海";
            Assert.AreEqual(DBFactory.Query<Person>().Insert(p2), 1);


            IList<Person> lists = DBFactory.Query<Person>().SelectAll("Age DESC");
            Assert.AreEqual(lists.Count, 3);
            Assert.AreEqual(lists[0].Name, "张三");
            Assert.AreEqual(lists[1].Name, "李四");
            Assert.AreEqual(lists[2].Name, "李四");
            Assert.AreEqual(lists[0].Age, 22);
            Assert.AreEqual(lists[1].Age, 21);
            Assert.AreEqual(lists[2].Age, 18);


            lists = DBFactory.Query<Person>().SelectAll("Age ASC");
            Assert.AreEqual(lists.Count, 3);
            Assert.AreEqual(lists[0].Name, "李四");
            Assert.AreEqual(lists[1].Name, "李四");
            Assert.AreEqual(lists[2].Name, "张三");
            Assert.AreEqual(lists[0].Age, 18);
            Assert.AreEqual(lists[1].Age, 21);
            Assert.AreEqual(lists[2].Age, 22);

            lists = DBFactory.Query<Person>().SelectAll(0, 1, "Age ASC");
            Assert.AreEqual(lists.Count, 1);
            Assert.AreEqual(lists[0].Name, "李四");
            Assert.AreEqual(lists[0].Age, 18);
        }

        [TestMethod]
        public void Get()
        {
            Init();

            Guid id = Guid.NewGuid();
            Person p = new Person();
            p.PersonId = id;
            p.Name = "李四";
            p.Age = 21;
            p.Address = "上海";
            Assert.AreEqual(DBFactory.Query<Person>().Insert(p), 1);

            Person p1 = DBFactory.Query<Person>().Get(p.PersonId);
            Assert.AreEqual(p.PersonId, p1.PersonId);
            Assert.AreEqual(p.Name, p1.Name);
            Assert.AreEqual(p.Age, p1.Age);
            Assert.AreEqual(p.Address, p1.Address);
        }

        [TestMethod]
        public void Count()
        {
            Init();

            Person p = new Person();
            p.PersonId = Guid.NewGuid();
            p.Name = "张三";
            p.Age = 21;
            p.Address = "上海";
            Assert.AreEqual(DBFactory.Query<Person>().Insert(p), 1);

            Person p1 = new Person();
            p1.PersonId = Guid.NewGuid();
            p1.Name = "李四";
            p1.Age = 21;
            p1.Address = "上海";
            Assert.AreEqual(DBFactory.Query<Person>().Insert(p1), 1);

            Person p2 = new Person();
            p2.PersonId = Guid.NewGuid();
            p2.Name = "李四";
            p2.Age = 18;
            p2.Address = "上海";
            Assert.AreEqual(DBFactory.Query<Person>().Insert(p2), 1);

            Assert.AreEqual(DBFactory.Query<Person>().Count("1=1"), 3);
            Assert.AreEqual(DBFactory.Query<Person>().Count("Age=21"), 2);
            Assert.AreEqual(DBFactory.Query<Person>().Count("Name='李四'"), 2);
        }

        [TestMethod]
        public void CountAll()
        {

            Init();

            Person p = new Person();
            p.PersonId = Guid.NewGuid();
            p.Name = "张三";
            p.Age = 21;
            p.Address = "上海";
            Assert.AreEqual(DBFactory.Query<Person>().Insert(p), 1);

            Person p1 = new Person();
            p1.PersonId = Guid.NewGuid();
            p1.Name = "李四";
            p1.Age = 21;
            p1.Address = "上海";
            Assert.AreEqual(DBFactory.Query<Person>().Insert(p1), 1);

            Person p2 = new Person();
            p2.PersonId = Guid.NewGuid();
            p2.Name = "李四";
            p2.Age = 18;
            p2.Address = "上海";
            Assert.AreEqual(DBFactory.Query<Person>().Insert(p2), 1);
            Assert.AreEqual(DBFactory.Query<Person>().CountAll(), 3);
        }
    }
}
