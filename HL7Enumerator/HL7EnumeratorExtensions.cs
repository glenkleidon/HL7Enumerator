using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HL7Enumerator.Extensions
{
    public static class HL7EnumeratorExtensions
    {

        /*
         \Cxxyy\	Single-byte character set escape sequence with two hexadecimal values not converted
\E\	Escape character converted to escape character (e.g., ‘\’)
\F\	Field separator converted to field separator character (e.g., ‘|’)
\H\	Start highlighting not converted
\Mxxyyzz\	Multi-byte character set escape sequence with two or three hexadecimal values (zz is optional) not converted
\N\	Normal text (end highlighting) not converted
\R\	Repetition separator converted to repetition separator character (e.g., ‘~’)
\S\	Component separator converted to component separator character (e.g., ‘^’)
\T\	Subcomponent separator converted to subcomponent separator character (e.g., ‘&’)
\Xdd…\	Hexadecimal data (dd must be hexadecimal characters) converted to the characters identified by each pair of digits
\Zdd…\	Locally defined escape sequence not converted
             
             */

        public class NonPrintingCharPos
        {
            public char character;
            public int index;
        }

        public static IEnumerable<NonPrintingCharPos> GetNonPrintableChars(string text)
        {
            return Enumerable.Range(1, text.Length - 1)
                .Select(i => new NonPrintingCharPos { character = text[i], index = i })
                .Where(c => (c.character < 32 || c.character > 127));
        }

        public static string EscapeNonPrintableChars(string text)
        {
            var charpos = GetNonPrintableChars(text).ToList();
            var s = new StringBuilder();
            int p = 0;
            int sp = 0;
            int l;
            while (p < charpos.Count)
            {
                l = charpos[p].index - sp;
                s.Append(text.Substring(sp, l));
                s.Append(@"\X").Append(Convert.ToByte(charpos[p].character).ToString("X2"));
                sp = charpos[p++].index + 1;
                while ((p < charpos.Count) && (charpos[p].index - charpos[p - 1].index) == 1)
                {
                    s.Append(Convert.ToByte(charpos[p].character).ToString("X2"));
                    sp = charpos[p++].index + 1;
                }
                s.Append(@"\");
            }
            s.Append(text.Substring(sp, text.Length - sp));
            return s.ToString();
        }

        public static string EscapeSimpleChars(string text)
        {
            return text
                .Replace(@"\", @"\E\")
                .Replace(@"|", @"\F\")
                .Replace(@"~", @"\R\")
                .Replace(@"^", @"\S\")
                .Replace(@"&", @"\T\");
        }

        /// <summary>
        /// Escapes Text with HL7 encoding characters
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string EscapeText(this string text) {
            return EscapeNonPrintableChars(
                     EscapeSimpleChars(text)
                   );
        }

        /*
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
					
public class Program
{
	
	public static void Main()
	{
		var text = "line 1 \r\nline 2\t and some moreksdhfkahsdfkj.\t" ;
		var lc = new LocateChars();
        var charpos = lc.GetText(text).ToList();
		foreach (CharPos cp in charpos) {
			Console.WriteLine(string.Format("{0} {1}",(int)cp.character,cp.index)); 
		}
		Console.WriteLine(s.ToString());
	}
	

	
	public class LocateChars {
		
  }	
}            */
    }
}
