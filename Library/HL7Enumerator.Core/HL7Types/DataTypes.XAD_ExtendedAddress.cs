using System;
using System.Collections.Generic;

namespace HL7Enumerator.Types
{
    public static partial class DataTypes
    {
        public class XAD_ExtendedAddress : IHL7Type
        {

            public SAD_StreetAddress StreetAddress { get; set; }
            public string OtherDesignation { get; set; }
            public string City { get; set; }
            public string StateOrProvince { get; set; }
            public string ZipOrPostalCode { get; set; }
            public ID_CodedValue Country { get; set; }
            public ID_CodedValue AddressType { get; set; }
            public string OtherGeographicDesignation { get; set; }
            public IS_CodedValue CountyOrParishCode { get; set; }
            public IS_CodedValue CensusTract { get; set; }
            public ID_CodedValue AddressRepresentationCode { get; set; }
            public DR_DateRange AddressValidityRange { get; set; }

            public int TablesUsed => 5;// 3 Ids and 2 ISs;
            public XAD_ExtendedAddress()
            {

            }
            public XAD_ExtendedAddress(HL7Element element, IEnumerable<string> tableIds = null)
            {
                Populate(element, tableIds);
            }

            public void Populate(HL7Element element, IEnumerable<string> tableIds = null)
            {
                var tblsUsed = 0;
                StreetAddress = new SAD_StreetAddress(element.IndexedElement(0));
                OtherDesignation = element.ElementValue(1);
                City = element.ElementValue(2);
                StateOrProvince = element.ElementValue(3);
                ZipOrPostalCode = element.ElementValue(4);
                Country = new ID_CodedValue(element.ElementValue(5), NextTableId(tableIds, ref tblsUsed));
                AddressType = new ID_CodedValue(element.ElementValue(6), NextTableId(tableIds, ref tblsUsed));
                OtherGeographicDesignation = element.ElementValue(7);
                CountyOrParishCode = new IS_CodedValue(element.ElementValue(8), NextTableId(tableIds, ref tblsUsed));
                CensusTract = new IS_CodedValue(element.ElementValue(9), NextTableId(tableIds, ref tblsUsed));
                AddressRepresentationCode = new ID_CodedValue(element.ElementValue(10), NextTableId(tableIds, ref tblsUsed));
                AddressValidityRange = element.AsDateRange(11);
            }

            public override string ToString()
            {
                return ToString('^');
            }
            public string ToString(char sepatator)
            {
                return $"{StreetAddress.ToString()}{sepatator}{OtherDesignation}{sepatator}{City}" +
                    $"{sepatator}{StateOrProvince}{sepatator}{ZipOrPostalCode}" +
                    $"{sepatator}{Country.BestValue}{sepatator}{AddressType.BestValue}{sepatator}{OtherGeographicDesignation}" +
                    $"{sepatator}{CountyOrParishCode.BestValue}{sepatator}{CensusTract.BestValue}{sepatator}" +
                    $"{AddressRepresentationCode.BestValue}{sepatator}{AddressValidityRange.ToString()}";
            }
        }

    }
}

