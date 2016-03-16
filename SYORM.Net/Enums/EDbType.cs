using SYORM.Net.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SYORM.Net.Enums
{
    public enum EDbType
    {
        [ConnectionString("Data Source={0};UTF8Encoding=True;BinaryGuid=False;")]
        SQLite,
        [ConnectionString("Data Source={0}")]
        SqlServerCe
    }
}
