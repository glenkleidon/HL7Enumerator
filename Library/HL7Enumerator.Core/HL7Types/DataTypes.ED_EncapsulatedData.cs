using System.Collections.Generic;

namespace HL7Enumerator.Types
{
    public static partial class DataTypes
    {
        public class ED_EncapsulatedData
        {
            public ED_EncapsulatedData()
            {

            }
            public ED_EncapsulatedData(HL7Element element, IEnumerable<string> tableIds = null)
            {
                if (element.Count == 0)
                    Data = element;
                else
                {
                    SourceApplication = element.AsHD(0);

                    var idIndex = 0;
                    TypeOfData = new ID_CodedValue(element.ElementValue(1), NextTableId(tableIds, ref idIndex));
                    DataSybType = new ID_CodedValue(element.ElementValue(2), NextTableId(tableIds, ref idIndex));
                    Encoding = new ID_CodedValue(element.ElementValue(3), NextTableId(tableIds, ref idIndex));
                    Data = element.ElementValue(4);
                };

            }
            public HD_HierarchicDesignator SourceApplication;
            public ID_CodedValue TypeOfData;
            public ID_CodedValue DataSybType;
            public ID_CodedValue Encoding;
            public string Data;
            public override string ToString()
            {
                return ToString('&');
            }
            public string ToString(char separator)
            {
                return $"{SourceApplication.ToString()}{separator}{TypeOfData.BestValue}{separator}"+
                    $"{DataSybType.BestValue}{separator}{Encoding.BestValue}{separator}{Data}";
            }
        }

    }
}

