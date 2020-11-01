using HL7Enumerator.HL7Tables.Interfaces;
using HL7Enumerator.Types.Interfaces;
using System.Collections.Generic;

namespace HL7Enumerator.Types
{
    public static partial class DataTypes
    {
        public class HD_HierarchicDesignator : HL7TypeBase, IHL7Type
        {
            public HD_HierarchicDesignator()
            {

            }
            public HD_HierarchicDesignator(HL7Element element, IEnumerable<string> tableIds = null, IDataTableProvider tables = null)
               : base(element, tableIds, tables)
            {
            }
            public override void Populate(HL7Element element, IEnumerable<string> tableIds = null)
            {
                var tblsUsed = 0;
                NamespaceId = NewIS(element.ElementValue(0),NextTableId(tableIds, ref tblsUsed));
                UniversalId = element.ElementValue(1);
                UniversalIdType = NewID(element.ElementValue(2),NextTableId(tableIds, ref tblsUsed));
            }

            public IS_CodedValue NamespaceId { get; set; }
            public string UniversalId { get; set; }
            public ID_CodedValue UniversalIdType { get; set; }

            public static int TotalCodedFieldCount => 2; // 2 ISs used.
            public int DataTablesRequired => TotalCodedFieldCount;

            public override string ToString()
            {
                return ToString('^');
            }
            public string ToString(char separator)
            {
                return $"{NamespaceId?.BestValue}{separator}{UniversalId}{separator}{UniversalIdType?.BestValue}"
                    .TrimEnd(separator);
                
            }
        }

    }
}

