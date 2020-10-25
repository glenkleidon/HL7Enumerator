namespace HL7Enumerator.Types
{
    public static partial class DataTypes
    {
        public class CE_CodedElement
        {
            public CE_CodedElement()
            {

            }
            public CE_CodedElement(HL7Element element)
            {
                Identifier = element.ElementValue(0);
                Text = element.ElementValue(1);
                NameOfCodingSystem = element.ElementValue(2);
                AlternateIdentifier = element.ElementValue(3);
                AlternateText = element.ElementValue(4);
                NameOfAlternateCodingSystem = element.ElementValue(5);
            }

            private void FromElement(HL7Element element)
            { 

            }
            public string Identifier { get; set; }
            public string Text { get; set; }
            public string NameOfCodingSystem { get; set; }
            public string AlternateIdentifier { get; set; }
            public string AlternateText { get; set; }
            public string NameOfAlternateCodingSystem { get; set; }
            public override string ToString()
            {
                return ToString('^');
            }
            public virtual string ToString(char sepatator)
            {
                return $"{Identifier}{sepatator}{Text}{sepatator}{NameOfCodingSystem}" +
                    $"{sepatator}{AlternateIdentifier}{sepatator}{AlternateText}" +
                    $"{sepatator}{NameOfAlternateCodingSystem}";
            }
        }

    }
}

