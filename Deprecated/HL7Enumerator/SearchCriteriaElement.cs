using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HL7Enumerator
{
    public struct SearchCriteriaElement
    {
        public readonly int Repetition;
        public readonly int Position;
        public bool Enabled;
        public bool Skip;
        private string value;
        public string Value { get { return value; } }
        public SearchCriteriaElement(int position, int repetition = 0, string value = "", bool enabled=true, bool skip=false)
        {
            this.Repetition = repetition;
            this.Position = position;
            this.value = value;
            this.Enabled = enabled;
            this.Skip = skip;
        }

        public static implicit operator SearchCriteriaElement(string text) { return ParseElement(text); }

        public static SearchCriteriaElement[] Parse(string criteria) {
            if (criteria.Length == 0) return new SearchCriteriaElement[0];

            var separator = '.';
            if (criteria.IndexOf("/") > 0) separator = '/';
            string[] searchFields = criteria.Split(separator);
            var result = new SearchCriteriaElement[searchFields.Length];

            for (int i = 0; i < searchFields.Length; i++) {
                result[i] = SearchCriteriaElement.ParseElement(searchFields[i]);
            }
            
            return result;
        }

        public static SearchCriteriaElement ParseElement(string criteria)
        {
            int repetition = 0;
            int position = -1;
            bool skip = false;
            string value = "";
            string numberElement = criteria;
            // does it have repition?
            var p = criteria.IndexOf('[');
            if (p >= 0)
            {
                var q = criteria.IndexOf(']', p);
                if (q < 0) throw new ArgumentException(string.Format("Closing bracket not present in {0}", 0));
                numberElement = numberElement.Substring(0, p);
                if (!Int32.TryParse(criteria.Substring(p + 1, q - p - 1), out repetition))
                    throw new ArgumentException(string.Format("Repetition number is not an integer at {0}", criteria));
            }
            if (numberElement.Equals("*")) 
            {
              skip = true;
            } else if (numberElement.IndexOf("'") >= 0)
            {
                value = numberElement.Replace("'", "");
            } else if (!Int32.TryParse(numberElement, out position))
            {
                // if the exception has 3 characters, then we are probably looking for a segment, so ignore and return default
                // otherwise, yes we want to throw an exception
                if (numberElement.Length == 3)
                {
                    value = numberElement;
                }
                else
                {
                    throw new ArgumentException(string.Format("Search criteria position is not an integer at {0}. Are you missing quotes?", criteria));
                }
            }
            return new SearchCriteriaElement(position, repetition, value,true, skip);
        }
    }
}
