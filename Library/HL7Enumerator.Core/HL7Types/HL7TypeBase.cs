using HL7Enumerator.HL7Tables.Interfaces;
using System;
using System.Collections.Generic;
using static HL7Enumerator.Types.DataTypes;

namespace HL7Enumerator.Types
{
    public class HL7TypeBase
    {
        public HL7TypeBase()
        {

        }

        public HL7TypeBase(HL7Element element, IEnumerable<string> tableIds = null, IDataTableProvider tables=null)
        {
            Tables = tables;
            Populate(element, tableIds);
        }
        public virtual void Populate(HL7Element element, IEnumerable<string> tableIds = null)
        {
            throw new NotImplementedException("Descendant class must override the 'Populate' method ");
        }
        public IDataTableProvider Tables { get; set; }

        protected ID_CodedValue NewID(string value, string tableId)
        {
            return new ID_CodedValue(value, tableId, Tables);
        }
        protected IS_CodedValue NewIS(string value, string tableId)
        {
            return new IS_CodedValue(value, tableId, Tables);
        }

    }
}
