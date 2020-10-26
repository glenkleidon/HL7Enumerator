using System.Collections.Generic;

namespace HL7Enumerator.Types
{
    public static partial class DataTypes
    {
        public class HD_HierarchicDesignator: IHL7Type
        {
            public HD_HierarchicDesignator()
            {

            }
            public HD_HierarchicDesignator(HL7Element element, IEnumerable<string> tableIds=null)
            {
                Populate(element, tableIds);
            }
            public void Populate(HL7Element element, IEnumerable<string> tableIds = null)
            {
                var tblsUsed = 0;
                NamespaceId = new IS_CodedValue(element.ElementValue(0),
                    NextTableId(tableIds, ref tblsUsed));
                UniversalId = element.ElementValue(1);
                UniversalIdType = new ID_CodedValue(element.ElementValue(2),
                    NextTableId(tableIds, ref tblsUsed));
            }

            public IS_CodedValue NamespaceId { get; set; }
            public string UniversalId { get; set; }
            public ID_CodedValue UniversalIdType { get; set; }

            public int TablesRequired => 2; // 2 ISs used.

            public override string ToString()
            {
                return ToString('^');
            }
            public string ToString(char separator)
            {
                return $"{NamespaceId.BestValue}^{UniversalId}^{UniversalIdType.BestValue}";
            }
        }

    }
}

