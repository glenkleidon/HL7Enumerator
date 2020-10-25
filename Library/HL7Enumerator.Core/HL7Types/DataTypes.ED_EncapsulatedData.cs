namespace HL7Enumerator.Types
{
    public static partial class DataTypes
    {
        public class ED_EncapsulatedData
        {
            public ED_EncapsulatedData()
            {

            }
            public ED_EncapsulatedData(HL7Element element)
            {
                if (element.Count == 0)
                    Data = element;
                else
                {
                    SourceApplication = element.AsHD(0);
                    TypeOfData = element.ElementValue(1);
                    DataSybType = element.ElementValue(2);
                    Encoding = element.ElementValue(3);
                    Data = element.ElementValue(4);
                };

            }
            public HD_HierarchicDesignator SourceApplication;
            public string TypeOfData;
            public string DataSybType;
            public string Encoding;
            public string Data;
            public override string ToString()
            {
                return ToString('&');
            }
            public string ToString(char separator)
            {
                return $"{TypeOfData}{separator}{DataSybType}{separator}{Encoding}{separator}{Data}";
            }
        }

    }
}

