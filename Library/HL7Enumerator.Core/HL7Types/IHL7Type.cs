using System.Collections.Generic;

namespace HL7Enumerator.Types
{
    public interface IHL7Type
    {
        int TablesUsed { get; }

        void Populate(HL7Element element, IEnumerable<string> tableIds = null);
        string ToString(char sepatator);
    }
}