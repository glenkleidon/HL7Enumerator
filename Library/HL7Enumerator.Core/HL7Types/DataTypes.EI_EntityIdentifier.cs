using HL7Enumerator.HL7Tables.Interfaces;
using HL7Enumerator.Types.Interfaces;
using System;
using System.Collections.Generic;

namespace HL7Enumerator.Types
{
    public static partial class DataTypes
    {
        public class EI_EntityIdentifier: HL7TypeBase, IHL7Type
        {
            public string Identifier { get; set; }
            public IS_CodedValue NamespaceId { get; set; }
            public string UniversalId { get; set; }
            public ID_CodedValue UniversalIdType { get; set; }

            public static int TotalCodedFieldCount => 2; // 1 IS and 1 ID

            public int DataTablesRequired => TotalCodedFieldCount;

            public override string ToString()
            {
                return ToString('^');
            }
            public EI_EntityIdentifier()
            {

            }
            public EI_EntityIdentifier(HL7Element element, IEnumerable<string> tableIds=null, IDataTableProvider tables = null)
                :base(element, tableIds, tables)
            {
            }
            public string ToString(char separator)
            {
                return $"{Identifier}{separator}{NamespaceId?.BestValue}{separator}{UniversalId}{separator}{UniversalIdType?.BestValue}"
                    .TrimEnd(separator);
               
            }

            public override void Populate(HL7Element element, IEnumerable<string> tableIds = null)
            {
                var tblsUsed = 0;
                Identifier = element.ElementValue(0);
                NamespaceId = NewIS(element.ElementValue(1), NextTableId(tableIds, ref tblsUsed));
                UniversalId = element.ElementValue(2);
                UniversalIdType = NewID(element.ElementValue(3),NextTableId(tableIds, ref tblsUsed));
            }
        }

    }
}

