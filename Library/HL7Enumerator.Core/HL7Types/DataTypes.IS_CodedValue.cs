using HL7Enumerator.HL7Tables;
using System.Collections.Generic;
using System.Linq;

namespace HL7Enumerator.Types
{
    public static partial class DataTypes
    {
        public class IS_CodedValue : CodedDataValue
        {

            public IS_CodedValue()
            {
            }
            public IS_CodedValue(string value, string tableId)
            {
                this.tableId = tableId;
                this.Value = value;
                this.table = DataTables.GetCodeTable(tableId);
            }
            public IS_CodedValue(Dictionary<string, string> table, string tableId = null)
            {
                this.table = table;
                this.tableId = tableId;
                LinkTable();
                if (table == null) this.table = DataTables.GetCodeTable(tableId);
            }
            public IS_CodedValue(string value, Dictionary<string, string> table, string tableId = null)
            {
                this.table = table;
                this.tableId = tableId;
                LinkTable();
                if (table == null) this.table = DataTables.GetCodeTable(tableId);
                Value = value;
            }
        }
    }
}

