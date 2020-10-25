using HL7Enumerator.Core.HL7Tables;
using System.Collections.Generic;
using System.Linq;

namespace HL7Enumerator.Types
{
    public static partial class DataTypes
    {
        public class CodedDataValue : ICodedDataValue
        {
            private readonly string tableId;
            private readonly Dictionary<string, string> table;
            public CodedDataValue()
            {
            }
            public CodedDataValue(string tableId)
            {
                this.tableId = tableId;
                this.table = DataTables.GetCodeTable(tableId);
            }
            public CodedDataValue(Dictionary<string, string> table, string tableId = null)
            {
                this.table = table;
                this.tableId = tableId;
                LinkTable();
                if (table == null) this.table = DataTables.GetCodeTable(tableId);
            }
            public CodedDataValue(string value, Dictionary<string, string> table, string tableId = null)
            {
                this.table = table;
                this.tableId = tableId;
                LinkTable();
                if (table == null) this.table = DataTables.GetCodeTable(tableId);
                Value = value;
            }

            protected void LinkTable()
            {
                if (!string.IsNullOrWhiteSpace(tableId) && table != null)
                    DataTables.AddUpdateCodeTable(tableId, table);
            }
            public string Value { get; set; }
            public string CodedValue
            {
                get
                {
                    return (IsValid.Value) ? Value : string.Empty;
                }
            }
            public string Description
            {
                get
                {
                    return (CodedValue.Length > 0) ? table[CodedValue] : string.Empty;
                }
            }
            public bool? IsValid
            {
                get => (table!=null && table.ContainsKey(Value));
            }
            public string BestValue 
            {
                get 
                {
                    return (CodedValue.Length>0) ? CodedValue : Value;
                }
            }

            public string TableId => tableId;

            public override string ToString()
            {
                return CodedValue;
            }
        }

    }
}

