using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HL7Enumerator
{
    public enum HL7ElementType { None = 0, Batch, Message, Segment, Field, FieldRepetition, Component, SubComponent, Escape }

    /// <summary>
    /// Holds known constants for the HL7Enumerator Class
    /// </summary>
    public static class Constants {
        public const string MSHSeparators = "|^~\\&";
        public const string Separators =  "\r|~^&\\"; // note: the ~ is deliberatly out of order...
        public static readonly string[] HeaderTypes = { "FHS", "BHS", "MSH" };
        public static string ToMSHSeparators(string separators = Separators) {
            // reorder separators to MSH order
            if (string.IsNullOrEmpty(separators) || separators.Equals(Separators)) return MSHSeparators;
            string sep=separators+new string(' ', separators.Length-6);
            if (separators[0].Equals('\r')) sep = separators.Substring(1);
            return (new char[]  {sep[0], sep[2], sep[1],sep[4], sep[3]}).ToString();
        }
    }

    public class HL7Element : List<HL7Element> {
        private char _separator;
        private string _separators;
        private HL7Element _parentElement;
        private HL7Reference _reference;
        private HL7ElementType _elementType;
        private string value;

        public char Separator { get { return _separator; } }
        public HL7Element Parent { get { return _parentElement; } }
        public HL7Reference Reference { get { return _reference; } }
        public HL7ElementType ElementType {get {return _elementType;} }
        public string Value { get { return value; } }


        private bool LastSeparator(int index, string data, string separators)
        {
            if (string.IsNullOrEmpty(data)) return true;

            var searchChars = separators.Substring(index).ToCharArray();

            return (searchChars.Length <= 1) || (data.IndexOfAny(searchChars) < 0);

        }

        private HL7ElementType ElementTypeFromSeparator(char separator)
        {
            return ElementTypeFromSeparator(_separators.IndexOf(separator));
        }
        /// <summary>
        /// Return the Element type using the Index of the Character that generated the need to create a new element.<br>
        /// NOTE: do not confuse this method with a generic method to cast a HL7Element type from and integer:
        /// This method does not support the BATCH type because the Message Separator is not defined, and the other
        /// index is offset by three to indicate that the "0" is actually for SEGMENT. SO -1 is NONE, 0 is Segment etc.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private HL7ElementType ElementTypeFromSeparator(int index)
        {
            if (index < 0 || index > _separators.Length) return HL7ElementType.None;
             return (HL7ElementType)(index + 3);
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

        public HL7Element() {
            this._reference = new HL7Reference();
            this._elementType = HL7ElementType.None;
        }

        public void AddElement(HL7ElementType elementType, string value, HL7Element data, HL7Element lastElement)  {
            this._elementType = elementType;
            this.value = value;
            if (data != null)
            {
                data._parentElement = this;
                this.Add(data);
            }
            switch (elementType) {
                case HL7ElementType.Field:
                    AddFieldInternal();
                    break;
                case HL7ElementType.Component:
                    AddComponentInternal();
                    break;
                case HL7ElementType.SubComponent:
                    AddSubComponentInternal();
                    break;
                case HL7ElementType.None:
                    break;
                case HL7ElementType.Batch:
                    break;
                case HL7ElementType.Message:
                    break;
                case HL7ElementType.Segment:
                    AddSegmentInternal();
                    break;
                case HL7ElementType.FieldRepetition:
                    AddFieldRepetitionInternal();
                    break;
                case HL7ElementType.Escape:
                    throw new ArgumentException("Cannot create an element of Escape Type");
                default:
                    throw new ArgumentException("Unexpected ElementType");
            }
        }

        private void AddSegmentInternal()
        {
            var parentElementType = HL7ElementType.None;
            if (this._parentElement != null) parentElementType = _parentElement.ElementType;
            switch (parentElementType)
            {
                case HL7ElementType.Field:
                    //Parent is actually sibling 
                    _parentElement = _parentElement._parentElement;
                    _parentElement.Add(this);
                    break;
                case HL7ElementType.FieldRepetition:
                case HL7ElementType.Segment:
                    _parentElement.Add(this);
                    break;
                case HL7ElementType.None:
                    //Add a new Message 
                    if (value == null) throw new ArgumentNullException("Segment has no name");
                    if (value.Length != 3) throw new ArgumentException(string.Format("Invalid Segment name '{0}'", value));
                    _reference.Segment = value;
                    var newParent = new HL7Element();
                    newParent.AddElement(HL7ElementType.Message, null, this, null);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(string.Format("Unexpected elementType {0}", _elementType.ToString()));
            }
            if (Parent != null)
            {
                _reference = (HL7Reference)Parent.Reference.Clone();
                _reference.Field = Parent.Count;
            }
        }

        private void AddFieldInternal()
        {
            var parentElementType = HL7ElementType.None;
            if (this._parentElement != null) parentElementType = _parentElement.ElementType;
            switch (parentElementType)
            {
                case HL7ElementType.Field:
                    //Parent is actually sibling 
                    _parentElement = _parentElement._parentElement;
                    _parentElement.Add(this);
                    break;
                case HL7ElementType.FieldRepetition:
                case HL7ElementType.Segment:
                    _parentElement.Add(this);
                    break;
                case HL7ElementType.None:
                    //Add a new segment.
                    var newParent = new HL7Element();
                    newParent.AddElement(HL7ElementType.Segment, value, this, null);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(string.Format("Unexpected elementType {0}", _elementType.ToString()));
            }
            if (Parent != null)
            {
                _reference = (HL7Reference)Parent.Reference.Clone();
                this._reference.Field = Parent.Count;
            }
        }

        private void AddFieldRepetitionInternal()
        {
            var lastElementType = HL7ElementType.None;
            if (this._parentElement != null) lastElementType = _parentElement.ElementType;
            switch (lastElementType)
            {
                case HL7ElementType.Field:
                    //I was in a field and I hit a ~ so, parent is now going to be a child of me 
                    if (Parent._parentElement._elementType == HL7ElementType.FieldRepetition)
                    {
                        //Already a parent repition, so just add to that one.
                        Parent.Parent.Add(this);
                    }
                    else if (Parent._parentElement._elementType == HL7ElementType.Field)
                    {
                        var newParent = new HL7Element();
                        newParent.AddElement(HL7ElementType.FieldRepetition, null, this, this.Parent);
                    }
                    else throw new ArgumentException("Unexpected Field repetition character");
                    break;
                case HL7ElementType.FieldRepetition:
                    this._parentElement.Add(this);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(string.Format("Unexpected elementType {0}", _elementType.ToString()));
            }
            if (Parent != null)
            {
                _reference = (HL7Reference)Parent.Reference.Clone();
                _reference.FieldRepetition = _parentElement.Count;
            }
        }

        private void AddComponentInternal()
        {
            var lastElementType = HL7ElementType.None;
            if (Parent != null) lastElementType = Parent.ElementType;
            switch (lastElementType)
            {
                case HL7ElementType.FieldRepetition:
                    //I was in a field repetition, so I need a new field
                    var newParent = new HL7Element();
                    newParent.AddElement(HL7ElementType.Field, null, this, this.Parent);
                    break;
                case HL7ElementType.Field:
                    //I was in a field and I hit a ^ so just Add me
                    Parent.Add(this);
                    break;
                case HL7ElementType.Component:
                    //I was in a component and I hit a ^ so parent is actually sibling, so add siblings parent
                    _parentElement = _parentElement._parentElement;
                    Parent.Add(this);
                    break;
                case HL7ElementType.SubComponent:
                    //I was in subcomponent and I hit a ^, so I need to add on to the subcomponents parent;
                    if (Parent.Parent.Parent != null && Parent.Parent.Parent.ElementType==HL7ElementType.Field) {
                        this._parentElement = _parentElement._parentElement._parentElement;
                        Parent.Add(this);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(string.Format("Unexpected elementType {0}", _elementType.ToString()));
            }
            if (Parent != null)
            {
                _reference = (HL7Reference)Parent.Reference.Clone();
                _reference.Component = _parentElement.Count;
            }
        }

        private void AddSubComponentInternal()
        {
            var lastElementType = HL7ElementType.None;
            if (Parent != null) lastElementType = Parent.ElementType;
            switch (lastElementType)
            {
                case HL7ElementType.Component:
                    //I was in a component and I hit a & so just add me
                    Parent.Add(this);
                    break;
                case HL7ElementType.SubComponent:
                    //I was in subcomponent and I hit a &, so parent is actually sibling;
                    if (Parent.Parent != null && Parent.Parent.ElementType == HL7ElementType.Component)
                    {
                        this._parentElement = _parentElement._parentElement;
                        Parent.Add(this);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(string.Format("Unexpected elementType {0}", _elementType.ToString()));
            }
            if (Parent != null)
            {
                _reference = (HL7Reference)Parent.Reference.Clone();
                _reference.SubComponent = Parent.Count;
            }
        }


        public HL7Element(Stream data, char separator, string separators = Constants.Separators, HL7Element owner = null) : this()
        {
            _separator = separator;
            _separators = separators;
            _parentElement = owner;
            var builder = new StringBuilder();
            var b = data.ReadByte();
            HL7Element currentElement=null;
            while (b>-1) {
                char c = Convert.ToChar(b);
                var index = separators.IndexOf(c);
                if (index >= 0)
                {
                    var et = ElementTypeFromSeparator(index);
                    if (et == HL7ElementType.Escape &&
                        // check for MSH/BHS/FHS 1...
                        (currentElement != null &&
                            (Constants.HeaderTypes.Any(h => h.Equals(currentElement.Reference.Segment))) &&
                            currentElement.Reference.Field == 1
                         )
                        )
                    {
                        builder.Append(c);
                    }
                    else
                    {
                        AddElement(et, builder.ToString(), null, this);
                        builder.Clear();
                    }
                }
                else {
                    builder.Append(c);
                }
                b = data.ReadByte();
            }
        }

        public HL7Element(string data, char separator, string separators = Constants.Separators, HL7Element owner = null)
        {
            _separator = separator;
            _separators = separators;
            _parentElement = owner;

            if (owner == null || separator=='\r') {
                _reference = new HL7Reference();
            }
            else
            {
                _reference = (HL7Reference) owner.Reference.Clone();
            }

            var index = separators.IndexOf(separator);
            _elementType = ElementTypeFromSeparator(index);


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
                    if (result!=null && result.ElementType==HL7ElementType.FieldRepetition) {
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
