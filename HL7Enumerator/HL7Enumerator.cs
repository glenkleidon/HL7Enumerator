using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HL7Enumerator
{

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

        public HL7Element(string data, char separator, string separators = "\n|~^&", HL7Element owner = null)
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
            var separators = "\n|~^&";
            char separator = '\n';
            foreach (char c in separators)
            {
                var p = separators.IndexOf(c);
                if (p < 0) continue;
                var q = separators.IndexOf(c,p+1);
                if (q > 0)
                {
                    separator = c;
                    break;
                }
            }
            return new HL7Element(text, '\n');
        }
        public HL7Element Element(SearchCriteriaElement criteria)
        {
            return Element( new SearchCriteriaElement[1] {criteria});  
        }
        public HL7Element Element(SearchCriteriaElement[] searchCriteria)
        {
            // TODO: refactor as a Linq Expression??
            HL7Element result = this;
            foreach (SearchCriteriaElement criteria in searchCriteria)
            {
                if (!criteria.Enabled) return result;
                if (
                      (criteria.Repitition > result.Count) ||
                      (criteria.Position > result[criteria.Repitition].Count)
                ) return null;
                if (criteria.Value.Length > 0)
                {
                    foreach (HL7Element e in result)
                    {
                        var searchElement = e[criteria.Repitition];
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
                    result = result[criteria.Repitition][position];
                }
            }
            return result;
        }
    }

    public class HL7Message {
        public string Separators { set; get; }
        private HL7Element _segments;
        public HL7Element Segments { get { return (_segments == null) ? null : _segments; } }
        public HL7Message(string mesg)
        {
            if (mesg.Length < 8) throw new ArgumentException("Not a valid HL7 message");
            string Separators = '\n' + mesg.Substring(3, 3) + mesg[7];
            _segments = new HL7Element(mesg, '\n', Separators, null);
        }

        public List<HL7Element> AllSegments(string segmentType) {
            return Segments
                   .FindAll(s => (s.Count > 0) && (s[0].Value.Equals(segmentType)));
        }

        public HL7Element Element(SearchCriteria criteria) {
            var segmentElements = AllSegments(criteria.Segment);
            var segments = new HL7Element();
            foreach (HL7Element e in segmentElements) segments.Add(e);
            SearchCriteriaElement[] newCriteria = new SearchCriteriaElement[3];
            for (int i = 1; i < 4; i++) newCriteria[i - 1] = criteria.elements[i];
            return segments.Element(newCriteria);
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
