using HL7Enumerator.HL7Tables;
using System.Collections.Generic;
using System.Linq;

namespace HL7Enumerator.Types
{
    public static partial class DataTypes
    {
        public class CodedDataValue : ICodedDataValue
        {

            protected string tableId;
            protected Dictionary<string, string> table;
            private IDataTableProvider tableProvider;

            public CodedDataValue()
            {
            }

            public CodedDataValue(string value, string tableId, IDataTableProvider tables=null)
            {
                this.tableProvider = tables;
                this.tableId = tableId;
                this.Value = value;
                this.table = TableProvider.GetCodeTable(tableId);
            }
            public CodedDataValue(string tableId, IDataTableProvider tables = null)
            {
                this.tableProvider = tables;
                this.tableId = tableId;
                this.table = TableProvider.GetCodeTable(tableId);
            }
            public CodedDataValue(Dictionary<string, string> table, string tableId = null, IDataTableProvider tables = null)
            {
                this.tableProvider = tables;
                this.table = table;
                this.tableId = tableId;
                LinkTable();
            }
            public CodedDataValue(string value, Dictionary<string, string> table, string tableId = null, IDataTableProvider tables = null)
            {
                this.tableProvider = tables;
                this.table = table;
                this.tableId = tableId;
                LinkTable();
                Value = value;
            }

            protected void LinkTable()
            {
                if (table == null) this.table = TableProvider.GetCodeTable(tableId);
                if (!string.IsNullOrWhiteSpace(tableId) && table != null)
                    TableProvider.AddCodeTable(tableId, table);
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


            public string TableId { 
                get => tableId; 
                set 
                { 
                    tableId = Value;
                    LinkTable(); 
                } 
            }
            public Dictionary<string, string> Table { 
                get => table; 
                set  
                {
                    table = value;
                    LinkTable();
                } 
            }

            public IDataTableProvider TableProvider {
                get
                {
                    if (tableProvider == null) tableProvider = new InMemoryDataTableProvider();
                    return tableProvider;
                }

                set => tableProvider = value;
                   
            }

            public override string ToString()
            {
                return CodedValue;
            }
        }

    }
}

