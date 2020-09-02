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
            var delimiterLength = delimiter.Length;
            
            var p = text.IndexOf(delimiter);
            if (fieldPosition == 1)
            {
                if (p == 0) return string.Empty;
                if (p < 0) return text;
                return text.Substring(0, p);
            }
            else
            {
                if (p < 0) return string.Empty;
            }
            // At this point there is at least 1 delimiter and the field position is >1 
            int sp = 0;
            int c = 1;
            while (c++ < fieldPosition && p>=0)
            {
                sp = p+delimiterLength;
                p = text.IndexOf(delimiter, sp);
            }
            if (p < 0)
            {
                if (c <= fieldPosition)
                {
                    return string.Empty;
                }
                else
                {
                    p = text.Length;
                }
            }
            return text.Substring(sp, p-sp);
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
        public static string BoundedBy(string text, string startWith, string endWith, int occurrence=1, bool copyToEndIfStartFound=false)
        {
            if (string.IsNullOrEmpty(startWith) || string.IsNullOrEmpty(endWith) || string.IsNullOrEmpty(text)) return string.Empty;
            var c = 0;
            var p = 0;
            var sp = 0;
            var startWithLength = startWith.Length;

            while (c++ < occurrence & p>=0)
            {
                p = text.IndexOf(startWith, sp);
                sp = p + startWithLength;
            }
            if (p < 0) return string.Empty;
            int q = 0;
            if (p >= 0)
            {
                p = sp;
                q = text.IndexOf(endWith, sp);
            }
            
            if (copyToEndIfStartFound && q < 0) q = text.Length;
            return (q > p) ? text.Substring(p, q - p) : string.Empty;
        }
    }
}
