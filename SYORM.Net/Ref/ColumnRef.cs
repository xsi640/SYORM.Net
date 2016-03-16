using SYORM.Net.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SYORM.Net.Ref
{
    public class ColumnRef
    {
        public bool PrimaryKey { get; set; }
        public string ColumName { get; set; }
        public EDataType DataType { get; set; }
        public bool IsNotNull { get; set; }
        public int Width { get; set; }
        public PropertyInfo PropertyInfo { get; set; }
    }
}
