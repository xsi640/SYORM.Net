using SYORM.Net.Attribute;
using SYORM.Net.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SYORM.Net.UnitTest
{
    [Table("PersonInfo")]
    public class Person
    {
        [Column(true, EDataType.Guid)]
        public Guid PersonId { get; set; }
        [Column(EDataType.Varchar, 50)]
        public string Name { get; set; }
        [Column(EDataType.Int)]
        public int Age { get; set; }
        [Column(EDataType.Varchar, 200)]
        public string Address { get; set; }
    }
}
