using System.Collections.Generic;

namespace HL7Enumerator.HL7Tables.Interfaces
{
    public interface ITableDetails
    {
        IEnumerable<string> Notes { get; set; }
        string ShortDescription { get; set; }
        string Value { get; set; }
    }
}