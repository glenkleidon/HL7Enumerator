using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HL7Enumerator
{

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
            if (mesg.Length < 8) throw new ArgumentException("Not a complete HL7 message");
            var mesgHeader = mesg.Substring(0, 8);
            _segments = new HL7Element(mesg, '\r', ValidatedSeparators(mesgHeader), null);
        }

        public HL7Message(Stream mesg)
        {
            if (mesg.Length < 8) throw new ArgumentException("Not a complete HL7 message");
            byte[] headerBytes = new byte[8];
            var b =  mesg.Read(headerBytes,0,8);
            var mesgHeader = Convert.ToString(b);
            mesg.Position = 0;
            _segments = new HL7Element(mesg, ValidatedSeparators(mesgHeader), null);
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
        /// Implicit Casting to String from HL7Message format. This is as efficient as calling the constructor directly
        /// </summary>
        /// <param name="msgText"></param>
        public static implicit operator HL7Message(string msgText) { return new HL7Message(msgText); }

        /// <summary>
        /// Implicit Casting from HL7Message to string.  This is slightly safer than calling ToString() directly
        /// </summary>
        /// <param name="msgText"></param>
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
