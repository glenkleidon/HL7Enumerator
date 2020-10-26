using System.Collections.Generic;
using System.Linq;

namespace HL7Enumerator.Types
{
    public static partial class DataTypes
    {
        public class XON_ExtendedCompositeNameForOrganizations : IHL7Type
        {   
            public string OrganizationName { get; set; }
            public IS_CodedValue OrganizationNameTypeCode { get; set; }
            public NM_Number ID { get; set; }
            public string CheckDigit { get; set; }
            public ID_CodedValue CheckDigitScheme { get; set; }
            public HD_HierarchicDesignator AssigningAuthority { get; set; }
            public IS_CodedValue IdentifierTypeCode { get; set; }
            public HD_HierarchicDesignator AssigningFacility { get; set; }

            public int TablesRequired => 3 + (2 * new HD_HierarchicDesignator().TablesRequired); // 2 Hds, 2 ISs and 1 Id;
            public XON_ExtendedCompositeNameForOrganizations()
            {

            }
            public XON_ExtendedCompositeNameForOrganizations(HL7Element element, IEnumerable<string> tableIds = null)
            {
                Populate(element, tableIds);
            }

            public void Populate(HL7Element element, IEnumerable<string> tableIds = null)
            {
                var tblsUsed = 0;
                OrganizationName = element.ElementValue(0);
                OrganizationNameTypeCode = new IS_CodedValue(element.ElementValue(1), NextTableId(tableIds, ref tblsUsed));
                ID = element.IndexedElement(2);
                CheckDigit = element.ElementValue(3);
                CheckDigitScheme = new ID_CodedValue(element.ElementValue(4), NextTableId(tableIds, ref tblsUsed));
                AssigningAuthority = element.AsHD(5, tableIds?.Skip(tblsUsed));
                tblsUsed += new HD_HierarchicDesignator().TablesRequired;
                IdentifierTypeCode = new IS_CodedValue(element.ElementValue(6), NextTableId(tableIds, ref tblsUsed));
                AssigningFacility = element.AsHD(7, tableIds); 
            }

            public override string ToString()
            {
                return ToString('^');
            }
            public virtual string ToString(char sepatator)
            {
                return $"{OrganizationName}{sepatator}{OrganizationNameTypeCode.BestValue}{sepatator}{ID}" +
                    $"{sepatator}{CheckDigit}{sepatator}{CheckDigitScheme.BestValue}" +
                    $"{sepatator}{AssigningAuthority.ToString()}{sepatator}{IdentifierTypeCode.BestValue}" +
                    $"{sepatator}{AssigningFacility.ToString()}";
            }
        }

    }
}

