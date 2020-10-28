using HL7Enumerator.HL7Tables.Interfaces;
using HL7Enumerator.Types.Interfaces;
using System.Collections.Generic;

namespace HL7Enumerator.Types
{
    public static partial class DataTypes
    {
        public class ED_EncapsulatedData : HL7TypeBase, IHL7Type
        {
            public ED_EncapsulatedData()
            {

            }
            public ED_EncapsulatedData(HL7Element element, IEnumerable<string> tableIds = null, IDataTableProvider tables = null)
                : base(element, tableIds, tables)
            {
            }
            public override void Populate(HL7Element element, IEnumerable<string> tableIds = null)
            {
                if (element.Count == 0)
                    Data = element;
                else
                {
                    var tblsUsed = 0;
                    SourceApplication = element.AsHD(0, tableIds, Tables);
                    tblsUsed += HD_HierarchicDesignator.TablesRequired;
                    TypeOfData = NewID(element.ElementValue(1), NextTableId(tableIds, ref tblsUsed));
                    DataSubType = NewID(element.ElementValue(2), NextTableId(tableIds, ref tblsUsed));
                    Encoding = NewID(element.ElementValue(3), NextTableId(tableIds, ref tblsUsed));
                    Data = element.ElementValue(4);
                };
            }
            public HD_HierarchicDesignator SourceApplication;
            public ID_CodedValue TypeOfData;
            public ID_CodedValue DataSubType;
            public ID_CodedValue Encoding;
            public string Data;

            public static int TablesRequired => 3 + (HD_HierarchicDesignator.TablesRequired); // 1 HD and 3 IDs;

            public int DataTablesRequired => TablesRequired;

            public override string ToString()
            {
                return ToString('&');
            }
            public string ToString(char separator)
            {
                return $"{SourceApplication?.ToString()}{separator}{TypeOfData?.BestValue}{separator}" +
                    $"{DataSubType?.BestValue}{separator}{Encoding?.BestValue}{separator}{Data}".TrimEnd(separator);
            }
        }

    }
}

