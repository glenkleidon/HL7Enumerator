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
            public string Country { get; set; }
            public string AddressType { get; set; }
            public string OtherGeographicDesignation { get; set; }
            public string CountyOrParishCode { get; set; }
            public string CensusTract { get; set; }
            public string AddressRepresentationCode { get; set; }
            public DR_DateRange AddressValidityRange { get; set; }
            public override string ToString()
            {
                return ToString('^');
            }
            public string ToString(char sepatator)
            {
                return $"{StreetAddress.ToString()}{sepatator}{OtherDesignation}{sepatator}{City}" +
                    $"{sepatator}{StateOrProvince}{sepatator}{ ZipOrPostalCode}" +
                    $"{sepatator}{Country}{sepatator}{AddressType}{sepatator}{OtherGeographicDesignation}" +
                    $"{sepatator}{CountyOrParishCode}{sepatator}{CensusTract}{sepatator}{AddressValidityRange.ToString()}";
            }
        }

    }
}

