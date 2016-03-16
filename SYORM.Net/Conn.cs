using SYORM.Net.Attribute;
using SYORM.Net.Enums;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SYORM.Net
{
    /// <summary>
    /// 数据库链接字符串
    /// </summary>
    public class Conn
    {
        #region 变量
        public bool _IsSingleConnection = true;

        private EDbType _DbType = EDbType.SQLite;
        private string _DbFilePath = string.Empty;
        private DbConnection _Connection = null;
        private object _Locker = new object();
        #endregion

        #region 属性
        public EDbType DbType
        {
            get { return this._DbType; }
            set { this._DbType = value; }
        }

        public string DbFilePath
        {
            get { return this._DbFilePath; }
            set { this._DbFilePath = value; }
        }

        public DbConnection DbConnection
        {
            get { return this.GetDbConnection(); }
        }
        public bool IsSingleConnection
        {
            get { return this._IsSingleConnection; }
            set { this._IsSingleConnection = value; }
        }
        #endregion

        #region 方法
        public void ConnectionClose()
        {
            if (_Connection != null)
                _Connection.Close();
            _Connection = null;
        }
        #endregion

        #region private
        private DbConnection GetDbConnection()
        {
            if (IsSingleConnection)
            {
                if (this._Connection == null)
                {
                    lock (_Locker)
                    {
                        if (this._Connection == null)
                        {
                            this._Connection = CreateConnection();
                            this._Connection.Open();
                        }
                    }
                }
                else
                {
                    if (this._Connection.State != System.Data.ConnectionState.Open)
                    {
                        lock (_Locker)
                        {
                            if (this._Connection.State != System.Data.ConnectionState.Open)
                                this._Connection.Open();
                        }
                    }
                }
                return this._Connection;
            }
            else
            {
                DbConnection connection = CreateConnection();
                connection.Open();
                return connection;
            }
        }

        private DbConnection CreateConnection()
        {
            DbConnection connection = null;
            CheckDbFilePath();

            string connectionString = string.Format(this.GetConnectionString(), this._DbFilePath);
            switch (this._DbType)
            {
                case EDbType.SQLite:
                    connection = new System.Data.SQLite.SQLiteConnection(connectionString);
                    break;
                case EDbType.SqlServerCe:
                    connection = new System.Data.SqlServerCe.SqlCeConnection(connectionString);
                    break;
            }

            return connection;
        }

        private void CheckDbFilePath()
        {
            FileInfo fileInfo = new FileInfo(this._DbFilePath);
            if (!fileInfo.Directory.Exists)
                fileInfo.Directory.Create();
        }

        private string GetConnectionString()
        {
            FieldInfo fieldInfo = this._DbType.GetType().GetField(this._DbType.ToString());
            ConnectionStringAttribute[] connectionStringAttributes = (ConnectionStringAttribute[])fieldInfo.GetCustomAttributes(typeof(ConnectionStringAttribute), true);
            if (connectionStringAttributes != null && connectionStringAttributes.Length > 0)
                return connectionStringAttributes[0].ConnectionString;
            return string.Empty;
        }
        #endregion
    }
}
