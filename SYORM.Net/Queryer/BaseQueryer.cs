using SYORM.Net.Enums;
using SYORM.Net.Factory;
using SYORM.Net.Ref;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace SYORM.Net.Queryer
{
    public abstract class BaseQueryer<T>
    {
        private string _InsertSql;
        private string _UpdateSql;
        private string _DeleteSql;
        private string _DeleteAllSql;
        private string _DeleteWhereSql;
        private string _SelectWhereSql;
        private string _SelectAllSql;
        private string _GetSql;
        private string _CountWhereSql;
        private string _CountAllSql;
        private string _LimitSql;

        private TableRef _TableRef = null;

        protected string InsertSql
        {
            get { return this._InsertSql; }
        }
        protected string UpdateSql
        {
            get { return this._UpdateSql; }
        }
        protected string DeleteSql
        {
            get { return this._DeleteSql; }
        }
        protected string DeleteAllSql
        {
            get { return this._DeleteAllSql; }
        }
        protected string DeleteWhereSql
        {
            get { return this._DeleteWhereSql; }
        }
        protected string SelectWhereSql
        {
            get { return this._SelectWhereSql; }
        }
        protected string SelectAllSql
        {
            get { return this._SelectAllSql; }
        }
        protected string GetSql
        {
            get { return this._GetSql; }
        }
        protected string CountWhereSql
        {
            get { return this._CountWhereSql; }
        }
        protected string CountAllSql
        {
            get { return this._CountAllSql; }
        }
        protected string LimitSql
        {
            get { return this._LimitSql; }
        }
        protected TableRef TableRef
        {
            get { return this._TableRef; }
        }

        public void Initialize()
        {
            Type type = typeof(T);

            this._TableRef = TableRef.Convert(type);
            IList<ColumnRef> lists = new List<ColumnRef>();
            this._InsertSql = string.Format("INSERT INTO [{0}]({1}) VALUES({2})",
                                                this._TableRef.TableName,
                                                this.GetColumnString("[{0}]", this.GetColumnRef(this._TableRef.ColumnRefArray, this._TableRef.PrimaryKey)),
                                                this.GetColumnString("@{0}", this.GetColumnRef(this._TableRef.ColumnRefArray, this._TableRef.PrimaryKey)));
            Debug.WriteLine(this._InsertSql);
            this._UpdateSql = string.Format("UPDATE [{0}] SET {1} WHERE [{2}]=@{2}",
                                                this._TableRef.TableName,
                                                this.GetColumnString("[{0}]=@{0}", this._TableRef.ColumnRefArray),
                                                this._TableRef.PrimaryKey.ColumName);
            Debug.WriteLine(this._UpdateSql);
            this._DeleteSql = string.Format("DELETE FROM [{0}] WHERE [{1}]=@{1}", this._TableRef.TableName, this._TableRef.PrimaryKey.ColumName);
            Debug.WriteLine(this._DeleteSql);
            this._DeleteAllSql = string.Format("DELETE FROM [{0}]", this._TableRef.TableName);
            Debug.WriteLine(this._DeleteAllSql);
            this._DeleteWhereSql = this._DeleteAllSql + " WHERE {0}";
            Debug.WriteLine(this._DeleteWhereSql);
            string selectSql = string.Format("SELECT {0} FROM [{1}]", this.GetColumnString("[{0}]", this.GetColumnRef(this._TableRef.ColumnRefArray, this._TableRef.PrimaryKey)), this._TableRef.TableName);
            this._SelectWhereSql = selectSql + " WHERE {0} ORDER BY {1}";
            Debug.WriteLine(this._SelectWhereSql);
            this._SelectAllSql = selectSql + " ORDER BY {0}";
            this._GetSql = string.Format("SELECT {0} FROM [{1}] WHERE [{2}]=@{2}", this.GetColumnString("[{0}]", this.GetColumnRef(this._TableRef.ColumnRefArray, this._TableRef.PrimaryKey)), this._TableRef.TableName, this._TableRef.PrimaryKey.ColumName);
            Debug.WriteLine(this._GetSql);
            this._CountAllSql = string.Format("SELECT COUNT(-1) FROM [{0}]", this._TableRef.TableName);
            Debug.WriteLine(this._CountAllSql);
            this._CountWhereSql = this._CountAllSql + " WHERE {0}";
            Debug.WriteLine(this._CountWhereSql);
            if (DBFactory.DbType == EDbType.SQLite)
                this._LimitSql = " LIMIT {0},{1}";
            else if (DBFactory.DbType == EDbType.SqlServerCe)
                this._LimitSql = " OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY;";

            Debug.WriteLine(this._LimitSql);
        }
        private string GetColumnString(string expression, params ColumnRef[] columnRefArray)
        {
            string result = string.Empty;
            for (int i = 0; i < columnRefArray.Length; i++)
            {
                ColumnRef column = columnRefArray[i];
                if (i == columnRefArray.Length - 1)
                {
                    result = string.Concat(result, string.Format(expression, column.ColumName));
                }
                else
                {
                    result = string.Concat(result, string.Format(expression, column.ColumName), ",");
                }
            }
            return result;
        }
        private ColumnRef[] GetColumnRef(ColumnRef[] columnRefArray, ColumnRef primaryKey)
        {
            if (primaryKey == null)
            {
                return columnRefArray;
            }
            else
            {
                ColumnRef[] result = new ColumnRef[columnRefArray.Length + 1];
                result[0] = primaryKey;
                for (int i = 0; i < columnRefArray.Length; i++)
                    result[i + 1] = columnRefArray[i];
                return result;
            }
        }
        protected DbParameter[] GetDbParameters(T obj, bool isPrimaryKey)
        {
            DbParameter[] result = null;
            if (isPrimaryKey && this._TableRef.PrimaryKey != null)
            {
                result = new DbParameter[this._TableRef.ColumnRefArray.Length + 1];
                result[0] = this.GetDbParameter(obj, this._TableRef.PrimaryKey);
                for (int i = 0; i < this._TableRef.ColumnRefArray.Length; i++)
                    result[i + 1] = this.GetDbParameter(obj, this._TableRef.ColumnRefArray[i]);
            }
            else
            {
                result = new DbParameter[this._TableRef.ColumnRefArray.Length];
                for (int i = 0; i < this._TableRef.ColumnRefArray.Length; i++)
                    result[i] = this.GetDbParameter(obj, this._TableRef.ColumnRefArray[i]);
            }
            return result;
        }
        internal DbParameter GetDbParameter(T obj, ColumnRef column)
        {
            DbParameter result = DBFactory.CreateParameter();
            result.ParameterName = "@" + column.ColumName;
            if (column.Width > 0)
                result.Size = column.Width;
            result.Value = column.PropertyInfo.GetValue(obj, null);
            return result;
        }

        internal DbParameter GetDbParameterByValue(Object value, ColumnRef column)
        {
            DbParameter result = DBFactory.CreateParameter();
            result.ParameterName = "@" + column.ColumName;
            if (column.Width > 0)
                result.Size = column.Width;
            result.Value = value;
            return result;
        }

        internal object GetValue(T obj, ColumnRef colum)
        {
            return colum.PropertyInfo.GetValue(obj, null);
        }
    }
}
