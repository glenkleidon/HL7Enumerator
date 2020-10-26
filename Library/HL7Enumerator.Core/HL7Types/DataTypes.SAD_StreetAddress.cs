namespace HL7Enumerator.Types
{
    public static partial class DataTypes
    {
        public class SAD_StreetAddress
        {
            public string StreetOrMailingAddress { get; set; }
            public string StreetName { get; set; }
            public string DwellingNumber { get; set; }
            public override string ToString()
            {
                return ToString('^');
            }
            public string ToString(char sepatator)
            {
                return $"{StreetOrMailingAddress}{sepatator}{StreetName}{sepatator}{DwellingNumber}";
            }
            public SAD_StreetAddress(HL7Element element)
            {
                StreetOrMailingAddress = element.ElementValue(0);
                StreetName = element.ElementValue(1);
                DwellingNumber = element.ElementValue(2);
            }
        }

    }
}

