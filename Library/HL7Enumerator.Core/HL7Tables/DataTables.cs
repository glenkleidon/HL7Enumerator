using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace HL7Enumerator.HL7Tables
{
    public static class DataTables
    {

        public static ConcurrentDictionary<string, Dictionary<string, string>> KnownTables =
              new ConcurrentDictionary<string, Dictionary<string, string>>();

        public static Dictionary<string, string> GetCodeTable(string tableId)
        {
            
            return (tableId!= null && KnownTables.ContainsKey(tableId)) ? KnownTables[tableId] : null;
        }

        public static void AddUpdateCodeTable(string tableId, Dictionary<string, string> table)
        {
            if (!KnownTables.ContainsKey(tableId)) KnownTables.TryAdd(tableId, table);
        }

    }
}
