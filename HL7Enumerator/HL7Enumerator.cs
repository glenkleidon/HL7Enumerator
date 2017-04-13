﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HL7Enumerator
{
    public static class Constants {
        public const string MSHSeparators = "|^~\\&";
        public const string Separators =  "\r|~^&\\"; // note: the ~ is deliberatly out of order...
        public static readonly string[] HeaderTypes = { "FHS", "BHS", "MSH" };
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
        /// new operator Or the ToString() methods.
        /// </summary>
        /// <param name="msgText"></param>
        public static implicit operator HL7Message(string msgText) { return new HL7Message(msgText); }
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
