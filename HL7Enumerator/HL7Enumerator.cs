using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            return (searchChars.Length<=1) || (data.IndexOfAny(searchChars)<0);
            
        }

        public HL7Element(string data, char separator, string separators = "\n|~^&",HL7Element owner=null)
        {
            _separator = separator;
            _separators = separators;
            var index = separators.IndexOf(separator);
            if (LastSeparator(index, data, separators))
            {
                this.value = data;
                return; 
            }
            var nextChar=separators[index+1];
            var subElements = data.Split(separator);
            if (index == 1 && owner!=null) owner.value = subElements[0];
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
            var final = result.ToString().Substring(0,result.Length-1); 
            return final;
        }
    }

    public class HL7Message  {
        public string Separators { set; get;}
        private HL7Element _segments;
        public HL7Element Segments { get { return _segments; } }
        public HL7Message(string mesg)
        {
            string Separators='\n'+mesg.Substring(3,3)+mesg[7];
            _segments = new HL7Element(mesg, '\n', Separators, null);
        }
        public List<HL7Element> Segment(string segmentType) {
            return Segments
                   .FindAll( x=>(x.Count>0) && (x[0].Value.Equals(segmentType)) );
        }
    }

    
}
