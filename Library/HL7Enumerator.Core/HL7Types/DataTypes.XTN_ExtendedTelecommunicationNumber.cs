using HL7Enumerator.HL7Tables.Interfaces;
using HL7Enumerator.Types.Interfaces;
using System.Collections.Generic;

namespace HL7Enumerator.Types
{
    public static partial class DataTypes
    {
        public class XTN_ExtendedTelecommunicationNumber : HL7TypeBase, IHL7Type
        {
            public TN_TelephoneNumber TelephoneNumber { get; set; } = "";
            public ID_CodedValue TelecommunicationUseCode { get; set; }
            public ID_CodedValue TelecommunicationEquipment { get; set; }
            public string EmailAddress { get; set; }

            public int TablesRequired => 2; // 2 IDs

            public int DataTablesRequired => TablesRequired;

            public XTN_ExtendedTelecommunicationNumber()
            {

            }

            public XTN_ExtendedTelecommunicationNumber(HL7Element element, IEnumerable<string> tableIds = null, IDataTableProvider tables=null)
                :base (element, tableIds, tables)
            {
            }

            public override void Populate(HL7Element element, IEnumerable<string> tableIds = null)
            {
                var tblsUsed = 0;
                TelephoneNumber = element.ElementValue(0);
                TelecommunicationUseCode = NewID(element.ElementValue(1), NextTableId(tableIds, ref tblsUsed));
                TelecommunicationEquipment = NewID(element.ElementValue(2), NextTableId(tableIds, ref tblsUsed));
            }

            public override string ToString()
            {
                return ToString('^');
            }
            public virtual string ToString(char separator)
            {
                return
                    $"{(string)TelephoneNumber}{separator}{TelecommunicationUseCode?.BestValue}"+
                    $"{separator}{TelecommunicationEquipment?.BestValue}" +
                    $"{separator}{EmailAddress}".TrimEnd(separator);
            }
        }

    }
}

