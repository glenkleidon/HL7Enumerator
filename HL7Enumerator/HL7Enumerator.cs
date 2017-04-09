using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HL7Enumerator
{
    public static class Constants {
        public const string Separators = "\n|~^&";
        public static readonly string[] HeaderTypes = { "FHS", "BHS", "MSH" };
    }

    public class HL7Element : List<HL7Element> {
        private char _separator;
        private string _separators;
        public char Separator { get { return _separator; } }

        private string value;
        public string Value { get { return value; } }

        private bool LastSeparator(int index, string data, string separators)
        {
            if (string.IsNullOrEmpty(data)) return true;

            var searchChars = separators.Substring(index).ToCharArray();

            return (searchChars.Length <= 1) || (data.IndexOfAny(searchChars) < 0);

        }
        public HL7Element() { }

        public HL7Element(string data, char separator, string separators = Constants.Separators, HL7Element owner = null)
        {
            _separator = separator;
            _separators = separators;
            var index = separators.IndexOf(separator);
            if (LastSeparator(index, data, separators))
            {
                this.value = data;
                return;
            }
            var nextChar = separators[index + 1];
            var subElements = data.Split(separator);
            if (index == 1 && owner != null) owner.value = subElements[0];
            foreach (string s in subElements)
            {
                this.Add(new HL7Element(s, nextChar, separators, this));
            }
        }

        public override string ToString()
        {
            if (this.Count() == 0) return value;
            StringBuilder result = new StringBuilder();
            foreach (HL7Element HL7Element in this) {
                result.Append(HL7Element.ToString());
                result.Append(Separator);
            }
            // remove trailing remnants...
            var final = result.ToString().Substring(0, result.Length - 1);
            return final;
        }
        /// <summary>
        /// Implicit Cast (efficiently) from HL7ELement to String
        /// </summary>
        /// <param name="element"></param>
        public static implicit operator string(HL7Element element) => (element == null) ? "": element.ToString();
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
            if (!found) separator = '\n';
            return new HL7Element(text, separator);
        }
        public HL7Element Element(SearchCriteriaElement criteria)
        {
            return Element( new SearchCriteriaElement[1] {criteria});  
        }
        /// <summary>
        /// Returns the element corresponi
        /// </summary>
        /// <param name="searchCriteria"></param>
        /// <returns></returns>
        public HL7Element Element(SearchCriteriaElement[] searchCriteria)
        {
            // TODO: refactor as a Linq Expression??  
            HL7Element result = this;
            foreach (SearchCriteriaElement criteria in searchCriteria)
            {
                var useRepetition = (this == result);
                if (!criteria.Enabled) return result;
                if (
                      (criteria.Repitition > result.Count) ||
                      (useRepetition && criteria.Position > result[criteria.Repitition].Count)
                ) return null;
                if (criteria.Value.Length > 0)
                {
                    foreach (HL7Element e in result)
                    { 
                        // ensure the Element has the correct depth 
                        var searchElement = (e.Count<1) ? e : e[criteria.Repitition];
                        if (searchElement.Value != null && searchElement.Value.Equals(criteria.Value))
                        {
                            result =searchElement;
                            break;
                        }
                    }
                    if (result == null) return null;
                    //return this.FirstOrDefault(e => e.Value.Equals(criteria.Value));
                }
                else
                {
                    var position = (criteria.Position < 1) ? 0 : criteria.Position;
                    result = (useRepetition) ? result[criteria.Repitition][position]: 
                        (position<=result.Count) ? result[position-1] :
                        (position==1) ? result : null;
                    if (result == null) return null;
                }
            }
            return (result==this) ? null: result;
        }
    }

    public class HL7Message {
        public string Separators { set; get; }
        private HL7Element _segments;
        public HL7Element Segments { get { return (_segments == null) ? null : _segments; } }


        /// <summary>
        /// Extracts and validates the separator chars from a HL7 message.
        /// Throws an exception if a MSH, BHS, or FHS has invalid Separator chars OR 
        /// Returns the HL7 Standard characters if no header is present.
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
        private string ValidatedSeparators(string header) {
            var segmentType = header.Substring(0, 3);
            string result = Constants.Separators;
            if (Constants.HeaderTypes.Any(h => h.Equals(segmentType)))
            {
                result = '\n' + header.Substring(3, 3) + header[7];
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

            _segments = new HL7Element(mesg, '\n', ValidatedSeparators(mesgHeader), null);
        }
        /// <summary>
        /// Returns all segments matching the segment type as an array of 
        /// </summary>
        /// <param name="segmentType"></param>
        /// <returns></returns>
        public List<HL7Element> AllSegments(string segmentType) {
            return Segments
                   .FindAll(s => (s.Count > 0) && (s[0].Value.Equals(segmentType)));
        }
        /// <summary>
        /// Returns a specific field based on selection Criteria
        /// NOTE: Selection Criteria can be a string such as "MSH.1.16[2]" or an array of Selection Criteria Elements
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public HL7Element Element(SearchCriteria criteria) {
            var segment = new HL7Element();

            // Locate required Segment
            var segmentElements = AllSegments(criteria.Segment);
            var targetRow = (criteria.SegmentRepitition > 0) ? criteria.SegmentRepitition - 1 : 0;
            if (targetRow > segmentElements.Count-1) return segment; // not found

            segment.Add(segmentElements[targetRow]);
            if (!criteria.Field.Enabled) return segment;  //wants only the full segment

            // locate the required field, components or subcomponent
            var makeHeaderAdjustment = (Constants.HeaderTypes.Any(h => h.Equals(criteria.Segment)));
            SearchCriteriaElement[] newCriteria = new SearchCriteriaElement[3];
            for (int i = 1; i < 4; i++)
            {
                // Make Adjustment for MSH
                newCriteria[i - 1] = (i==1 && criteria.elements[i].Enabled && makeHeaderAdjustment) ?
                    new SearchCriteriaElement(criteria.elements[i].Position-1, 
                                              criteria.elements[i].Repitition,
                                              criteria.elements[i].Value,
                                              true)
                    :
                    criteria.elements[i];
            }
            return segment.Element(newCriteria);
        }

        /// <summary>
        /// Implicit Casting to and from Text and HL7Message format.  These are as efficient as using the 
        /// new operator Or the ToString() methods.
        /// </summary>
        /// <param name="msgText"></param>
        public static implicit operator HL7Message(string msgText) => new HL7Message(msgText) ;
        public static implicit operator string(HL7Message msg) => (
            (msg == null) ? "" : (
                msg._segments.Count<1) ? string.Empty : 
                msg._segments.ToString()
            );

    }


    
}
