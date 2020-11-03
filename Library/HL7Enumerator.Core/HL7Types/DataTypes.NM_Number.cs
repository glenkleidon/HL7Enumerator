using System;
namespace HL7Enumerator.Types
{
    public static partial class DataTypes
    {
        public class NM_Number
        {
            private string strValue=null;
            public NM_Number()
            {
            }
            public NM_Number(HL7Element element)
            {
                Value = element;
            }
            public NM_Number(long intValue)
            {
                Value = intValue.ToString();
            }
            public NM_Number(int intValue)
            {
                Value = intValue.ToString();
            }
            public NM_Number(Double dblValue)
            {
                Value = dblValue.ToString();
            }
            public NM_Number(float fltValue)
            {
                Value = fltValue.ToString();
            }
            public NM_Number(decimal decValue)
            {
                Value = decValue.ToString();
            }

            public static implicit operator NM_Number(HL7Element element)
            {
                return new NM_Number(element);
            }
            public bool IsNumber 
            { get
                {
                    return (decimal)this != Decimal.MinValue;
                }
            }

            public string Value { 
                get 
                {
                    return strValue?.Trim();
                }
                set
                {
                    strValue = value?.Trim();   
                }
                     }

            public static implicit operator double(NM_Number number)
            {
                if (number.Value != null)
                {
                    double newDoubleValue;
                    return (Double.TryParse(number.Value, out newDoubleValue)) ? newDoubleValue : Double.MinValue;
                }
                return Double.MinValue;
            }
            public static implicit operator long(NM_Number number)
            {
                if (number.Value == null) return long.MinValue;
                long newInt64Value;
                return (long.TryParse(number.Value, out newInt64Value)) ? newInt64Value : long.MinValue;

            }
            public static implicit operator int(NM_Number number)
            {
                if (number.Value == null) return int.MinValue;
                int newIntValue;
                return (int.TryParse(number.Value, out newIntValue)) ? newIntValue : int.MinValue;
            }
            public static implicit operator float(NM_Number number)
            {
                if (number.Value == null) return float.MinValue;
                float newFloatValue;
                return (float.TryParse(number.Value, out newFloatValue)) ? newFloatValue : float.MinValue;
            }


            public static implicit operator decimal(NM_Number number)
            {
                if (number.Value == null) return decimal.MinValue;
                decimal newDecValue;
                return (Decimal.TryParse(number.Value, out newDecValue)) ? newDecValue : Decimal.MinValue;
            }
            public static implicit operator string(NM_Number number)
            {
                if (number.Value == null) return String.Empty;

                string value = number.Value;
                if (value.Contains("."))
                {
                    double newDoubleValue;
                    return (Double.TryParse(value, out newDoubleValue)) ? newDoubleValue.ToString() : String.Empty;
                }
                else
                {
                    long newInt64Value;
                    return (Int64.TryParse(value, out newInt64Value)) ? newInt64Value.ToString() : String.Empty;
                }
            }
            public override string ToString()
            {
                return strValue;
            }
        }

    }
}

