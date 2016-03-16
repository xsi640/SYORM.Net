using SYORM.Net.Attribute;
using SYORM.Net.Enums;
using SYORM.Net.Factory;
using SYORM.Net.Ref;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SYORM.Net.Util
{
    internal class DataBaseUtil
    {
        public static void CreateDataBase()
        {
            if (File.Exists(DBFactory.DbFilePath))
                File.Delete(DBFactory.DbFilePath);

            if (DBFactory.DbType == Enums.EDbType.SQLite)
            {
                SqlHelper.ExecuteNonQuery("CREATE TABLE [Demo](id INTEGER NOT NULL)");
                SqlHelper.ExecuteNonQuery("DROP TABLE [Demo]");
            }
            else if (DBFactory.DbType == Enums.EDbType.SqlServerCe)
            {
                Assembly asm = Assembly.GetExecutingAssembly();
                using (Stream stream = asm.GetManifestResourceStream(asm.GetName().Name + ".Files.sqlce.sdf"))
                {
                    using (FileStream fs = new FileStream(DBFactory.DbFilePath, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        int size = 0;
                        byte[] buffer = new byte[4096];
                        while ((size = stream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            fs.Write(buffer, 0, size);
                        }
                        fs.Flush();
                    }
                }

                SqlHelper.ExecuteNonQuery("CREATE TABLE [Demo](id INT NOT NULL)");
                SqlHelper.ExecuteNonQuery("DROP TABLE [Demo]");
            }
        }

        public static void CreateTable(Type type)
        {
            TableRef tableRef = TableRef.Convert(type);
            try
            {
                SqlHelper.ExecuteNonQuery(string.Format("DROP TABLE [{0}]", tableRef.TableName));
            }
            catch { }
            SqlHelper.ExecuteNonQuery(GetCreateTableString(tableRef, DBFactory.DbType));
        }

        private static string GetCreateTableString(TableRef tableRef, EDbType dbType)
        {
            string result = string.Empty;
            if (dbType == EDbType.SqlServerCe || dbType == EDbType.SQLite)
            {
                IList<ColumnRef> columnRefLists = tableRef.ColumnRefArray.ToList();
                if (tableRef.PrimaryKey != null)
                    columnRefLists.Add(tableRef.PrimaryKey);
                StringBuilder colums = new StringBuilder();
                foreach (ColumnRef columnRef in columnRefLists)
                {
                    colums.Append(GetCreateColumnString(columnRef));
                    if (tableRef.PrimaryKey == columnRef)
                        colums.Append(" PRIMARY KEY ");
                    colums.Append(",");
                }
                if (colums[colums.Length - 1] == ',')
                    colums = colums.Remove(colums.Length - 1, 1);
                result = string.Format("CREATE TABLE [{0}] ( {1} )", tableRef.TableName, colums.ToString());
                Debug.WriteLine(result);
            }
            return result;
        }

        /// <summary>
        /// 返回
        ///   [列名] 数据类型 {NOT NULL}
        /// </summary>
        /// <param name="colum"></param>
        /// <returns></returns>
        private static string GetCreateColumnString(ColumnRef colum)
        {
            string result = string.Format("[{0}] ", colum.ColumName);
            result = string.Concat(result, string.Format(GetSqlColumnString(colum.DataType, DBFactory.DbType), colum.Width));
            if (colum.IsNotNull)
                result = string.Concat(result, " NOT NULL");
            return result;
        }

        private static string GetSqlColumnString(EDataType dataType, EDbType dbType)
        {
            string result = string.Empty;
            switch (dataType)
            {
                case EDataType.Int:
                    result = "int";
                    break;
                case EDataType.Varchar:
                    switch (dbType)
                    {
                        case EDbType.SQLite:
                            result = "varchar({0})";
                            break;
                        case EDbType.SqlServerCe:
                            result = "nvarchar({0})";
                            break;
                    }
                    break;
                case EDataType.Char:
                    switch (dbType)
                    {
                        case EDbType.SQLite:
                            result = "char({0})";
                            break;
                        case EDbType.SqlServerCe:
                            result = "nchar({0})";
                            break;
                    }
                    break;
                case EDataType.Guid:
                    switch (dbType)
                    {
                        case EDbType.SQLite:
                            result = "guid";
                            break;
                        case EDbType.SqlServerCe:
                            result = "uniqueidentifier";
                            break;
                    }
                    break;
                case EDataType.Text:
                    switch (dbType)
                    {
                        case EDbType.SQLite:
                            result = "text";
                            break;
                        case EDbType.SqlServerCe:
                            result = "ntext";
                            break;
                    }
                    break;
                case EDataType.DateTime:
                    result = "datetime";
                    break;
                case EDataType.Bigint:
                    result = "bigint";
                    break;
            }
            return result;
        }
    }
}
