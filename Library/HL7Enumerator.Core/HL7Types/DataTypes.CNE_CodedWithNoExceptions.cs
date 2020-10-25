namespace HL7Enumerator.Types
{
    public static partial class DataTypes
    {
        public class CNE_CodedWithNoExceptions : CE_CodedElement
        {
            public string CodingSystemVersionID { get; set; }
            public string AlternateCodingSystemVersionID { get; set; }
            public string OriginalText { get; set; }
            public override string ToString(char separator)
            {
               return $"{base.ToString(separator)}{separator}{CodingSystemVersionID}{separator}" +
                    $"{AlternateCodingSystemVersionID}{separator}{OriginalText}";
            }
        }

    }
}

