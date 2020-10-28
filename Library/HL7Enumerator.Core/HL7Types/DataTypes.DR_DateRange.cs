using System;
namespace HL7Enumerator.Types
{
    public static partial class DataTypes
    {
        public class DR_DateRange
        {
            public DateTime? DateFrom { get; set; }
            public DateTime? DateTo { get; set; }
            public override string ToString()
            {
                return ToString('^');
            }
            public string ToString(char separator)
            {
                return $"{DateFrom?.AsTSLocal()}{separator}{DateTo?.AsTSLocal()}".TrimEnd(separator);
                
            }
        }

    }
}

