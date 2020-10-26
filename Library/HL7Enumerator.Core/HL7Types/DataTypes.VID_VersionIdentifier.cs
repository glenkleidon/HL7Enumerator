using System.Collections.Generic;
using System.Linq;

namespace HL7Enumerator.Types
{
    public static partial class DataTypes
    {
        public class VID_VersionIdentifier: IHL7Type
        { 
           public ID_CodedValue VersionID { get; set; }
           public CE_CodedElement Internationalization { get; set; }
           public CE_CodedElement InternationalVersionID { get; set; }

            public int TablesUsed => 1 + (2 * new CE_CodedElement().TablesUsed); // 2 CEs and 1 ID

            public VID_VersionIdentifier()
            {

            }
            public VID_VersionIdentifier(HL7Element element, IEnumerable<string> tableIds = null)
            {
                Populate(element, tableIds);
            }
            public void Populate(HL7Element element, IEnumerable<string> tableIds = null)
            {
                var tblsUsed = 0;
                VersionID = new ID_CodedValue(element.ElementValue(0), NextTableId(tableIds,ref tblsUsed));
                Internationalization = new CE_CodedElement(element.IndexedElement(1), tableIds?.Skip(tblsUsed));
                tblsUsed += Internationalization.TablesUsed;
                InternationalVersionID = new CE_CodedElement(element.IndexedElement(2), tableIds?.Skip(tblsUsed));
            }

            public override string ToString()
            {
                return ToString('^');
            }
            public virtual string ToString(char separator)
            {
                return
                    $"{VersionID}{separator}{Internationalization.ToString()}{separator}{InternationalVersionID.ToString()}";
            }

        }

    }
}

