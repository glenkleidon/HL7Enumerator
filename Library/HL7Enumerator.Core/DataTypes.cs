using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using HL7Enumerator;
namespace HL7Enumerator.Types
{
    public static class DataTypes
    {
        /// <summary>
        /// Converts a DateTime to the LOCAL HL7 DT Format string (YYYYMMDD)  If the Local/Universal Kind 
        /// (DateTime.Kind) property has NOT been set, local Time is assumed.
        /// NOTE: It is very important that Dates such as DATE OF BIRTH are passed in with the DateTime.Kind property
        /// set to Unspecified to ensure the date is not automatically converted to UTC time and perhaps
        /// indicated the wrong DOB.
        /// </summary>
        /// <param name="dt">Date to convert</param>
        /// <returns>HL7 DT Format string</returns>
        public static string AsDTLocal(this DateTime dt)
        {
            return AsTSLocalFmt(dt, "yyyyMMdd");
        }
        /// <summary>
        /// Converts a DateTime to the LOCAL HL7 TM Format string (HHMMSS.SSSS[+/-/Z]HHNN])  If the Local/Universal Kind 
        /// (DateTime.Kind) property has NOT been set, local Time is assumed and no Zone is return
        /// if DateTime.Kind property is Local or UTC, the conversion to Local is performed and the 
        /// Zone information is return.
        /// </summary>
        /// <param name="dt">Time to convert</param>
        /// <returns>HL7 TM Format string</returns>
        public static string AsTMLocal(this DateTime dt)
        {
            return AsTSLocalFmt(dt, "HHmmss.ffff");
        }
        /// <summary>
        /// Converts a DateTime to the LOCAL HL7 TS Format string (YYYYMMDDHHMMSS.SSSS[+/-/Z]HHNN])  If the Local/Universal Kind 
        /// (DateTime.Kind) property has NOT been set, local Time is assumed and no Zone is return
        /// if DateTime.Kind property is Local or UTC, the conversion to Local is performed and the 
        /// Zone information is return.
        /// </summary>
        /// <param name="dt">DateTime to convert</param>
        /// <returns>HL7 TS Format string</returns>
        public static string AsTSLocal(this DateTime dt)
        {
            return AsTSLocalFmt(dt, "yyyyMMddHHmmss.ffff");
        }
        /// <summary>
        /// Converts a DateTime to the LOCAL HL7 TS in the specified Format (YYYYMMDDHHMMSS.SSSS[+/-/Z]HHNN])  If the Local/Universal Kind 
        /// (DateTime.Kind) property has NOT been set, local Time is assumed and no Zone is return
        /// if DateTime.Kind property is Local or UTC, the conversion to Local is performed and the 
        /// Zone information is return.
        /// </summary>
        /// <param name="dt">DateTime to convert</param>
        /// <param name="format">DateTime Format String</param>
        /// <returns>HL7 TS Format string</returns>

        public static string AsTSLocalFmt(DateTime dt, string format)
        {
            switch (dt.Kind)
            {
                case DateTimeKind.Unspecified:
                    return dt.ToString(format);
                case DateTimeKind.Local:
                case DateTimeKind.Utc:
                default:
                    return DateTime.SpecifyKind(dt, DateTimeKind.Local)
                        .ToString($"{format}zzz").Replace(":", "");
            }
        }
        /// <summary>
        /// Converts a DateTime to the UTC HL7 DT Format string (YYYYMMDD)  If the Local/Universal Kind 
        /// (DateTime.Kind) property has NOT been set, UNIVERSAL/UNSPECIFIED Time is assumed NO conversion to UTC is made.
        /// if DateTime.Kind property is Local or UTC, the conversion to UTC is performed.
        /// NOTE: It is very important that Dates such as DATE OF BIRTH are passed in with the DateTime.Kind property
        /// set to Unspecified to ensure the date is not automatically converted to UTC time and perhaps
        /// indicated the wrong DOB.
        /// </summary>
        /// <param name="dt">Date to convert</param>
        /// <returns>HL7 DT Format string</returns>
        public static string AsDTUtc(this DateTime dt)
        {
            return AsTSUtcFmt(dt, "yyyyMMdd");
        }
        /// <summary>
        /// Converts a DateTime to the UTC HL7 TM Format string (HHMMSS.SSSS"Z")  If the Local/Universal Kind 
        /// (DateTime.Kind) property has NOT been set, local Time is assumed and the Convertsion to UTC is performed.
        /// if DateTime.Kind property is Local or UTC, the conversion to UTC is performed.
        /// The Zone information always returned.
        /// </summary>
        /// <param name="dt">Time to convert</param>
        /// <returns>HL7 TM Format string</returns>
        public static string AsTMUtc(this DateTime dt)
        {
            return AsTSUtcFmt(dt, "HHmmss.ffff");
        }
        /// <summary>
        /// Converts a DateTime to the LOCAL HL7 TS Format string (YYYYMMDDHHMMSS.SSSS"Z")  If the Local/Universal Kind 
        /// (DateTime.Kind) property has NOT been set, local Time is assumed and the conversion to UTC is made.
        /// if DateTime.Kind property is Local or UTC, the conversion to UTC is performed. The Zone Information "Z"
        /// is always returned.
        /// </summary>
        /// <param name="dt">DateTime to convert</param>
        /// <returns>HL7 TS Format string</returns>
        public static string AsTSUtc(this DateTime dt)
        {
            return AsTSUtcFmt(dt, "yyyyMMddHHmmss.ffff");
        }
        /// <summary>
        /// Converts a DateTime to the UTC HL7 TS in the specified Format (YYYYMMDDHHMMSS.SSSS[+/-/Z]HHNN])  If the Local/Universal Kind 
        /// (DateTime.Kind) property has NOT been set, local Time is assumed and no Zone is return
        /// if DateTime.Kind property is Local or UTC, the conversion to Local is performed and the 
        /// Zone information is return.
        /// </summary>
        /// <param name="dt">DateTime to convert</param>
        /// <param name="format">DateTime Format String</param>
        /// <returns>HL7 TS Format string</returns>

        public static string AsTSUtcFmt(DateTime dt, string format)
        {
            switch (dt.Kind)
            {
                case DateTimeKind.Unspecified:
                    return dt.ToString(format);
                case DateTimeKind.Local:
                case DateTimeKind.Utc:
                default:
                    return dt.ToUniversalTime().ToString($"{format}Z");
            }
        }
        /// <summary>
        /// Get the Separator character of the next lower level given the default Separators
        /// </summary>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static char NextSeparator(char separator)
        {
            var idx = Constants.MSHSeparators.IndexOf(separator);
            return (idx == -1) ? '\r' :
                   (idx > 2) ? Constants.MSHSeparators[4] : Constants.MSHSeparators[idx + 1];
        }

        public class EI_EntityIdentifier
        {
            public string Identifier { get; set; }
            public string NamespaceId { get; set; }
            public string UniversalId { get; set; }
            public string UniversalIdType { get; set; }
            public override string ToString()
            {
                return ToString('^');
            }
            public string ToString(char separator)
            {
                return $"{Identifier}{separator}{NamespaceId}{separator}{UniversalId}{separator}{UniversalIdType}";
            }
        }
        public class ED_EncapsulatedData
        {
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

        public class HD_HierarchicDesignator
        {
            public string NamespaceId { get; set; }
            public string UniversalId { get; set; }
            public string UniversalIdType { get; set; }
            public override string ToString()
            {
                return ToString('^');
            }
            public string ToString(char separator)
            {
                return $"{NamespaceId}^{UniversalId}^{UniversalIdType}";
            }
        }

        public class CX_CompositeId
        {
            public string ID { get; set; }
            public string CheckDigit { get; set; }
            public string CheckDigitScheme { get; set; }
            public HD_HierarchicDesignator AssigningAuthority { get; set; }
            public string IdentifierTypeCode { get; set; }
            public HD_HierarchicDesignator AssigningFacility { get; set; }
            public DateTime? EffectiveDate { get; set; }
            public DateTime? ExpirationDate { get; set; }
            public override string ToString()
            {
                return ToString('^');
            }
            public string ToString(char separator)
            {
                var ns = NextSeparator(separator);
                return
                    $"{ID}{separator}{CheckDigit}{separator}{CheckDigitScheme}{separator}" +
                    $"{AssigningAuthority.ToString(ns)}{separator}{AssigningFacility.ToString(ns)}" +
                    $"{EffectiveDate?.AsDTLocal()}{separator}{ExpirationDate?.AsDTLocal()}";
            }
        }
        public class NM_Number
        {
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

            public string Value { get; set; }

            public static implicit operator double(NM_Number number)
            {
                if (number.Value != null && number.Value.Contains("."))
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
        }
        public class SN_Number : NM_Number
        {
            public string Comparitor { get; set; }
            public NM_Number Num1 { get; set; }
            public string SeparatorOrSuffix { get; set; }
            public NM_Number Num2 { get; set; }
        }

        public class XON_ExtendedCompositeNameForOrganizations 
        {   
            public string OrganizationName { get; set; }
            public string OrganizationNameTypeCode { get; set; }
            public NM_Number ID { get; set; }
            public string CheckDigit { get; set; }
            public string CheckDigitScheme { get; set; }
            public HD_HierarchicDesignator AssigningAuthority { get; set; }
            public string IdentifierTypeCode { get; set; }
            public HD_HierarchicDesignator AssigningFacility { get; set; }
        }
        public class XPN_ExtendedPersonName
        {
            public string FamilyName { get; set; }
            public string GivenName { get; set; }
            public string SecondGivenNamesOrInitials { get; set; }
            public string Suffix { get; set; }
            public string Prefix { get; set; }
            public string Degree { get; set; }
            public string NameTypeCode { get; set; }
            public string NameRepresentationCode { get; set; }
            public CE_CodedElement NameContext { get; set; }
            public string NameAssemblyOrder { get; set; }
            public override string ToString()
            {
                return ToString('^');
            }
            public virtual string ToString(char separator)
            {
                var ns = NextSeparator(separator);
                return
                    $"{FamilyName}{separator}{GivenName}{separator}{SecondGivenNamesOrInitials}" +
                    $"{separator}{Suffix}{separator}{Prefix}{separator}{Degree}{separator}" +
                    $"{NameTypeCode}{separator}{NameRepresentationCode}{separator}"+
                    $"{NameContext.ToString(ns)}{separator}{NameAssemblyOrder}";
            }
        }
        public class TN_TelephoneNumber
        {
            public string CountryCode { get; set; }
            public string AreaCode { get; set; }
            public string Number { get; set; }
            public string Extension { get; set; }
            public string Beeper { get; set; }
            public string Comment { get; set; }
            public TN_TelephoneNumber()
            { 

            }

            public TN_TelephoneNumber(HL7Element element)
            {
                this.Parse((string)element);
            }
            private enum TNState {None, CountryCode, AreaCode, Number, Extension, Beeper, Comment};
            public static bool TryParse(string tn, out TN_TelephoneNumber tnNumber)
            {
                tnNumber = null;
                var number = new TN_TelephoneNumber();
                if (number.Parse(tn, false))
                {
                    tnNumber = number;
                    return true;
                }
                else return false;
            }
            public bool Parse(string tn, bool throwException=true)
            {
                if (string.IsNullOrWhiteSpace(tn))
                {
                    if (throwException) throw new FormatException("Invalid TN Format");
                    else return false;
                }
                var done = false;
                var state = TNState.None;
                var index = 0;
                while (index < tn.Length && !done)
                {
                    char c = tn[index++];
                    switch (state)
                    {
                        case TNState.None:
                            if (Char.IsDigit(c)) state = TNState.CountryCode;
                            else if (c.Equals('(')) state = TNState.AreaCode;
                            break;
                        case TNState.CountryCode:
                            if (!char.IsDigit(c)) state = TNState.AreaCode;
                            else CountryCode = $"{CountryCode}{c}";
                            if (CountryCode.Length > 2)
                            {
                                Number = CountryCode;
                                CountryCode = "";
                                AreaCode = "";
                                state = TNState.Number;
                            }
                            break;
                        case TNState.AreaCode:
                            if (!c.Equals('('))
                            {
                                if (!char.IsDigit(c)) state = TNState.Number;
                                AreaCode = $"{AreaCode}{c}";
                                if (AreaCode.Length > 3)
                                {
                                    Number = AreaCode;
                                    AreaCode = "";
                                }
                            }
                            break;
                        case TNState.Number:
                            if (char.IsDigit(c) || c.Equals('-'))
                            {
                                Number = $"{Number}{c}";
                            }
                            else
                            {
                                switch (c)
                                {
                                    case ' ':
                                        break; // ignore these
                                    case 'X':
                                        state = TNState.Extension;
                                        break;
                                    case 'B':
                                        state = TNState.Beeper;
                                        break;
                                    case 'C':
                                        state = TNState.Comment;
                                        break;
                                    default:
                                        if (throwException) throw new FormatException($"Invalid TN Format Number '{c}' not expected at {1+index}");
                                        else return false;
                                }
                            }
                            break;
                        case TNState.Extension:
                            if (char.IsDigit(c) || c.Equals('-'))
                            {
                                Extension = $"{Extension}{c}";
                            }
                            else
                            {
                                switch (c)
                                {
                                    case ' ':
                                        break; // ignore these
                                    case 'B':
                                        state = TNState.Beeper;
                                        break;
                                    case 'C':
                                        state = TNState.Comment;
                                        break;
                                    default:
                                        if (throwException) throw new FormatException($"Invalid TN Format Extension '{c}' not expected at {1 + index}");
                                        else return false;
                                }
                            }
                            break;
                        case TNState.Beeper:
                            if (char.IsDigit(c) || c.Equals('-'))
                            {
                                Beeper = $"{Extension}{c}";
                            }
                            else
                            {
                                switch (c)
                                {
                                    case ' ':
                                        break; // ignore these
                                    case 'C':
                                        state = TNState.Comment;
                                        break;
                                    default:
                                        if (throwException) throw new FormatException($"Invalid TN Format Beeper '{c}' not expected at {1 + index}");
                                        else return false;
                                }
                            }
                            break;
                        case TNState.Comment:
                            Comment = tn.Substring(index);
                            done = true;
                            break;
                    }

                }
                return true;
            }
            public override string ToString()
            {
                var tn = (CountryCode==null) ? $"{CountryCode} ": "";
                if (AreaCode != null) tn = $"{tn}({AreaCode})";
                tn = $"{tn}{Number}";
                if (Extension != null) tn = $"{tn}X{Extension}";
                if (Beeper != null) tn = $"{tn}B{Beeper}";
                if (Comment != null) tn = $"{tn}C{Comment}";
                return tn;
            }

            public static implicit operator string(TN_TelephoneNumber number)
            {
                return (number==null) ? String.Empty : number.ToString();
            }
            public static implicit operator TN_TelephoneNumber(string tn)
            {
                var tnNumber = new TN_TelephoneNumber();
                tnNumber.Parse(tn);
                return tnNumber;
            }
        }
        public class XTN_ExtendedTelecommunicationNumber
        {
            public TN_TelephoneNumber TelephoneNumber { get; set; }
            public string TelecommunicationUseCode { get; set; }
            public string TelecommunicationEquipment { get; set; }
            public string EmailAddress { get; set; }
        }

        public class XCN_ExtendedCompositeIDAndName:XPN_ExtendedPersonName
        {
            public string ID { get; set; }
            public string SourceTable { get; set; }
            public HD_HierarchicDesignator AssigningAuthority { get; set; }
            public string IdentifierCheckDigit { get; set; }
            public string CheckDigitScheme { get; set; }
            public string IdentifierTypeCode { get; set; }
            public HD_HierarchicDesignator AssigningFacility { get; set; }
            public DR_DateRange NameValidityRange { get; set; }
            public override string ToString(char separator)
            {
                var ns = NextSeparator(separator);
                return
                    $"{ID}{separator}{FamilyName}{separator}{GivenName}{separator}{SecondGivenNamesOrInitials}" +
                    $"{separator}{Suffix}{separator}{Prefix}{separator}{Degree}{separator}" +
                    $"{SourceTable}{separator}{AssigningAuthority.ToString(ns)}{separator}" +
                    $"{NameTypeCode}{separator}{IdentifierCheckDigit}{separator}{CheckDigitScheme}{separator}" +
                    $"{IdentifierTypeCode}{separator}{AssigningFacility.ToString(ns)}{separator}" +
                    $"{NameRepresentationCode}{separator}{NameContext.ToString(ns)}{separator}" +
                    $"{NameValidityRange.ToString(ns)}{separator}{NameAssemblyOrder}";
            }
        }

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
                return $"{DateFrom?.AsTSLocal()}{separator}{DateTo?.AsTSLocal()}";
            }
        }

        public class CE_CodedElement
        {
            public string Identifier { get; set; }
            public string Text { get; set; }
            public string NameOfCodingSystem { get; set; }
            public string AlternateIdentifier { get; set; }
            public string AlternateText { get; set; }
            public string NameOfAlternateCodingSystem { get; set; }
            public override string ToString()
            {
                return ToString('^');
            }
            public string ToString(char sepatator)
            {
                return $"{Identifier}{sepatator}{Text}{sepatator}{NameOfCodingSystem}" +
                    $"{sepatator}{AlternateIdentifier}{sepatator}{AlternateText}" +
                    $"{sepatator}{NameOfAlternateCodingSystem}";
            }
        }


        public static HL7Element IndexedElement(this HL7Element element, int index = -1)
        {
            return (index == -1) ? element : (index < element.Count) ? element[index] : null;
        }

        public static string ElementValue(this HL7Element element, int index)
        {
            return (element == null) ? null :
                   (index == -1) ? element.ToString() :
                   (index == 0 && element.Count == 0) ? element.Value :
                   (element.Count > index) ? element[index].ToString() : "";
        }

        public static HD_HierarchicDesignator AsHD(this HL7Element element, int index = -1)
        {
            var el = element.IndexedElement(index);
            if (el == null) return null;
            return new HD_HierarchicDesignator()
            {
                NamespaceId = (el.Count == 0) ? el.Value : el[0].Value,
                UniversalId = (el.Count < 2) ? "" : el[1].Value,
                UniversalIdType = (el.Count < 3) ? "" : el[2].Value
            };
        }

        public static DR_DateRange AsDateRange(this HL7Element element, int index = -1)
        {
            var el = element.IndexedElement(index);
            if (el == null) return null;
            return new DR_DateRange()
            {
                DateFrom = (el.Count == 0) ? ToLocalDateTime(el.Value) : ToLocalDateTime(el[0].Value),
                DateTo = (el.Count < 2) ? ToLocalDateTime("") : ToLocalDateTime(el[1].Value)
            };
        }
        public static DateTime? FromDT(this HL7Element element, int index = -1)
        {
            var dtTm = element.FromTS(index);
            return dtTm?.Date;
        }
        public static TimeSpan? FromTM(this HL7Element element, int index = -1)
        {
            var dtTm = AsDateTime($"20000101{element.ElementValue(index)}");
            return dtTm?.TimeOfDay;
        }

        public static DateTime? FromTS(this HL7Element element, int index = -1)
        {
            var el = element.ElementValue(index);
            if (String.IsNullOrWhiteSpace(el)) return null;
            try
            {
                return AsDateTime(el);
            }
            catch
            {
                return null;
            }
        }

        public static DateTime? AsDateTime(string hl7TS)
        {
            var tsDt = ExtractTimeZone(hl7TS, out string zone);
            var dtText = HL7DateTextAsISODateText(tsDt, zone);
            if (String.IsNullOrEmpty(dtText)) return null;
            if (dtText.Length < 5) dtText = $"{dtText}-01";
            if (zone?.Length > 0) return DateTime.Parse(dtText, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
            return DateTime.Parse(dtText, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal);
        }

        public static string HL7DateTextAsISODateText(string dateText, string zone = null)
        {
            // Date And Time (DT/TM) are a subset of HL7 Timestamps(TS) 
            //yyyymmddhhmmss.ssss+zzzz
            //0123456789012345678
            // Technically DT does not support TZ, and the "Z" format is 
            // not supported although both forms have been seen in common use

            // Is there a timezone?
            if (zone == null)
            {
                dateText = ExtractTimeZone(dateText, out zone);
                if (!Double.TryParse(dateText, out _)) return null;
            };


            var iso = "";
            if (dateText.Length < 4) return iso;
            // year
            iso = dateText.Substring(0, 4);
            if (dateText.Length < 6) return iso;
            // Month
            iso = $"{iso}-{dateText.Substring(4, 2)}";
            if (dateText.Length < 8) return iso;
            // Day 
            iso = $"{iso}-{dateText.Substring(6, 2)}";
            if (dateText.Length < 10) return iso;


            // HH
            iso = $"{iso}T{dateText.Substring(8, 2)}";
            if (dateText.Length < 12) return $"{iso}{zone}";
            // mm
            iso = $"{iso}:{dateText.Substring(10, 2)}";
            if (dateText.Length < 14) return $"{iso}{zone}";
            // ss
            iso = $"{iso}:{dateText.Substring(12, 2)}";
            if (dateText.Length < 16 || dateText[14] != '.') return $"{iso}{zone}";
            //.ssss
            return $"{iso}{dateText.Substring(14)}{zone}";

        }
        /// <summary>
        /// Extract the TIMEZONE String in ISO Format from the HL7 Timestamp and
        /// return the HL7 TS Excluding the TZ portion.
        /// </summary>
        /// <param name="dateText">Hl7 Timestamp Text</param>
        /// <param name="tzText">The string that will contain the TZ if present</param>
        /// <returns>The Shortened version of the </returns>
        public static string ExtractTimeZone(string dateText, out string tzText)
        {
            var plusMinusPosition = dateText.Length - 5;
            var zPosition = dateText.Length - 1;
            var tz = "";
            if (plusMinusPosition > 0)
            {
                if ("+-".Contains(dateText.Substring(plusMinusPosition, 1)))
                {
                    tz = dateText.Substring(plusMinusPosition, 5);
                    dateText = dateText.Substring(0, plusMinusPosition);
                    if (tz.Substring(1, 4).Equals("0000"))
                    {
                        tz = "Z";
                    }
                    else
                    {
                        tz = $"{tz.Substring(0, 3)}:{tz.Substring(3, 2)}";
                    };

                }
                else if (dateText.Substring(zPosition, 1).Equals("Z", StringComparison.InvariantCultureIgnoreCase))
                {
                    tz = "Z";
                    dateText = dateText.Substring(0, dateText.Length - 1);
                }
            }
            tzText = tz;
            return dateText;
        }

        public static CX_CompositeId AsCX(this HL7Element element)
        {
            return new CX_CompositeId()
            {
                ID = element.ElementValue(0),
                CheckDigit = element.ElementValue(1),
                CheckDigitScheme = element.ElementValue(2),
                AssigningAuthority = element.AsHD(3),
                IdentifierTypeCode = element.ElementValue(4),
                AssigningFacility = element.AsHD(5),
                EffectiveDate = element.FromTS(6),
                ExpirationDate = element.FromTS(7),
            };
        }
        public static ED_EncapsulatedData AsED(this HL7Element element)
        {
            if (element.Count == 0)
                return new ED_EncapsulatedData() { Data = element };

            return new ED_EncapsulatedData()
            {
                SourceApplication = element.AsHD(0),
                TypeOfData = element.ElementValue(1),
                DataSybType = element.ElementValue(2),
                Encoding = element.ElementValue(3),
                Data = element.ElementValue(4)
            };
        }

        public static DateTime? ToLocalDateTime(string value)
        {
            var dt = AsDateTime(value);
            if (dt == null) return null;
            if (dt.Value.Kind == DateTimeKind.Unspecified) return DateTime.SpecifyKind(dt.Value, DateTimeKind.Local);
            return dt.Value.ToLocalTime();
        }
        public static DateTime? ToUTCDateTime(string value)
        {
            var dt = AsDateTime(value);
            if (dt == null) return null;
            if (dt.Value.Kind == DateTimeKind.Unspecified) dt = DateTime.SpecifyKind(dt.Value, DateTimeKind.Local);
            return dt.Value.ToUniversalTime();
        }
        public static bool IsLocalTime(DateTime value)
        {
            return value.Kind != DateTimeKind.Utc;
        }
        public static bool IsLocalTime(string value)
        {
            var tm = AsDateTime(value);
            return (tm == null) ? true : IsLocalTime(tm.Value);
        }
        private static int AsInteger(string value)
        {
            if (Int32.TryParse(value, out int intValue))
            {
                return intValue;
            }
            return 0;
        }

        public static XCN_ExtendedCompositeIDAndName AsXCN(this HL7Element element)
        {
            if (element.Separator == '~')
            {
                throw new InvalidOperationException("AsXCN() called on repeating field.\r\n" +
                    " The Field in question should be treated as an Enumerable type");
            }
            return new XCN_ExtendedCompositeIDAndName()
            {
                ID = element.ElementValue(0),
                FamilyName = element.ElementValue(1),
                GivenName = element.ElementValue(2),
                SecondGivenNamesOrInitials = element.ElementValue(3),
                Suffix = element.ElementValue(4),
                Prefix = element.ElementValue(5),
                Degree = element.ElementValue(6),
                SourceTable = element.ElementValue(7),
                AssigningAuthority = element.IndexedElement(8).AsHD(),
                NameTypeCode = element.ElementValue(9),
                IdentifierCheckDigit = element.ElementValue(10),
                CheckDigitScheme = element.ElementValue(11),
                IdentifierTypeCode = element.ElementValue(12),
                AssigningFacility = element.IndexedElement(13).AsHD(),
                NameRepresentationCode = element.ElementValue(14),
                NameContext = element.IndexedElement(15).AsCE(),
                NameValidityRange = element.IndexedElement(16).AsDateRange(),
                NameAssemblyOrder = element.ElementValue(0)
            };
        }

        public static CE_CodedElement AsCE(this HL7Element element)
        {
            return new CE_CodedElement()
            {
                Identifier = element.ElementValue(0),
                Text = element.ElementValue(1),
                NameOfCodingSystem = element.ElementValue(2),
                AlternateIdentifier = element.ElementValue(3),
                AlternateText = element.ElementValue(4),
                NameOfAlternateCodingSystem = element.ElementValue(5)
            };

        }

    }
}

