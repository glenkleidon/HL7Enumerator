using System.Collections.Generic;

namespace HL7Enumerator.Types
{
    public static partial class DataTypes
    {
        public class ED_EncapsulatedData : IHL7Type
        {
            public ED_EncapsulatedData()
            {

            }
            public ED_EncapsulatedData(HL7Element element, IEnumerable<string> tableIds = null)
            {
                Populate(element, tableIds);

            }
            public void Populate(HL7Element element, IEnumerable<string> tableIds = null)
            {
                if (element.Count == 0)
                    Data = element;
                else
                {
                    var tblsUsed = 0;
                    SourceApplication = element.AsHD(0, tableIds);
                    if (SourceApplication != null) tblsUsed += SourceApplication.TablesUsed;
                    TypeOfData = new ID_CodedValue(element.ElementValue(1), NextTableId(tableIds, ref tblsUsed));
                    DataSubType = new ID_CodedValue(element.ElementValue(2), NextTableId(tableIds, ref tblsUsed));
                    Encoding = new ID_CodedValue(element.ElementValue(3), NextTableId(tableIds, ref tblsUsed));
                    Data = element.ElementValue(4);
                };
            }
            public HD_HierarchicDesignator SourceApplication;
            public ID_CodedValue TypeOfData;
            public ID_CodedValue DataSubType;
            public ID_CodedValue Encoding;
            public string Data;

            public int TablesUsed => 3 + (new HD_HierarchicDesignator().TablesUsed); // 1 HD and 3 IDs;

            public override string ToString()
            {
                return ToString('&');
            }
            public string ToString(char separator)
            {
                return $"{SourceApplication.ToString()}{separator}{TypeOfData.BestValue}{separator}"+
                    $"{DataSubType.BestValue}{separator}{Encoding.BestValue}{separator}{Data}";
            }
        }

    }
}

