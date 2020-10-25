namespace HL7Enumerator.Types
{
    public static partial class DataTypes
    {
        public class XON_ExtendedCompositeNameForOrganizations 
        {   
            public string OrganizationName { get; set; }
            public IS_CodedValue OrganizationNameTypeCode { get; set; }
            public NM_Number ID { get; set; }
            public string CheckDigit { get; set; }
            public ID_CodedValue CheckDigitScheme { get; set; }
            public HD_HierarchicDesignator AssigningAuthority { get; set; }
            public IS_CodedValue IdentifierTypeCode { get; set; }
            public HD_HierarchicDesignator AssigningFacility { get; set; }
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

