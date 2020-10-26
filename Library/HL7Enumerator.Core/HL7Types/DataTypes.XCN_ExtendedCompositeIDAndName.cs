using System;
using System.Collections.Generic;

namespace HL7Enumerator.Types
{
    public static partial class DataTypes 
    {

        public class XCN_ExtendedCompositeIDAndName : XPN_ExtendedPersonName
        {
            public void Populate(HL7Element element, IEnumerable<string> tableIds = null)
            {
                if (element.IsRepeatingField)
                {
                    throw new InvalidOperationException("AsXCN() called on repeating field.\r\n" +
                        " The Field in question should be treated as an Enumerable type");
                }

                ID = element.ElementValue(0);
                FamilyName = element.ElementValue(1);
                GivenName = element.ElementValue(2);
                SecondGivenNamesOrInitials = element.ElementValue(3);
                Suffix = element.ElementValue(4);
                Prefix = element.ElementValue(5);

                var tblsUsed = 0;
                Degree = new IS_CodedValue(element.ElementValue(6), NextTableId(tableIds, ref tblsUsed));

                SourceTable = element.ElementValue(7);
                AssigningAuthority = element.IndexedElement(8).AsHD(tableIds);
                var hdTbls = new HD_HierarchicDesignator().TablesUsed;
                tblsUsed += hdTbls;

                NameTypeCode = new ID_CodedValue(element.ElementValue(9), NextTableId(tableIds, ref tblsUsed));
                IdentifierCheckDigit = element.ElementValue(10);
                CheckDigitScheme = element.ElementValue(11);
                IdentifierTypeCode = element.ElementValue(12);
                AssigningFacility = element.IndexedElement(13).AsHD(tableIds);
                tblsUsed += hdTbls;
                NameRepresentationCode = new ID_CodedValue(element.ElementValue(14),
                    NextTableId(tableIds, ref tblsUsed));
                NameContext = element.IndexedElement(15).AsCE();

                NameValidityRange = element.IndexedElement(16).AsDateRange();
                NameAssemblyOrder = new ID_CodedValue(element.ElementValue(17), NextTableId(tableIds, ref tblsUsed));
            }

            public XCN_ExtendedCompositeIDAndName()
            {

            }

            public XCN_ExtendedCompositeIDAndName(HL7Element element, IEnumerable<string> tableIds = null)
            {
                Populate(element, tableIds);
            }
            public string ID { get; set; }
            public string SourceTable { get; set; }
            public HD_HierarchicDesignator AssigningAuthority { get; set; }
            public string IdentifierCheckDigit { get; set; }
            public string CheckDigitScheme { get; set; }
            public string IdentifierTypeCode { get; set; }
            public HD_HierarchicDesignator AssigningFacility { get; set; }
            public DR_DateRange NameValidityRange { get; set; }
            public int TablesUsed => (2 * new HD_HierarchicDesignator().TablesUsed);

            public override string ToString(char separator)
            {
                var ns = NextSeparator(separator);
                return
                    $"{ID}{separator}{FamilyName}{separator}{GivenName}{separator}{SecondGivenNamesOrInitials}" +
                    $"{separator}{Suffix}{separator}{Prefix}{separator}{Degree}{separator}" +
                    $"{SourceTable}{separator}{AssigningAuthority.ToString(ns)}{separator}" +
                    $"{NameTypeCode}{separator}{IdentifierCheckDigit}{separator}{CheckDigitScheme}{separator}" +
                    $"{IdentifierTypeCode}{separator}{AssigningFacility.ToString(ns)}{separator}" +
                    $"{NameRepresentationCode}{separator}{NameContext.ToString(ns)}{separator}" +
                    $"{NameValidityRange.ToString(ns)}{separator}{NameAssemblyOrder}";
            }
        }

    }
}

