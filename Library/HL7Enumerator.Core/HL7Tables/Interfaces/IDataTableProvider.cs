using System.Collections.Generic;

namespace HL7Enumerator.HL7Tables.Interfaces
{
    public interface IDataTableProvider
    {
        Dictionary<string, string> GetCodeTable(string tableId);
        void AddCodeTable(string tableId, Dictionary<string, string> table);
        void Clear(string tableId = null);
        string GetTableValue(string tableId, string key);

    }
}
