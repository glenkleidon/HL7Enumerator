using HL7Enumerator.HL7Tables.Interfaces;
using HL7Enumerator.Types.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace HL7Enumerator.Types
{
    public static partial class DataTypes
    {
        public class XON_ExtendedCompositeNameForOrganizations : HL7TypeBase, IHL7Type
        {   
            public string OrganizationName { get; set; }
            public IS_CodedValue OrganizationNameTypeCode { get; set; }
            public NM_Number ID { get; set; }
            public string CheckDigit { get; set; }
            public ID_CodedValue CheckDigitScheme { get; set; }
            public HD_HierarchicDesignator AssigningAuthority { get; set; }
            public IS_CodedValue IdentifierTypeCode { get; set; }
            public HD_HierarchicDesignator AssigningFacility { get; set; }

            public static int TotalCodedFieldCount => 3 + (2 * HD_HierarchicDesignator.TotalCodedFieldCount); // 2 Hds, 2 ISs and 1 Id;

            public int DataTablesRequired => TotalCodedFieldCount;

            public XON_ExtendedCompositeNameForOrganizations()
            {

            }
            public XON_ExtendedCompositeNameForOrganizations(HL7Element element, IEnumerable<string> tableIds = null, IDataTableProvider tables = null)
                 : base(element, tableIds, tables)
            {

            }

            public override void Populate(HL7Element element, IEnumerable<string> tableIds = null)
            {
                var tblsUsed = 0;
                OrganizationName = element.ElementValue(0);
                OrganizationNameTypeCode = NewIS(element.ElementValue(1), NextTableId(tableIds, ref tblsUsed));
                ID = element.IndexedElement(2);
                CheckDigit = element.ElementValue(3);
                CheckDigitScheme = NewID(element.ElementValue(4), NextTableId(tableIds, ref tblsUsed));
                AssigningAuthority = element.AsHD(5, tableIds?.Skip(tblsUsed), Tables);
                tblsUsed += HD_HierarchicDesignator.TotalCodedFieldCount;
                IdentifierTypeCode = NewIS(element.ElementValue(6), NextTableId(tableIds, ref tblsUsed));
                AssigningFacility = element.AsHD(7, tableIds, Tables); 
            }

            public override string ToString()
            {
                return ToString('^');
            }
            public virtual string ToString(char separator)
            {
                return $"{OrganizationName}{separator}{OrganizationNameTypeCode?.BestValue}{separator}{ID}" +
                    $"{separator}{CheckDigit}{separator}{CheckDigitScheme?.BestValue}" +
                    $"{separator}{AssigningAuthority?.ToString()}{separator}{IdentifierTypeCode?.BestValue}" +
                    $"{separator}{AssigningFacility?.ToString()}"
                    .TrimEnd(separator);
            }
        }

    }
}

