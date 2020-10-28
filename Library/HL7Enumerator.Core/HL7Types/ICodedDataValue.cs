using HL7Enumerator.HL7Tables;
using System.Collections.Generic;

namespace HL7Enumerator.Types
{
    public interface ICodedDataValue
    {
        string CodedValue { get; }
        string Description { get; }
        bool? IsValid { get; }
        string Value { get; set; }
        string BestValue { get; }
        string TableId { get; set; }
        Dictionary<string, string> Table { get; set; }
        IDataTableProvider TableProvider { get; set; }
      }
}