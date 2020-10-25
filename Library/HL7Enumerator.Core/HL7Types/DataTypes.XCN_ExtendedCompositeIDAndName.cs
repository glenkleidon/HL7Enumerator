using System;

namespace HL7Enumerator.Types
{
    public static partial class DataTypes
    {
        public class XCN_ExtendedCompositeIDAndName : XPN_ExtendedPersonName
        {
            public XCN_ExtendedCompositeIDAndName()
            {

            }

            public XCN_ExtendedCompositeIDAndName(HL7Element element)
            {
                if (element.Separator == '~')
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
                Degree = element.ElementValue(6);
                SourceTable = element.ElementValue(7);
                AssigningAuthority = element.IndexedElement(8).AsHD();
                NameTypeCode = element.ElementValue(9);
                IdentifierCheckDigit = element.ElementValue(10);
                CheckDigitScheme = element.ElementValue(11);
                IdentifierTypeCode = element.ElementValue(12);
                AssigningFacility = element.IndexedElement(13).AsHD();
                NameRepresentationCode = element.ElementValue(14);
                NameContext = element.IndexedElement(15).AsCE();
                NameValidityRange = element.IndexedElement(16).AsDateRange();
                NameAssemblyOrder = element.ElementValue(0);

            }
            public string ID { get; set; }
            public string SourceTable { get; set; }
            public HD_HierarchicDesignator AssigningAuthority { get; set; }
            public string IdentifierCheckDigit { get; set; }
            public string CheckDigitScheme { get; set; }
            public string IdentifierTypeCode { get; set; }
            public HD_HierarchicDesignator AssigningFacility { get; set; }
            public DR_DateRange NameValidityRange { get; set; }
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

