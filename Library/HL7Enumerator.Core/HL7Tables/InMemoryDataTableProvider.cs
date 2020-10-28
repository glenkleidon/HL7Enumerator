using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace HL7Enumerator.HL7Tables
{
    public class InMemoryDataTableProvider : IDataTableProvider
    {
        private ConcurrentDictionary<string, Dictionary<string, string>> KnownTables =
              new ConcurrentDictionary<string, Dictionary<string, string>>();

        public Dictionary<string, string> GetCodeTable(string tableId)
        {
            return (!String.IsNullOrWhiteSpace(tableId) && KnownTables.ContainsKey(tableId)) ? KnownTables[tableId] : null;
        }

        public void AddCodeTable(string tableId, Dictionary<string, string> table)
        {
            if (!KnownTables.ContainsKey(tableId)) KnownTables.TryAdd(tableId, table);
        }
        public void Clear(string tableId=null)
        {
            if (tableId == null) KnownTables.Clear();
            else
            {
                Dictionary<string, string> tbl;
                KnownTables.TryRemove(tableId, out tbl);
            }
        }

        public string GetTableValue(string tableId, string key)
        {
            var tbl = GetCodeTable(tableId);
            return (tbl?.ContainsKey(key)==true) ? tbl[key] : "";
           
        }
    }
}
