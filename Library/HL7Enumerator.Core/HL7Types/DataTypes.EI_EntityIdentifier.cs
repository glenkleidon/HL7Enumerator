namespace HL7Enumerator.Types
{
    public static partial class DataTypes
    {
        public class EI_EntityIdentifier
        {
            public string Identifier { get; set; }
            public string NamespaceId { get; set; }
            public string UniversalId { get; set; }
            public string UniversalIdType { get; set; }
            public override string ToString()
            {
                return ToString('^');
            }
            public string ToString(char separator)
            {
                return $"{Identifier}{separator}{NamespaceId}{separator}{UniversalId}{separator}{UniversalIdType}";
            }
        }

    }
}

