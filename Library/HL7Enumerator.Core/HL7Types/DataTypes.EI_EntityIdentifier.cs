using System.Collections.Generic;

namespace HL7Enumerator.Types
{
    public static partial class DataTypes
    {
        public class EI_EntityIdentifier
        {
            public string Identifier { get; set; }
            public IS_CodedValue NamespaceId { get; set; }
            public string UniversalId { get; set; }
            public ID_CodedValue UniversalIdType { get; set; }
            public override string ToString()
            {
                return ToString('^');
            }
            public EI_EntityIdentifier()
            {

            }
            public EI_EntityIdentifier(HL7Element element, IEnumerable<string> tableIds=null)
            {
                Identifier = element.ElementValue(0);
                
                var idIndex = 0;
                NamespaceId = new IS_CodedValue(
                    element.ElementValue(1), NextTableId(tableIds, ref idIndex));
                
                UniversalId = element.ElementValue(2);

                UniversalIdType = new ID_CodedValue(element.ElementValue(3),
                      NextTableId(tableIds, ref idIndex));
            }
            public string ToString(char separator)
            {
                return $"{Identifier}{separator}{NamespaceId.BestValue}{separator}{UniversalId}{separator}{UniversalIdType.BestValue}";
            }
        }

    }
}

