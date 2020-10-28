using System;
using System.Collections.Generic;
using System.Text;

namespace HL7Enumerator.HL7Tables
{
    public interface IDataTableProvider
    {
        Dictionary<string, string> GetCodeTable(string tableId);
        void AddCodeTable(string tableId, Dictionary<string, string> table);
        void Clear(string tableId = null);
        string GetTableValue(string tableId, string key);

    }
}
