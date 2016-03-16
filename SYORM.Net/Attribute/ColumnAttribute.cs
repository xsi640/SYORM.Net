using SYORM.Net.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SYORM.Net.Attribute
{
    public class ColumnAttribute : System.Attribute
    {
        #region 变量
        private string _ColumName = string.Empty;
        private bool _PrimaryKey = false;
        private EDataType _DateType;
        private int _Width = 50;
        private bool _IsNotNull = false;
        #endregion

        #region 构造函数
        public ColumnAttribute(string columName, bool primaryKey, EDataType dataType, int width, bool isNotNull)
        {
            this._ColumName = columName;
            this._PrimaryKey = primaryKey;
            this._DateType = dataType;
            this._Width = width;
            this._IsNotNull = isNotNull;
        }

        public ColumnAttribute(EDataType dataType, int width)
            : this(string.Empty, false, dataType, width, false)
        { }

        public ColumnAttribute(EDataType dataType)
            : this(string.Empty, false, dataType, 50, false)
        { }

        public ColumnAttribute(bool primaryKey, EDataType dataType)
            : this(string.Empty, primaryKey, dataType, 0, false)
        { }
        #endregion

        #region 属性
        public bool PrimaryKey
        {
            get { return this._PrimaryKey; }
        }
        public string ColumName
        {
            get { return this._ColumName; }
        }
        public EDataType DataType
        {
            get { return this._DateType; }
        }
        public int Width
        {
            get { return this._Width; }
        }
        public bool IsNotNull
        {
            get { return this._IsNotNull; }
        }
        #endregion

        #region 方法
        internal void Check()
        {
            if ((this._DateType == EDataType.Char ||
                this._DateType == EDataType.Varchar) && this._Width <= 0)
            {
                throw new ArgumentException(string.Format("Column Setting error data:{0} width:{1}", this._DateType, this._Width));
            }
        }
        #endregion
    }
}
