using SYORM.Net.Factory;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace SYORM.Net.Queryer
{
    public class BasicQueryer<TObject> : BaseQueryer<TObject>, IQueryer<TObject>
    {
        public int Insert(TObject obj)
        {
            int result = 0;
            if (obj == null)
                return result;

            DbParameter[] dbParameters = base.GetDbParameters(obj, base.TableRef.PrimaryKey != null);

            try
            {
                result = SqlHelper.ExecuteNonQuery(base.InsertSql, dbParameters);
            }
            catch (DbException ex)
            {
                throw ex;
            }

            return result;
        }

        public int Insert(IList<TObject> lists)
        {
            int result = 0;
            if (lists == null || lists.Count == 0)
                return result;

            DbTransaction tran = null;
            try
            {
                tran = DBFactory.Connection.DbConnection.BeginTransaction();
                foreach (TObject obj in lists)
                {
                    DbParameter[] dbParameters = base.GetDbParameters(obj, true);
                    result += SqlHelper.ExecuteNonQuery(tran, base.InsertSql, dbParameters);
                }

                tran.Commit();
            }
            catch (DbException ex)
            {
                if (tran != null)
                    tran.Rollback();
                result = 0;
                throw ex;
            }

            return result;
        }

        public int InsertOrUpdate(TObject obj)
        {
            if (base.TableRef.PrimaryKey == null)
                throw new NotSupportedException("No PrimaryKey.");

            int result = 0;
            if (this.Count(string.Format("[{0}]='{1}'", base.TableRef.PrimaryKey.ColumName, base.GetValue(obj, base.TableRef.PrimaryKey))) == 0)
            {
                result = Insert(obj);
            }
            else
            {
                result = Update(obj);
            }
            return result;
        }

        public int InsertOrUpdate(IList<TObject> lists)
        {
            if (base.TableRef.PrimaryKey == null)
                throw new NotSupportedException("No PrimaryKey.");

            int result = 0;
            if (lists == null || lists.Count == 0)
                return result;

            DbTransaction tran = null;
            try
            {
                tran = DBFactory.Connection.DbConnection.BeginTransaction();
                foreach (TObject obj in lists)
                {
                    if (this.Count(string.Format("[{0}]='{1}'", base.TableRef.PrimaryKey.ColumName, base.GetValue(obj, base.TableRef.PrimaryKey))) == 0)
                    {
                        DbParameter[] dbParameters = base.GetDbParameters(obj, true);
                        result += SqlHelper.ExecuteNonQuery(tran, base.InsertSql, dbParameters);
                    }
                    else
                    {
                        DbParameter[] dbParameters = base.GetDbParameters(obj, true);
                        result += SqlHelper.ExecuteNonQuery(tran, base.UpdateSql, dbParameters);
                    }
                }
                tran.Commit();
            }
            catch (DbException ex)
            {
                if (tran != null)
                    tran.Rollback();
                result = 0;
                throw ex;
            }
            return result;
        }

        public int Update(TObject obj)
        {
            if (base.TableRef.PrimaryKey == null)
                throw new NotSupportedException("No PrimaryKey.");

            int result = 0;
            if (obj == null)
                return result;

            DbParameter[] dbParameters = base.GetDbParameters(obj, true);
            try
            {
                result = SqlHelper.ExecuteNonQuery(base.UpdateSql, dbParameters);
            }
            catch (DbException ex)
            {
                throw ex;
            }

            return result;
        }

        public int Delete(object key)
        {
            if (base.TableRef.PrimaryKey == null)
                throw new NotSupportedException("No PrimaryKey.");

            int result = 0;

            DbParameter paramId = base.GetDbParameterByValue(key, base.TableRef.PrimaryKey);

            try
            {
                result = SqlHelper.ExecuteNonQuery(base.DeleteSql, paramId);
            }
            catch (DbException ex)
            {
                throw ex;
            }

            return result;
        }

        public int DeleteAll()
        {
            int result = 0;

            try
            {
                result = SqlHelper.ExecuteNonQuery(base.DeleteAllSql);
            }
            catch (DbException ex)
            {
                throw ex;
            }
            return result;
        }

        public int DeleteAll(string where)
        {
            int result = 0;
            if (string.IsNullOrEmpty(where))
            {
                return result;
            }
            string sql = string.Format(base.DeleteWhereSql, where);
            try
            {
                result = SqlHelper.ExecuteNonQuery(sql);
            }
            catch (DbException ex)
            {
                throw ex;
            }
            return result;
        }

        public int Delete(IList<object> idLists)
        {
            if (base.TableRef.PrimaryKey == null)
                throw new NotSupportedException("No PrimaryKey.");

            int result = 0;
            if (idLists == null || idLists.Count == 0)
                return result;

            DbTransaction tran = null;

            try
            {
                tran = DBFactory.Connection.DbConnection.BeginTransaction();
                foreach (object id in idLists)
                {
                    DbParameter paramId = base.GetDbParameterByValue(id, base.TableRef.PrimaryKey);
                    result += SqlHelper.ExecuteNonQuery(tran, base.DeleteSql, paramId);
                }

                tran.Commit();
            }
            catch (DbException ex)
            {
                if (tran != null)
                    tran.Rollback();
                result = 0;
                throw ex;
            }

            return result;
        }

        public List<TObject> Select(string where, string order = "")
        {
            List<TObject> result = new List<TObject>();
            if (string.IsNullOrEmpty(where))
            {
                where = "1=1";
            }
            if (string.IsNullOrEmpty(order))
            {
                order = string.Format("[{0}] DESC", base.TableRef.PrimaryKey.ColumName);
            }

            string sql = string.Format(base.SelectWhereSql, where, order);

            try
            {
                result = SqlHelper.ExecuteList<TObject>(sql);
            }
            catch (DbException ex)
            {
                throw ex;
            }
            return result;
        }

        public List<TObject> Select(string where, int offset, int limit, string order = "")
        {
            List<TObject> result = new List<TObject>();
            if (string.IsNullOrEmpty(where))
            {
                where = "1=1";
            }
            if (string.IsNullOrEmpty(order))
            {
                order = string.Format("[{0}] DESC", base.TableRef.PrimaryKey.ColumName);
            }

            string sql = string.Format(base.SelectWhereSql, where, order) + string.Format(base.LimitSql, offset, limit);

            try
            {
                result = SqlHelper.ExecuteList<TObject>(sql);
            }
            catch (DbException ex)
            {
                throw ex;
            }
            return result;
        }

        public List<TObject> SelectAll(string order = "")
        {
            List<TObject> result = new List<TObject>();
            if (string.IsNullOrEmpty(order))
            {
                order = string.Format("[{0}] DESC", base.TableRef.PrimaryKey.ColumName);
            }

            string sql = string.Format(base.SelectAllSql, order);

            try
            {
                result = SqlHelper.ExecuteList<TObject>(sql);
            }
            catch (DbException ex)
            {
                throw ex;
            }
            return result;
        }

        public List<TObject> SelectAll(int offset, int limit, string order = "")
        {
            List<TObject> result = new List<TObject>();
            if (string.IsNullOrEmpty(order))
            {
                order = string.Format("[{0}] DESC", base.TableRef.PrimaryKey.ColumName);
            }
            string sql = string.Format(base.SelectAllSql, order) + string.Format(base.LimitSql, offset, limit);

            try
            {
                result = SqlHelper.ExecuteList<TObject>(sql);
            }
            catch (DbException ex)
            {
                throw ex;
            }
            return result;
        }

        public TObject Get(object key)
        {
            if (base.TableRef.PrimaryKey == null)
                throw new NotSupportedException("No PrimaryKey.");

            TObject result = default(TObject);

            DbParameter paramId = base.GetDbParameterByValue(key, base.TableRef.PrimaryKey);

            try
            {
                result = SqlHelper.ExecuteEntity<TObject>(base.GetSql, paramId);
            }
            catch (DbException ex)
            {
                throw ex;
            }
            return result;
        }

        public int Count(string where)
        {
            int result = 0;
            if (string.IsNullOrEmpty(where))
            {
                where = "1=1";
            }

            string sql = string.Format(base.CountWhereSql, where);

            try
            {
                result = SqlHelper.ExecuteScalar(sql);
            }
            catch (DbException ex)
            {
                throw ex;
            }
            return result;
        }

        public int CountAll()
        {
            int result = 0;
            try
            {
                result = SqlHelper.ExecuteScalar(base.CountAllSql);
            }
            catch (DbException ex)
            {
                throw ex;
            }
            return result;
        }
    }
}
