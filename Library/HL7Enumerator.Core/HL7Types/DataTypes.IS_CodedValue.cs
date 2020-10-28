using HL7Enumerator.HL7Tables.Interfaces;
using System.Collections.Generic;

namespace HL7Enumerator.Types
{
    public static partial class DataTypes
    {
        public class IS_CodedValue : CodedDataValue
        {

            public IS_CodedValue()
            {
            }
            public IS_CodedValue(string value, string tableId=null, IDataTableProvider tables = null) : base(value, tableId, tables)
            {
            }
            public IS_CodedValue(Dictionary<string, string> table, string tableId = null, IDataTableProvider tables = null) : base(table, tableId, tables)
            {
            }
            public IS_CodedValue(string value, Dictionary<string, string> table, string tableId = null, IDataTableProvider tables = null) : base(value, table, tableId, tables)
            {
            }
            public static implicit operator IS_CodedValue(string value)
            {
                return new IS_CodedValue(value); 
            }


        }
    }
}

