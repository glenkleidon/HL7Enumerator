using System.Collections.Generic;

namespace HL7Enumerator.Types
{
    public static partial class DataTypes
    {
        public class XPN_ExtendedPersonName : IHL7Type
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

            public int TablesUsed => (2 * new CE_CodedElement().TablesUsed) + 4;
            public XPN_ExtendedPersonName()
            {

            }
            public XPN_ExtendedPersonName(HL7Element element, IEnumerable<string> tableIds = null)
            {
                Populate(element, tableIds);
            }

            public void Populate(HL7Element element, IEnumerable<string> tableIds = null)
            {
                var tblsUsed = 0;

                FamilyName = element.ElementValue(0);
                GivenName = element.ElementValue(1);
                SecondGivenNamesOrInitials = element.ElementValue(2);
                Suffix = element.ElementValue(3);
                Prefix = element.ElementValue(4);
                Degree = new IS_CodedValue(element.IndexedElement(5), NextTableId(tableIds, ref tblsUsed));
                NameTypeCode = new ID_CodedValue(element.IndexedElement(6), NextTableId(tableIds, ref tblsUsed));
                NameRepresentationCode = new ID_CodedValue(element.IndexedElement(7), NextTableId(tableIds, ref tblsUsed));
                NameContext = element.IndexedElement(8).AsCE(tableIds);
                tblsUsed += (new CE_CodedElement().TablesUsed);
                NameAssemblyOrder = new ID_CodedValue(element.IndexedElement(9), NextTableId(tableIds, ref tblsUsed));
            }

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

