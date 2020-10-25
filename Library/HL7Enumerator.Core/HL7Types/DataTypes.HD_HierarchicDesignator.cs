namespace HL7Enumerator.Types
{
    public static partial class DataTypes
    {
        public class HD_HierarchicDesignator
        {
            public HD_HierarchicDesignator()
            {

            }
            public HD_HierarchicDesignator(HL7Element element)
            {
                NamespaceId = element.ElementValue(0);
                UniversalId = element.ElementValue(1);
                UniversalIdType = element.ElementValue(2);
            }
            public string NamespaceId { get; set; }
            public string UniversalId { get; set; }
            public string UniversalIdType { get; set; }
            public override string ToString()
            {
                return ToString('^');
            }
            public string ToString(char separator)
            {
                return $"{NamespaceId}^{UniversalId}^{UniversalIdType}";
            }
        }

    }
}

