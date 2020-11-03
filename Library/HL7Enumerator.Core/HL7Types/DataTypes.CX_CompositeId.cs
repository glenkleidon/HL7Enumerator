using HL7Enumerator.HL7Tables.Interfaces;
using HL7Enumerator.Types.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HL7Enumerator.Types
{
    public static partial class DataTypes
    {
        public class CX_CompositeId : HL7TypeBase, IHL7Type
        {
            public CX_CompositeId()
            {

            }
            public CX_CompositeId(HL7Element element, IEnumerable<string> tableIds = null, IDataTableProvider tables=null)
                : base(element, tableIds, tables)
            {
            }
            public override void Populate(HL7Element element, IEnumerable<string> tableIds = null)
            {
                var tblsUsed = 0;
                ID = element.ElementValue(0);
                CheckDigit = element.ElementValue(1);
                CheckDigitScheme = NewID(element.ElementValue(2), NextTableId(tableIds, ref tblsUsed), Tables);
                AssigningAuthority = element.AsHD(3, tableIds?.Skip(tblsUsed), Tables);
                tblsUsed += HD_HierarchicDesignator.TotalCodedFieldCount;

                IdentifierTypeCode = NewID(element.ElementValue(4), NextTableId(tableIds, ref tblsUsed), Tables);
                AssigningFacility = element.AsHD(5, tableIds?.Skip(tblsUsed), Tables);
                tblsUsed += HD_HierarchicDesignator.TotalCodedFieldCount;

                EffectiveDate = element.FromTS(6);
                ExpirationDate = element.FromTS(7);

            }
            public string ID { get; set; }
            public string CheckDigit { get; set; }
            public ID_CodedValue CheckDigitScheme { get; set; }
            public HD_HierarchicDesignator AssigningAuthority { get; set; }
            public ID_CodedValue IdentifierTypeCode { get; set; }
            public HD_HierarchicDesignator AssigningFacility { get; set; }
            public DateTime? EffectiveDate { get; set; }
            public DateTime? ExpirationDate { get; set; }

            public static int TotalCodedFieldCount => 2 + (2 * HD_HierarchicDesignator.TotalCodedFieldCount); //2Hds and 2 IDs

            public int DataTablesRequired => TotalCodedFieldCount;

            public override string ToString()
            {
                return ToString('^');
            }
            public string ToString(char separator)
            {
                var ns = NextSeparator(separator);
                return
                    $"{ID}{separator}{CheckDigit}{separator}{CheckDigitScheme?.BestValue}{separator}" +
                    $"{AssigningAuthority?.ToString(ns)}{separator}{IdentifierTypeCode?.BestValue}{separator}"+
                    $"{AssigningFacility?.ToString(ns)}{EffectiveDate?.AsDTLocal()}{separator}{ExpirationDate?.AsDTLocal()}"
                    .TrimEnd(separator);

            }
            public static implicit operator HL7Element(CX_CompositeId cx)
            {
                return (HL7Element)cx.ToString();
            }
        }

    }
}

