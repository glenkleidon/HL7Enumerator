using HL7Enumerator.HL7Tables.Interfaces;
using HL7Enumerator.Types.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace HL7Enumerator.Types
{
    public static partial class DataTypes
    {
        public class VID_VersionIdentifier : HL7TypeBase, IHL7Type
        {
            public ID_CodedValue VersionID { get; set; }
            public CE_CodedElement Internationalization { get; set; }
            public CE_CodedElement InternationalVersionID { get; set; }

            public static int TablesRequired => 1 + (2 * CE_CodedElement.TablesRequired); // 2 CEs and 1 ID

            public int DataTablesRequired => TablesRequired;

            public VID_VersionIdentifier()
            {
            }
            public VID_VersionIdentifier(HL7Element element, IEnumerable<string> tableIds = null, IDataTableProvider tables = null)
                : base(element, tableIds, tables)
            {
            }

            public override void Populate(HL7Element element, IEnumerable<string> tableIds = null)
            {
                var tblsUsed = 0;
                VersionID = NewID(element.ElementValue(0), NextTableId(tableIds, ref tblsUsed));
                Internationalization = element.IndexedElement(1).AsCE(tableIds?.Skip(tblsUsed), Tables);
                tblsUsed += CE_CodedElement.TablesRequired;
                InternationalVersionID = element.IndexedElement(2).AsCE(tableIds?.Skip(tblsUsed), Tables);
            }

            public override string ToString()
            {
                return ToString('^');
            }
            public virtual string ToString(char separator)
            {
                return
                    $"{VersionID}{separator}{Internationalization?.ToString()}{separator}{InternationalVersionID?.ToString()}"
                    .TrimEnd(separator);
            }

        }

    }
}

