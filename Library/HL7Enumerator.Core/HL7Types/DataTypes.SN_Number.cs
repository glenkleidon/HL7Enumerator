using System.Linq;

namespace HL7Enumerator.Types
{
    public static partial class DataTypes
    {
        public class SN_StructuredNumeric : NM_Number
        {
            public SN_StructuredNumeric(HL7Element element)
            {
                FromElement(element);
            }

            private void FromElement(HL7Element element)
            {
                var comparitor = element.ElementValue(0);
                var separator = element.ElementValue(2);
                Comparitor = (Constants.AllowedComparitors.Contains(comparitor)) ? comparitor : "";
                Num1 = new NM_Number(element.IndexedElement(1));
                SeparatorOrSuffix = (Constants.AllowedSeparators.Contains(separator)) ? separator : "";
                Num2 = new NM_Number(element.IndexedElement(3));
            }

            public string Comparitor { get; set; }
            public NM_Number Num1 { get; set; }
            public string SeparatorOrSuffix { get; set; }
            public NM_Number Num2 { get; set; }
            public override string ToString()
            {
                return ToString('^');
            }
            public string ToString(char separator)
            {
                return $"{Comparitor}{separator}{Num1?.Value}{separator}{SeparatorOrSuffix}{separator}{Num2?.Value}"
                    .TrimEnd(separator);
            }
        }

    }
}

