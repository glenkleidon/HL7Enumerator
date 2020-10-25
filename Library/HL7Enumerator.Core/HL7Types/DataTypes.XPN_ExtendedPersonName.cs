namespace HL7Enumerator.Types
{
    public static partial class DataTypes
    {
        public class XPN_ExtendedPersonName
        {
            public string FamilyName { get; set; }
            public string GivenName { get; set; }
            public string SecondGivenNamesOrInitials { get; set; }
            public string Suffix { get; set; }
            public string Prefix { get; set; }
            public IS_CodedValue Degree { get; set; }
            public ID_CodedValue NameTypeCode { get; set; }
            public ID_CodedValue NameRepresentationCode { get; set; }
            public CE_CodedElement NameContext { get; set; }
            public ID_CodedValue NameAssemblyOrder { get; set; }
            public override string ToString()
            {
                return ToString('^');
            }
            public virtual string ToString(char separator)
            {
                var ns = NextSeparator(separator);
                return
                    $"{FamilyName}{separator}{GivenName}{separator}{SecondGivenNamesOrInitials}" +
                    $"{separator}{Suffix}{separator}{Prefix}{separator}{Degree.BestValue}{separator}" +
                    $"{NameTypeCode.BestValue}{separator}{NameRepresentationCode.BestValue}{separator}"+
                    $"{NameContext.ToString(ns)}{separator}{NameAssemblyOrder.BestValue}";
            }
        }

    }
}

