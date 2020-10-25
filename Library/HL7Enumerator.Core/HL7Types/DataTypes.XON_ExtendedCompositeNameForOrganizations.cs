namespace HL7Enumerator.Types
{
    public static partial class DataTypes
    {
        public class XON_ExtendedCompositeNameForOrganizations 
        {   
            public string OrganizationName { get; set; }
            public string OrganizationNameTypeCode { get; set; }
            public NM_Number ID { get; set; }
            public string CheckDigit { get; set; }
            public string CheckDigitScheme { get; set; }
            public HD_HierarchicDesignator AssigningAuthority { get; set; }
            public string IdentifierTypeCode { get; set; }
            public HD_HierarchicDesignator AssigningFacility { get; set; }
        }

    }
}

