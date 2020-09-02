using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HL7Enumerator
{
    public static class Constants {
        public const string MSHSeparators = "|^~\\&";
        public const string Separators =  "\r|~^&\\"; // note: the ~ is deliberatly out of order...
        public enum HL7Separators { segment, field, repeat, component, subcomponent, escape };
        public static readonly string[] HeaderTypes = { "FHS", "BHS", "MSH" };
        public static string ToSeparators(string mshSeparators) {
            // reorder MSH Separators to Sepator Order
            char[] separators= new char[6];
            int[] translateOrder = { 1, 3, 2, 5, 4 };
            separators[0] = '\r';
            for (int i=1; i<6; i++) separators[i] = Separators[translateOrder[i]];
            return separators.ToString();
        }
        public static string ToMSHSeparators(string separators = Separators) {
            // reorder separators to MSH order
            if (string.IsNullOrEmpty(separators) || separators.Equals(Separators)) return MSHSeparators;
            string sep=separators;
            if (separators[0].Equals('\r')) sep = separators.Substring(1);
            return (new char[]  {sep[0], sep[2], sep[1],sep[4], sep[3]}).ToString();
        }
    }

    public class HL7Element : List<HL7Element> {
        private char _separator;
        private string _separators;
        private HL7Element _parentElement;
        private bool fieldRepetition = false;
        public char Separator { get { return _separator; } }
        public HL7Element Parent { get { return _parentElement; } }

        private string value;
        public string Value { get { return value; } }

        private bool LastSeparator(int index, string data, string separators)
        {
            if (string.IsNullOrEmpty(data)) return true;

            var searchChars = separators.Substring(index).ToCharArray();

            return (searchChars.Length <= 1) || (data.IndexOfAny(searchChars) < 0);

        }

        /// <summary>
        /// Carefully escape the separators ensuring that we dont escape the segment string
        /// </summary>
        /// <param name="text"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        private static string ApplyEscape(string text, char separator, string separators) {
            var escapeChar = separators[separators.Length - 1];
            string escapeSequence = "" + escapeChar + separator;
            string escapeChar1 = ""+escapeChar+Convert.ToChar(1);
            
            // is this the separator sequence?
            if (Constants.ToMSHSeparators(separators).Substring(1).Equals(text)) return text;

            return text.Replace(escapeSequence, escapeChar1);
        }

        private static string RemoveEscape(string text, char separator) {
            return text.Replace((char)1, separator);
        }

        public HL7Element() { }

        public HL7Element(string data, char separator, string separators = Constants.Separators, HL7Element owner = null)
        {
            _separator = separator;
            _separators = separators;
            _parentElement = owner;
            var index = separators.IndexOf(separator);
            fieldRepetition = (index==2);

            if (LastSeparator(index, data, separators))
            {
                this.value = data;
                return;
            }
            var nextChar = separators[index + 1];
            data = ApplyEscape(data, separator, separators);
            var subElements = data.Split(separator);

            if (index == 1 && owner != null) owner.value = subElements[0];
            foreach (string s in subElements)
            {
                this.Add(new HL7Element(RemoveEscape(s, separator), nextChar, separators, this));
            }
        }

        public override string ToString()
        {
            if (this.Count() == 0) return value;
            StringBuilder result = new StringBuilder();
            foreach (HL7Element HL7Element in this) {
                result.Append(HL7Element.ToString())
                      .Append(Separator);
            }
            // remove trailing remnants...
            var final = result.ToString().Substring(0, result.Length - 1);
            return final;
        }
        /// <summary>
        /// Implicit Cast (efficiently) from HL7ELement to String
        /// </summary>
        /// <param name="element"></param>
        public static implicit operator string(HL7Element element)
        {
            return (element == null) ? "" : element.ToString();
        }
        /// <summary>
        /// Implicitly Cast to and from string and HL7Element.
        /// Note: This is inherently slower than using a constructor directly because 
        /// it must locate the correct separator to use - also it assumes standard HL7 separators.
        /// To support non-standard characters use the constructor (ie use the &quot;new&quot;)
        /// <param name="text"></param>
        public static implicit operator HL7Element(string text)
        {
            var found = false;
            char separator = '\0';
            foreach (char c in Constants.Separators) {
                found = (text.IndexOf(c)>0);
                if (found) {
                    separator = c;
                    break;
                }
            }
            if (!found) separator = '\r';
            return new HL7Element(text, separator);
        }
        public HL7Element Element(string criteria)
        { 
            return Element(new SearchCriteria(criteria).elements);  
        }
        /// <summary>
        /// Returns the element corresponding to the set of search criteria.
        /// </summary>
        /// <param name="searchCriteria"></param>
        /// <returns></returns>
        public HL7Element Element(SearchCriteriaElement[] searchCriteria)
        {
            HL7Element result = this;
            var fieldOffset = 0;
            foreach (SearchCriteriaElement criteria in searchCriteria)
            {
                if (!criteria.Enabled) return result;
                if (criteria.Skip) continue;
                if (criteria.Value.Length > 0)
                {
                    foreach (HL7Element e in result)
                    {
                        // ensure the Element has the correct depth 
                        var searchElement = (criteria.Repetition< 1) ? e : e[criteria.Repetition];
                        if (searchElement.Value != null && searchElement.Value.Equals(criteria.Value))
                        {
                            result =searchElement;
                            break;
                        }
                    }
                    if (result == null) return null;
                }
                else
                {
                    var position = (criteria.Position < 1) ? 0 : criteria.Position;
                    result = (position <= result.Count) ? result[position-fieldOffset] : (position==1) ? result : null;
                    if (result!=null && result.fieldRepetition) {
                        var repetition = (criteria.Repetition < 1) ? 0 : criteria.Repetition - 1;
                        result = (repetition < result.Count) ? result[repetition] :
                                (result.Count == 0) ? result : null;
                    } 
                }
                fieldOffset = 1;
            }
            return (result==this) ? null: result;
        }
    }

    public class HL7Message {
        public string Separators { set; get; }
        private HL7Element _segments;
        public HL7Element Segments { get { return (_segments == null) ? null : _segments; } }

        /// <summary>
        /// For a common issue where OBX containing Base64 sometimes contains rogue CRLF
        /// This is common enough to be included in the routine processing.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string EscapeOBXCRLF(string text)
        {
            var builder = new StringBuilder();
            var p = text.IndexOf("OBX|");
            var processedToPosition = 0;
            while (p >= 0)
            {
                var q = p+1;
                var rowsToProcess = false;
                while (q >= 0)
                {
                    q = text.IndexOf('\r', q+1);
                    var isPipe = (q <0 || q>text.Length-4 || text.Substring(q + 4, 1) == "|");
                    if (isPipe && !rowsToProcess) break;
                    if (!isPipe) 
                    {
                       rowsToProcess = true;
                       continue;
                    }
                    // OK, this is a whole OBX escape the for this segment
                    if (q < 0) q = text.Length;
                    builder.Append(text.Substring(processedToPosition, p-processedToPosition))
                           .Append(text.Substring(p, q - p).Replace("\r", @"\X0D\").Replace("\n", @"\X0A\"));
                    processedToPosition = q;
                    rowsToProcess = false;
                    break;
                }
                if (q < 0) break;
                p = text.IndexOf("OBX|", q); // repeat for next one.
            }
            if (processedToPosition != 0 && processedToPosition < text.Length)
                builder.Append(text.Substring(processedToPosition, text.Length - processedToPosition)); 
            return (builder.Length == 0) ? text : builder.ToString();
        }


        /// <summary>
        /// Extracts and validates the separator chars from a HL7 message.
        /// Throws an exception if a MSH, BHS, or FHS has invalid Separator chars OR 
        /// Returns the HL7 Standard characters if no header is present.
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
        private static string ValidatedSeparators(string header) {
            var segmentType = header.Substring(0, 3);
            string result = Constants.Separators;
            if (Constants.HeaderTypes.Any(h => h.Equals(segmentType)))
            {
                result = ""+ '\r' + header[3] + header[5] + header[4] + header[7] + header[6];
                var distinctResult = "";
                foreach (char c in result) {
                    if (distinctResult.IndexOf(c) > 0)
                    {
                        throw new ArgumentException("Message has invalid separator character definition");
                    }
                    else {
                        distinctResult = distinctResult + c;
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// Creates an instance of a HL7 Message
        /// </summary>
        /// <param name="mesg"></param>
        public HL7Message(string mesg)
        {
            if (mesg.Length<8) throw new ArgumentException("Not a complete HL7 message");
            var mesgHeader = mesg.Substring(0, 8);
            if (mesgHeader.Length < 8) throw new ArgumentException("Not a valid HL7 message");

            _segments = new HL7Element(mesg, '\r', ValidatedSeparators(mesgHeader), null);
        }

        public static HL7Element ParseOnly(string mesg, SearchCriteria criteria)
        {
            if (mesg.Length < 8) throw new ArgumentException("Not a complete HL7 message");
            var mesgHeader = mesg.Substring(0, 8);
            if (mesgHeader.Length < 8) throw new ArgumentException("Not a valid HL7 message");

            var separators = ValidatedSeparators(mesgHeader);

            var segmentTerminator = Constants.Separators[0];
            if (criteria.Segment.Length > 0)
            {
                var SegmentRepitition = (criteria.elements[0].Repetition < 2) ? 1 : criteria.elements[0].Repetition;
                var segment = DelimitedString.BoundedBy(mesg, criteria.Segment, segmentTerminator+"", SegmentRepitition);
                if (!criteria.Field.Enabled) return segment; // (implictly cast as element)
                var headerOffset = (Constants.HeaderTypes.Any(h => h.Equals(criteria.Segment))) ? 0 : 1;
                var separator = separators[(int)Constants.HL7Separators.field];
                var field = DelimitedString.Field(segment, ""+separator, criteria.Field.Position+headerOffset);
                if (field.Length == 0) return new HL7Element("",separator,separators);
                if (criteria.Field.Repetition > 1) {
                    field = DelimitedString.Field(
                          field, 
                          "" + separators[(int)Constants.HL7Separators.repeat],
                          criteria.Field.Repetition
                          );
                }
                if (!criteria.Component.Enabled) return new HL7Element(field, separator, separators);
                separator = Constants.Separators[(int)Constants.HL7Separators.component];
                var component = DelimitedString.Field(field,
                     "" + separator,
                     criteria.Component.Position);
                var subcomponentseparator = separators[(int)Constants.HL7Separators.subcomponent];
                return (component.Length==0 || !criteria.Subcomponent.Enabled) ? 
                    new HL7Element(component, separator, separators) 
                    :
                    new HL7Element(
                     DelimitedString.Field(field,
                      "" + subcomponentseparator,
                      criteria.Subcomponent.Position), subcomponentseparator, separators);
                    
            }
            return null;
        }




        /// <summary>
        /// Returns all segments matching the segment type as an array of 
        /// </summary>
        /// <param name="segmentType"></param>
        /// <returns></returns>
        public List<HL7Element> AllSegments(string segmentType) {
            if (segmentType == null) return null;
            return Segments
                   .FindAll(s => (s.Count > 0) && ( (s[0].Value!=null) && (s[0].Value.Equals(segmentType))));
        }
        /// <summary>
        /// Returns a specific field based on selection Criteria
        /// NOTE: Selection Criteria can be a string such as "MSH.1.16[2]" or an array of Selection Criteria Elements
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public HL7Element Element(SearchCriteria criteria) {
            // Locate required Segment
            if (criteria==null || criteria.Segment == null) return null;
            var segmentElements = AllSegments(criteria.Segment);
            var targetRow = (criteria.SegmentRepetition > 0) ? criteria.SegmentRepetition - 1 : 0;
            if (targetRow > segmentElements.Count - 1) return new HL7Element(); // not found;

            if (!criteria.Field.Enabled) return segmentElements[targetRow];  //wants only the full segment

            // locate the required field, components or subcomponent
            var makeHeaderAdjustment = (Constants.HeaderTypes.Any(h => h.Equals(criteria.Segment)));
            SearchCriteriaElement[] newCriteria = new SearchCriteriaElement[3];
            for (int i = 1; i < 4; i++)
            {
                // Make Adjustment for MSH
                newCriteria[i - 1] = (i==1 && criteria.elements[i].Enabled && makeHeaderAdjustment) ?
                    new SearchCriteriaElement(criteria.elements[i].Position-1, 
                                              criteria.elements[i].Repetition,
                                              criteria.elements[i].Value,
                                              true)
                    :
                    criteria.elements[i];
            }
            return segmentElements[targetRow].Element(newCriteria);
        }

        /// <summary>
        /// Implicit Casting to and from Text and HL7Message format.  These are as efficient as using the 
        /// new operator Or the ToString() methods EXCEPT that the EscapeOBXCRLF is also called by default
        /// </summary>
        /// <param name="msgText"></param>
        public static implicit operator HL7Message(string msgText)
        {
            return new HL7Message(EscapeOBXCRLF(msgText));
        }
        public static implicit operator string(HL7Message msg)
        {
              return (
                (msg == null) ? "" : (
                msg._segments.Count < 1) ? string.Empty :
                msg._segments.ToString()
                );
        }

    }


    
}
