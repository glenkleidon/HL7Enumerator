using HL7Enumerator.HL7Tables.Interfaces;
using HL7Enumerator.Types.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;

namespace HL7Enumerator.Types
{
    public static partial class DataTypes
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

        /// <summary>
        /// Safely locate the desired element If it exists (returning defaults)
        /// </summary>
        /// <param name="element">HL7 Element</param>
        /// <param name="index">the index in the element required</param>
        /// <returns>A HL7 element if it exists</returns>
        public static HL7Element IndexedElement(this HL7Element element, int index = -1)
        {
            
            var el = (index == -1) ? element : (index < element.Count) ? element[index] : null;
            return (el!=null && el.IsRepeatingField && element.Value == null && el.Count == 1) ? el[0] : el;

        }
        /// <summary>
        /// Safely return Value of the desired Element.  This relieves the need to check 
        /// for ethe existence or null value of the required element.
        /// </summary>
        /// <param name="element">The Containing element element</param>
        /// <param name="index">Zero based index of the element required</param>
        /// <returns>A string represenation of the hl7 field.</returns>
        public static string ElementValue(this HL7Element element, int index)
        {
            return (element == null) ? null :
                   (index == -1) ? element.ToString() :
                   (index == 0 && element.Count == 0) ? element.Value :
                   (element.Count > index) ? element[index].ToString() : "";
        }
        /// <summary>
        /// Safely Extract the Supplied element assuming the element contains suitable contents.
        /// </summary>
        /// <param name="element">Element representing or containing the HD Data</param>
        /// <param name="index">Optionally the sub element </param>
        /// <returns>A fully populated HL7 HD object</returns>
        public static HD_HierarchicDesignator AsHD(this HL7Element element, int index = -1, IEnumerable<string> tableIds = null, IDataTableProvider tables = null)
        {
            var el = element.IndexedElement(index);
            if (el == null) return null;
            return new HD_HierarchicDesignator(el, tableIds, tables);
        }
        public static HD_HierarchicDesignator AsHD(this HL7Element element, IEnumerable<string> tableIds = null, IDataTableProvider tables =null)
        {
            return new HD_HierarchicDesignator(element, tableIds, tables);
        }
        /// <summary>
        /// Safely Extract a Date Range from the Supplied element assuming suitable contents;
        /// </summary>
        /// <param name="element"></param>
        /// <param name="index"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Extract a Date Time field from a supplied element assuming suitable content
        /// NOTE: Dates use the DateTime.Kind property to correctly convert the date
        /// from UTC and Local Times.  Use the LocaLtime and UTCTime extensions to safely
        /// handle the returned date.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="index"></param>
        /// <returns>A Date time as UTC or Local Time depending on specificity of field data</returns>
        public static DateTime? FromDT(this HL7Element element, int index = -1)
        {
            var dtTm = element.FromTS(index);
            return dtTm?.Date;
        }
        /// <summary>
        /// Extract a Time field from a supplied element assuming suitable content
        /// NOTE: Dates and Tims use the DateTime.Kind property to correctly convert the date
        /// from UTC and Local Times.  Use the LocaLtime and UTCTime extensions to safely
        /// handle the returned Time.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static TimeSpan? FromTM(this HL7Element element, int index = -1)
        {
            var dtTm = AsDateTime($"20000101{element.ElementValue(index)}");
            return dtTm?.TimeOfDay;
        }
        /// <summary>
        /// Extract a DateTime field from a supplied element assuming suitable content
        /// NOTE: Dates and Tims use the DateTime.Kind property to correctly convert the date
        /// from UTC and Local Times.  Use the LocaLtime and UTCTime extensions to safely
        /// handle the returned DateTime.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="index"></param>
        /// <returns></returns>
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
        public static ICodedDataValue AsCodedValue(this HL7Element element, int index = -1, string tableId = "",
              Dictionary<string, string> table = null)
        {
            string value = element.ElementValue(index);
            return new CodedDataValue(value, table, tableId);
        }
        public static ICodedDataValue AsCodedValue(this HL7Element element, int index = -1, string tableId = "",
              IDataTableProvider tables=null)
        {
            string value = element.ElementValue(index);
            return new CodedDataValue(value, tableId, tables);
        }
        /// <summary>
        /// Safely return a DataTime from a HL7 Timestamp. NOTE: It is not usually necessary
        /// to call this method directly. Use the ExtractTimeZone method 
        /// 
        /// Dates and Tims use the DateTime.Kind property to correctly convert the date
        /// from UTC and Local Times.  Use the LocaLtime and UTCTime extensions to safely
        /// handle the returned DateTime.
        /// </summary>
        /// <param name="hl7TS"></param>
        /// <returns></returns>
        public static DateTime? AsDateTime(string hl7TS)
        {
            var tsDt = ExtractTimeZone(hl7TS, out string zone);
            var dtText = HL7DateTextAsISODateText(tsDt, zone);
            if (String.IsNullOrEmpty(dtText)) return null;
            if (dtText.Length < 5) dtText = $"{dtText}-01";
            if (zone?.Length > 0) return DateTime.Parse(dtText, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
            return DateTime.Parse(dtText, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal);
        }
        /// <summary>
        /// Converts a HL7 Text string to ISO 8601 format
        /// </summary>
        /// <param name="dateText"></param>
        /// <param name="zone"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Return the LOCAL time from a HL7 Encoded string;
        /// NOTE: the DateTime.Kind property will be set appropriately depending on the 
        /// specificity of the TimeStamp String.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime? ToLocalDateTime(string value)
        {
            var dt = AsDateTime(value);
            if (dt == null) return null;
            if (dt.Value.Kind == DateTimeKind.Unspecified) return DateTime.SpecifyKind(dt.Value, DateTimeKind.Local);
            return dt.Value.ToLocalTime();
        }
        /// <summary>
        /// Returns the UTC time from a HL7 Encoded string 
        /// NOTE: the DateTime.Kind property will be set appropriately depending on the 
        /// specificity of the TimeStamp String.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Determine if a HL7 Encoded string represent Local Time.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsLocalTime(string value)
        {
            var tm = AsDateTime(value);
            return (tm == null) ? true : IsLocalTime(tm.Value);
        }
        /// <summary>
        /// Encapsulates the Int Try/Parse for a string value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static int AsInteger(string value)
        {
            if (Int32.TryParse(value, out int intValue))
            {
                return intValue;
            }
            return 0;
        }
        /// <summary>
        /// Automatically convert a HL7 NM to an integer (If possible)
        /// </summary>
        /// <param name="number"></param>
        /// <returns>Integer value or Int.Minimum if not convertable</returns>
        public static int AsInteger(this NM_Number number)
        {
            return (int)number;
        }
        /// <summary>
        /// Automatically convert a HL7 NM to Double value (if possible)
        /// </summary>
        /// <param name="number"></param>
        /// <returns>double value or Double.Minimum if not convertable</returns>
        public static double AsDouble(this NM_Number number)
        {
            return (double)number;
        }
        /// <summary>
        /// Automatically convert a HL7 NM to a Single (Float) value (if possible)
        /// </summary>
        /// <param name="number"></param>
        /// <returns>A Float Value or Float.Minimum if not convertable</returns>
        public static float AsFloat(this NM_Number number)
        {
            return (float)number;
        }
        /// <summary>
        /// Automatically convert a HL7 NM to a Decimal value (if possible)
        /// </summary>
        /// <param name="number"></param>
        /// <returns>a Decimal value or Decimal.Minimum if not convertable</returns>
        public static decimal AsDecimal(this NM_Number number)
        {
            return (decimal)number;
        }
        /// <summary>
        /// Automatically convert a HL7 NM to a 64 bit Integer value (if possible)
        /// </summary>
        /// <param name="number"></param>
        /// <returns>An Int64 value or Int64.Minimum if not convertable</returns>
        public static Int64 AsInt64(this NM_Number number)
        {
            return (Int64)number;
        }
        /// <summary>
        /// Safely Extract a HL7 SN (structured Numeric) from a HL7 Element assuming suitable content
        /// </summary>
        /// <param name="element"></param>
        /// <returns>a SN_Structured Numeric</returns>
        public static SN_StructuredNumeric AsSN(this HL7Element element)
        {
            return new DataTypes.SN_StructuredNumeric(element);
        }
        /// <summary>
        /// Safely Extract a HL7 CX (Composite ID) from a HL7 Element assuming suitable content
        /// </summary>
        /// <param name="element"></param>
        /// <returns>A HL7 CX object</returns>
        public static CX_CompositeId AsCX(this HL7Element element, IEnumerable<string> tableIds = null, IDataTableProvider tables = null)
        {
            return new CX_CompositeId(element,tableIds, tables);
        }
        public static IEnumerable<CX_CompositeId> AsCXs(this HL7Element element, IEnumerable<string> tableIds=null, IDataTableProvider tables=null)
        {
            var cxs = new List<CX_CompositeId>();
            if (element.IsRepeatingField)
            {
                cxs.AddRange(element.Select(e => new CX_CompositeId(e, tableIds, tables)));
            }
            else
            {
                cxs.Add(new CX_CompositeId(element, tableIds, tables));
            }
            return cxs;
        }


        /// <summary>
        /// Safely Extract a ED (Encapsulated Data) from a HL7 Element Assuming suitable content (eg HL7 (ED) OBX[5])
        /// </summary>
        /// <param name="element"></param>
        /// <returns>a HL7 ED object</returns>
        public static ED_EncapsulatedData AsED(this HL7Element element, IEnumerable<string> tableIds = null, IDataTableProvider tables = null)
        {
            return new ED_EncapsulatedData(element, tableIds, tables);
        }
        /// <summary>
        /// Safely Extract a HL7 XCN (Extended Composite ID and Name) from a HL7 Element Assuming suitable conent
        /// </summary>
        /// <param name="element"></param>
        /// <returns>A HL7 XCN Object</returns>
        public static XCN_ExtendedCompositeIDAndName AsXCN(this HL7Element element, IEnumerable<string> tableIds = null, IDataTableProvider tables = null)
        {
            return new XCN_ExtendedCompositeIDAndName(element, tableIds, tables);
        }
        /// <summary>
        /// Safely Extract all HL7 XCN (Extended Composite ID and Name) from a HL7 Element assuming suitable content
        /// </summary>
        /// <param name="element"></param>
        /// <returns>An IEnumerable of HL7 XCN Objects</returns>
        public static IEnumerable<XCN_ExtendedCompositeIDAndName> AsXCNs(this HL7Element element, IEnumerable<string> tableIds = null, IDataTableProvider tables = null)
        {
            var xcns = new List<XCN_ExtendedCompositeIDAndName>();
            if (element.IsRepeatingField)
            {
                xcns.AddRange(element.Select(e => new XCN_ExtendedCompositeIDAndName(e, tableIds, tables)));
            }
            else
            {
                xcns.Add(new XCN_ExtendedCompositeIDAndName(element, tableIds, tables));
            }
            return xcns;
        }
        /// <summary>
        /// Safely Extract a HL7 CE Coded Element type from a HL7 Element assuming sutiable content
        /// </summary>
        /// <param name="element"></param>
        /// <returns>a HL7 CE object</returns>
        public static CE_CodedElement AsCE(this HL7Element element, IEnumerable<string> tableIds = null, IDataTableProvider tables=null)
        {
            return new CE_CodedElement(element, tableIds, tables);

        }
        /// <summary>
        /// Safely Extract all HL7 CE Coded Elements type from a HL7 Element assuming sutiable content
        /// </summary>
        /// <param name="element"></param>
        /// <returns>an IEnumerable of HL7 CE objects</returns>

        public static IEnumerable<CE_CodedElement> AsCEs(this HL7Element element, IEnumerable<string> tableIds = null, IDataTableProvider tables=null)
        {
            var ces = new List<CE_CodedElement>();
            if (element.IsRepeatingField)
            {
                ces.AddRange(element.Select(e => new CE_CodedElement(e, tableIds, tables)));
            }
            else
            {
                ces.Add(new CE_CodedElement(element, tableIds, tables));
            }
            return ces;
        }
        public static XAD_ExtendedAddress AsXAD(this HL7Element element, IEnumerable<string> tableIds = null, IDataTableProvider tables = null)
        {
            return new XAD_ExtendedAddress(element, tableIds, tables);
        }

        public static IEnumerable<XAD_ExtendedAddress> AsXADs(this HL7Element element, IEnumerable<string> tableIds = null, IDataTableProvider tables = null)
        {
            var xads = new List<XAD_ExtendedAddress>();
            if (element.IsRepeatingField)
            {
                xads.AddRange(element.Select(e => new XAD_ExtendedAddress(e, tableIds, tables)));
            }
            else
            {
                xads.Add(new XAD_ExtendedAddress(element, tableIds, tables));
            }
            return xads;
        }
        public static XPN_ExtendedPersonName AsXPN(this HL7Element element, IEnumerable<string> tableIds = null, IDataTableProvider tables = null)
        {
            return new XPN_ExtendedPersonName(element, tableIds, tables);
        }
        public static IEnumerable<XPN_ExtendedPersonName> AsXPNs(this HL7Element element, IEnumerable<string> tableIds = null, IDataTableProvider tables = null)
        {
            var xpns = new List<XPN_ExtendedPersonName>();
            if (element.IsRepeatingField)
            {
                xpns.AddRange(element.Select(e => new XPN_ExtendedPersonName(e, tableIds, tables)));
            }
            else
            {
               xpns.Add(new XPN_ExtendedPersonName(element, tableIds, tables));
            };
            return xpns;
        }


        public static HL7Element AsElement(this IEnumerable<CX_CompositeId> cxs)
        {
            var element = new HL7Element(String.Empty, '~');
            if (cxs.Any()) foreach (var cx in cxs) element.Add(cx);
            return element;
        }


        internal static string NextTableId(IEnumerable<string> tableIds, ref int index)
        {
            if (tableIds == null) return null;
            // I am matching first to avoid enumerating it all if not needed.
            var id = tableIds.Skip(index++).FirstOrDefault();
            if (id == null)
            {
                index = 0;
                id = tableIds.First();
            }
            return id;
        }

    }
}

