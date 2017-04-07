using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HL7Enumerator
{
    
    public class HL7Element : List<HL7Element> {
        private string _separators;
        private char _separator;
        public string Separators { get { return _separators; } }
        public char Separator { get { return _separator; } }
        
        private string value;
        public string Value { get { return value; } }

        public HL7Element(string data, char separator, string separators = "\n|~^&") {
            _separators = separators;
            _separator = separator;
            var subElements = data.Split(separator);
            var index = separators.IndexOf(separator);
            if (index == separators.Length-1 || string.IsNullOrEmpty(data) ) {
                this.value = data;
            } else {
                char nextchar = separators[index+1];
                foreach (string s in subElements)
                {
                    this.Add(new HL7Element(s, nextchar, separators));
                }
                if (index==0) this.value = subElements[0];
            }
        }

        public override string ToString()
        {
            if (this.Count() == 0) return value;
            string result = "";
            foreach (HL7Element HL7Element in this) {
                return HL7Element.ToString() + Separator;
            }
            return result;
        }
    }

    public class HL7Message  {

        private HL7Element _segments;
        public HL7Element Segments { get { return _segments; } }
        public HL7Message(string mesg)
        {
            string separators='\n'+mesg.Substring(3,3)+mesg[7];
            _segments = new HL7Element(mesg, '\n', separators);
        }
    }

    
}
