using System;
using System.Collections.Generic;
using System.Linq;

namespace HL7Enumerator.Types
{
    public static partial class DataTypes
    {
        public class CX_CompositeId : IHL7Type
        {
            public CX_CompositeId()
            {

            }
            public CX_CompositeId(HL7Element element, IEnumerable<string> tableIds = null)
            {
                Populate(element, tableIds);
            }
            public void Populate(HL7Element element, IEnumerable<string> tableIds = null)
            {
                var tblsUsed = 0;
                ID = element.ElementValue(0);
                CheckDigit = element.ElementValue(1);
                CheckDigitScheme = new ID_CodedValue(element.ElementValue(2), NextTableId(tableIds, ref tblsUsed));
                AssigningAuthority = element.AsHD(3, tableIds?.Skip(tblsUsed));
                if (AssigningAuthority != null) tblsUsed += AssigningAuthority.TablesRequired;

                IdentifierTypeCode = element.ElementValue(4);
                AssigningFacility = element.AsHD(5, tableIds?.Skip(tblsUsed));
                if (AssigningFacility!=null) tblsUsed += AssigningFacility.TablesRequired;

                EffectiveDate = element.FromTS(6);
                ExpirationDate = element.FromTS(7);

            }
            public string ID { get; set; }
            public string CheckDigit { get; set; }
            public ID_CodedValue CheckDigitScheme { get; set; }
            public HD_HierarchicDesignator AssigningAuthority { get; set; }
            public string IdentifierTypeCode { get; set; }
            public HD_HierarchicDesignator AssigningFacility { get; set; }
            public DateTime? EffectiveDate { get; set; }
            public DateTime? ExpirationDate { get; set; }

            public int TablesRequired => 1 + (2 * new HD_HierarchicDesignator().TablesRequired); //2Hds and 1 ID

            public override string ToString()
            {
                return ToString('^');
            }
            public string ToString(char separator)
            {
                var ns = NextSeparator(separator);
                return
                    $"{ID}{separator}{CheckDigit}{separator}{CheckDigitScheme.BestValue}{separator}" +
                    $"{AssigningAuthority.ToString(ns)}{separator}{AssigningFacility.ToString(ns)}" +
                    $"{EffectiveDate?.AsDTLocal()}{separator}{ExpirationDate?.AsDTLocal()}";
            }
        }

    }
}

