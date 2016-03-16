using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SYORM.Net.Queryer
{
    /// <summary>
    /// 基础数据库访问接口
    /// </summary>
    /// <typeparam name="TKey">数据库主键</typeparam>
    /// <typeparam name="TObject">数据库对象</typeparam>
    public interface IQueryer<TObject>
    {
        int Insert(TObject obj);
        int Insert(IList<TObject> lists);
        int InsertOrUpdate(TObject obj);
        int InsertOrUpdate(IList<TObject> lists);
        int Update(TObject obj);
        int Delete(object key);
        int DeleteAll();
        int DeleteAll(string where);
        int Delete(IList<object> idLists);
        List<TObject> Select(string where, string order = "");
        List<TObject> Select(string where, int offset, int limit, string order = "");
        List<TObject> SelectAll(string order = "");
        List<TObject> SelectAll(int offset, int limit, string order = "");
        TObject Get(object key);
        int Count(string where);
        int CountAll();
    }
}
