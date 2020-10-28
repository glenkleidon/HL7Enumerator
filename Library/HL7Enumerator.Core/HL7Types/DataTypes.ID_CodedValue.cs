using HL7Enumerator.HL7Tables;
using System.Collections.Generic;
using System.Linq;

namespace HL7Enumerator.Types
{
    public static partial class DataTypes
    {
        public class ID_CodedValue : CodedDataValue
        {

            public ID_CodedValue()
            {

            }
            public ID_CodedValue(string value, string tableId, IDataTableProvider tables=null) : base(value, tableId, tables)
            { 
            }
            public ID_CodedValue(Dictionary<string, string> table, string tableId = null, IDataTableProvider tables = null) : base(table, tableId, tables)
            {
            }
            public ID_CodedValue(string value, Dictionary<string, string> table, string tableId = null, IDataTableProvider tables = null): base(value, table, tableId, tables)
            {
            }
        }
    }
}

