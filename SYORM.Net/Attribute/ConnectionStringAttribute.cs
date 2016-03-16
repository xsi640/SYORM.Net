using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SYORM.Net.Attribute
{
    internal class ConnectionStringAttribute : System.Attribute
    {
        private string _ConnectionString = string.Empty;

        public ConnectionStringAttribute(string connectionString)
        {
            this._ConnectionString = connectionString;
        }

        public string ConnectionString
        {
            get { return this._ConnectionString; }
        }
    }
}
