using System;
using System.Collections.Generic;
using System.Text;

namespace HL7Enumerator.Core
{
    public static class DataTypes
    {

        public class EntityIdentifier
        {
            public String Identifier { get; set; }
            public string NamespaceId { get; set; }
            public string UniversalId { get; set; }
            public string universalIdType { get; set; }
        }

        public class HD_HierarchicDesignator
        {
            public string NamespaceId { get; set; }
            public string UniversalId { get; set; }
            public string UniversalIdType { get; set; }
        }

        public class CX_CompositeId
        {
            public string ID { get; set; }
            public string CheckDigit { get; set; }
            public string CheckDigitScheme { get; set; }
            public HD_HierarchicDesignator AssigningAuthority { get; set; }
            public string IdentifierTypeCode { get; set; }
            public HD_HierarchicDesignator AssigningFacility { get; set; }
            public DateTime? EffectiveDate { get; set; }
            public DateTime? ExpirationDate { get; set; }
        }
        public static HL7Element IndexedElement(this HL7Element element, int index = -1)
        {
            return (index == -1) ? element : (index < element.Count) ? element[index] : null;

        }

        public static string ElementValue(this HL7Element element, int index)
        {
            return (element.Count > index) ? element[index].Value : (index == 0) ? element.Value : "";
        }

        public static HD_HierarchicDesignator AsHD(this HL7Element element, int index = -1)
        {
            var el = element.IndexedElement(index);
            if (el == null) return null;
            return new HD_HierarchicDesignator()
            {
                NamespaceId = (el.Count == 0) ? el.Value : el[0].Value,
                UniversalId = (el.Count < 2) ? "" : el[1].Value,
                UniversalIdType = (el.Count < 3) ? "" : el[2].Value
            };
        }
        public static DateTime? AsDateTime(this HL7Element element, int index = -1)
        {
            var el = element.IndexedElement(index);
            if (el == null) return null;
            var iso = "";
            var dtPart = el.Value.Substring(0,8);
            switch (el.Value.Length)
            { 
               case 4:
                    dtPart = el.Value;
                    break;
                case 6:
                    dtPart = $"{el.Value.Substring(0, 4)}-{el.Value.Substring(4, 2)}-01";
                    break;
                case 8:
                    dtPart = $"{el.Value.Substring(0, 4)}-{el.Value.Substring(4, 2)}-{el.Value.Substring(4, 2)}";
                    break;
                default:
                    return null;
            }



        }

        public static CX_CompositeId AsCX(this HL7Element element)
        {
            return new CX_CompositeId()
            {
                ID = element.ElementValue(0),
                CheckDigit = element.ElementValue(1),
                CheckDigitScheme = element.ElementValue(2),
                AssigningAuthority = element.AsHD(3),
                IdentifierTypeCode = element.ElementValue(4),
                AssigningFacility = element.ElementValue(5),
                EffectiveDate =
            };
        }



    }
}
