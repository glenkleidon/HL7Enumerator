using System;
namespace HL7Enumerator.Types
{
    public static partial class DataTypes
    {
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

    }
}

