using SYORM.Net.Attribute;
using SYORM.Net.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SYORM.Net.Ref
{
    public class TableRef
    {
        public ColumnRef PrimaryKey { get; set; }
        public string TableName { get; set; }
        public Type Type { get; set; }
        public ColumnRef[] ColumnRefArray { get; set; }

        public static TableRef Convert(Type type)
        {
            TableRef tableRef = BasicCache<Type, TableRef>.Instance.Get(type);
            if (tableRef != null)
                return tableRef;

            tableRef = new TableRef();
            tableRef.Type = type;
            TableAttribute tableAttribute = GetTaleAttribute(type);
            if (tableAttribute == null || string.IsNullOrEmpty(tableAttribute.TableName))
                tableRef.TableName = type.Name;
            else
                tableRef.TableName = tableAttribute.TableName;

            IList<ColumnRef> columLists = new List<ColumnRef>();
            PropertyInfo[] propertyInfos = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                if (!propertyInfo.CanWrite ||
                    !propertyInfo.CanRead)
                    continue;

                ColumnAttribute columAttribute = GetColumnAttribute(propertyInfo);
                ColumnRef columRef = new ColumnRef();
                columRef.ColumName = string.IsNullOrEmpty(columAttribute.ColumName) ? propertyInfo.Name : columAttribute.ColumName;
                columRef.DataType = columAttribute.DataType;
                columRef.PrimaryKey = columAttribute.PrimaryKey;
                columRef.IsNotNull = columAttribute.IsNotNull;
                columRef.Width = columAttribute.Width;
                columRef.PropertyInfo = propertyInfo;
                if (columRef.PrimaryKey)
                    tableRef.PrimaryKey = columRef;
                else
                    columLists.Add(columRef);
            }
            tableRef.ColumnRefArray = columLists.ToArray();
            BasicCache<Type, TableRef>.Instance.Set(type, tableRef);
            return tableRef;
        }

        private static TableAttribute GetTaleAttribute(Type type)
        {
            TableAttribute[] tableAttributes = (TableAttribute[])type.GetCustomAttributes(typeof(TableAttribute), false);
            if (tableAttributes != null && tableAttributes.Length > 0)
                return tableAttributes[0];
            return null;
        }

        private static ColumnAttribute GetColumnAttribute(PropertyInfo propertyInfo)
        {
            ColumnAttribute[] columnAttributes = (ColumnAttribute[])propertyInfo.GetCustomAttributes(typeof(ColumnAttribute), false);
            if (columnAttributes != null && columnAttributes.Length > 0)
                return columnAttributes[0];
            return null;
        }
    }
}
