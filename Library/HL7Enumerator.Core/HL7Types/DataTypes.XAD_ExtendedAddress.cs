using System;
using System.Collections.Generic;

namespace HL7Enumerator.Types
{
    public static partial class DataTypes
    {
        public class XAD_ExtendedAddress
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

