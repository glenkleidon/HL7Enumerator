using HL7Enumerator.HL7Tables.Interfaces;
using System.Collections.Generic;

namespace HL7Enumerator.Types.Interfaces
{
    public interface IHL7Type
    {
        int DataTablesRequired { get; }

        void Populate(HL7Element element, IEnumerable<string> tableIds = null);
        string ToString(char sepatator);

        IDataTableProvider Tables { get; set; }
    }
}