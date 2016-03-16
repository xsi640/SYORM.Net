using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SYORM.Net.Attribute
{
    public class TableAttribute : System.Attribute
    {
        private string _TableName;

        public TableAttribute(string tableName)
        {
            this._TableName = tableName;
        }

        public string TableName
        {
            get { return this._TableName; }
        }
    }
}
