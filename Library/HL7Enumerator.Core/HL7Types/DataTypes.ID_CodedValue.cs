using HL7Enumerator.HL7Tables.Interfaces;
using System.Collections.Generic;

namespace HL7Enumerator.Types
{
    public static partial class DataTypes
    {
        public class ID_CodedValue : CodedDataValue
        {

            public ID_CodedValue()
            {

            }
            public ID_CodedValue(string value, string tableId=null, IDataTableProvider tables=null) : base(value, tableId, tables)
            { 
            }
            public ID_CodedValue(Dictionary<string, string> table, string tableId = null, IDataTableProvider tables = null) : base(table, tableId, tables)
            {
            }
            public ID_CodedValue(string value, Dictionary<string, string> table, string tableId = null, IDataTableProvider tables = null): base(value, table, tableId, tables)
            {
            }
            public static implicit operator ID_CodedValue(string value)
            {
                return new ID_CodedValue(value);
            }

        }
    }
}

