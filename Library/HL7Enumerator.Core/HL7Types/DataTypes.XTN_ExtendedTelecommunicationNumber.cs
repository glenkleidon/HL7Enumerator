namespace HL7Enumerator.Types
{
    public static partial class DataTypes
    {
        public class XTN_ExtendedTelecommunicationNumber
        {
            public TN_TelephoneNumber TelephoneNumber { get; set; }
            public ID_CodedValue TelecommunicationUseCode { get; set; }
            public ID_CodedValue TelecommunicationEquipment { get; set; }
            public string EmailAddress { get; set; }
            public override string ToString()
            {
                return ToString('^');
            }
            public virtual string ToString(char separator)
            {
                return
                    $"{(string)TelephoneNumber}{separator}{TelecommunicationUseCode.BestValue}"+
                    $"{separator}{TelecommunicationEquipment.BestValue}" +
                    $"{separator}{EmailAddress}";
            }
        }

    }
}

