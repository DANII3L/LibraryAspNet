using System;
using System.Data;

namespace LibraryBaseData
{
    public class ClassParameters
    {
        public ClassParameters()
        {
            this.DbType = SqlDbType.VarChar;
            this.TypeCode = TypeCode.String;
        }
        public string NameParameter { get; set; }
        public SqlDbType DbType { get; set; }
        public object Value { get; set; }
        public TypeCode TypeCode { get; set; }
    }
}
