using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HL7Enumerator
{
    public class DelimitedString
    {
        /// <summary>
        /// Return the nth "Field" defined by a delmiter string
        /// eg: return 2nd field from "ABC, DEF" => Field("ABC, DEF", ", " ,2);
        /// note: delimiter can any number of characters eg "\r\n"
        /// </summary>
        /// <param name="text"></param>
        /// <param name="delimiter"></param>
        /// <param name="fieldPosition"></param>
        /// <param name="alternateEnd"></param>
        /// <returns></returns>
        public static string Field(string text, string delimiter, int fieldPosition, string alternateEnd = "")
        {
            if (string.IsNullOrEmpty(delimiter) || string.IsNullOrEmpty(text)) return string.Empty;
            var dLength = delimiter.Length;
            int p = text.IndexOf(delimiter);
            if  (p < 0) return (fieldPosition==1) ? text : string.Empty;

            int startPoint = 0; 
            int c = 1;
            while (c++ < fieldPosition && p>=0)
            {
                startPoint = p + dLength;
                p = text.IndexOf(delimiter, startPoint);
            }
            p = (p >= 0) ? p - 1 : text.Length; 
            return text.Substring(startPoint, p-startPoint);
        }
        /// <summary>
        /// Return the text bounded by to start and end text 
        /// eg return "ABC" from "[ABC]" => BoundedBy("[ABC]","[","]");
        /// note: Start and end can be the same char.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="startWith"></param>
        /// <param name="endWith"></param>
        /// <param name="copyToEndIfStartFound"></param>
        /// <returns></returns>
        public static string BoundedBy(string text, string startWith, string endWith, bool copyToEndIfStartFound=false)
        {
            if (string.IsNullOrEmpty(startWith) || string.IsNullOrEmpty(endWith) || string.IsNullOrEmpty(text)) return string.Empty;
            var p = text.IndexOf(startWith);
            if (p < 0) return string.Empty;
            int q = 0;
            if (p >= 0) {
                p = p + startWith.Length;
                q = text.IndexOf(endWith, p);
            }
            if (copyToEndIfStartFound && q < 0) q = text.Length;
            return (q > p) ? text.Substring(p, q - p) : string.Empty;
        }
    }
}
