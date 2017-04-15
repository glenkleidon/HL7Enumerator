using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HL7Enumerator
{
    /// <summary>
    /// Holds known constants for the HL7Enumerator Class
    /// </summary>
    public static class Constants
    {
        public const string MSHSeparators = "|^~\\&";
        public const string Separators = "\r|~^&\\"; // note: the ~ is deliberatly out of order...
        public static readonly string[] HeaderTypes = { "FHS", "BHS", "MSH" };
        public static string ToMSHSeparators(string separators = Separators)
        {
            // reorder separators to MSH order
            if (string.IsNullOrEmpty(separators) || separators.Equals(Separators)) return MSHSeparators;
            string sep = separators + new string(' ', separators.Length - 6);
            if (separators[0].Equals('\r')) sep = separators.Substring(1);
            return (new char[] { sep[0], sep[2], sep[1], sep[4], sep[3] }).ToString();
        }
    }

    public enum HL7ElementType { None = 0, Batch, Message, Segment, Field, FieldRepetition, Component, SubComponent, Escape }

    public class HL7Element : List<HL7Element>
    {
        private char _separator;
        private string _separators;
        private HL7Element _parentElement;
        private HL7Reference _reference;
        private HL7ElementType _elementType;
        private string value;

        public char SeparatorForChildren { get { return _separator; } }
        public HL7Reference Reference { get { return _reference; } }
        public HL7ElementType ElementType { get { return _elementType; } }
        public string Value { get { return value; } }
        public HL7Element Parent {
            get { return _parentElement; }
            internal set
                {  // Apply Rules here....
                   _parentElement = value;
                }
        }


        private bool LastSeparator(int index, string data, string separators)
        {
            if (string.IsNullOrEmpty(data)) return true;

            var searchChars = separators.Substring(index).ToCharArray();

            return (searchChars.Length <= 1) || (data.IndexOfAny(searchChars) < 0);
        }

        public char SeparatorFromElementType(HL7ElementType type) {
            var index = (int)type - 3;
            if (index < 1 || index > (_separators.Length - 1)) return '\0';
            return _separators[index];
        }

        public char ChildSeparatorFromElementType(HL7ElementType type)
        {
            if (type==HL7ElementType.Field) return SeparatorFromElementType(type + 2);
            if (type == HL7ElementType.SubComponent) return '\0';
            return SeparatorFromElementType(type+1);
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
        private static string ApplyEscape(string text, char separator, string separators)
        {
            var escapeChar = separators[separators.Length - 1];
            string escapeSequence = "" + escapeChar + separator;
            string escapeChar1 = "" + escapeChar + Convert.ToChar(1);

            // is this the separator sequence?
            if (Constants.ToMSHSeparators(separators).Substring(1).Equals(text)) return text;

            return text.Replace(escapeSequence, escapeChar1);
        }

        private static string RemoveEscape(string text, char separator)
        {
            return text.Replace((char)1, separator);
        }

        public HL7Element()
        {
            this._reference = new HL7Reference();
            this._elementType = HL7ElementType.None;
            this._separators = Constants.Separators;
        }

        public void AddElement(HL7ElementType elementType, string value, HL7Element child, HL7Element previousElement)
        {
            this._elementType = elementType;
            this.value = value;
            // Add the separator that CAUSED the element to be added (not the one representing the required Element) 
            if (string.IsNullOrEmpty(this._separators)) this._separators =
                    (previousElement != null && previousElement._separators != null)
                    ? previousElement._separators
                    : Constants.Separators;
            this._separator = ChildSeparatorFromElementType(elementType); 
            if (child != null)
            {
                child._parentElement = this;
                this.Add(child);
            }
            var elementTypeToAdd = (previousElement != null && elementType>HL7ElementType.Segment &&  previousElement.ElementType > elementType)
                ? previousElement.ElementType
                : elementType;
            

            switch (elementTypeToAdd)
            {
                case HL7ElementType.Field:
                    AddFieldInternal(previousElement);
                    break;
                case HL7ElementType.Component:
                    AddComponentInternal(previousElement);
                    break;
                case HL7ElementType.SubComponent:
                    AddSubComponentInternal(previousElement);
                    break;
                case HL7ElementType.None:
                    break;
                case HL7ElementType.Batch:
                    break;
                case HL7ElementType.Message:
                    break;
                case HL7ElementType.Segment:
                    AddSegmentInternal(previousElement);
                    break;
                case HL7ElementType.FieldRepetition:
                    AddFieldRepetitionInternal(previousElement);
                    break;
                case HL7ElementType.Escape:
                    throw new ArgumentException("Cannot create an element of Escape Type");
                default:
                    throw new ArgumentException("Unexpected ElementType");
            }
        }

        public HL7Element AscendTo(HL7ElementType type)
        {
            var searchElement = this;
            while (searchElement != null && (int)searchElement.ElementType > (int)type) searchElement = searchElement.Parent;
            return (searchElement!= null && searchElement.ElementType == type) ? searchElement : null;
        }

        private void AddSegmentInternal(HL7Element previousElement)
        {
            if (previousElement == null || previousElement.ElementType != HL7ElementType.Field)
                throw new ArgumentOutOfRangeException("Unexpected Segment.  Segments must follow a Field definition");
            // When a End of Segment marker is encountered, a next field will have the MESSAGE attached as its parent
            // We need to reparent that field to the segment - if no message, then make one.
            var mesg = previousElement.AscendTo(HL7ElementType.Message);
            if (mesg == null)
            {
                mesg = new HL7Element();
                mesg.AddElement(HL7ElementType.Message, null, this, this);
            }
            else {
                _parentElement = mesg;
                mesg.Add(this);
            }
            value = previousElement.Value;
            _reference.Segment = value;
        }

        private void AddFieldInternal(HL7Element previousElement)
        {
            string newSegmentId = null;
            if (previousElement == null)
            {
                //Try to create a new segment
                var newParent = new HL7Element();
                newParent.AddElement(HL7ElementType.Segment, value, this, this);
                newSegmentId = value;
            }
            else
            {
                var parent = previousElement.AscendTo(HL7ElementType.Segment);
                if (parent != null) _parentElement = parent;
                parent.Add(this);
            }
            if (Parent != null)
            {
                _reference = (HL7Reference)Parent.Reference.Clone();
                _reference.Field = Parent.Count-1;
                if (newSegmentId != null) _reference.Segment = newSegmentId;
            }
        }

        private void AddFieldRepetitionInternal(HL7Element previousElement)
        {
            if (previousElement == null)
            {
                throw new ArgumentOutOfRangeException("Unexpected Repetition Encountered. Expected Field, Component or Subcomponent");
            }
            else
            {
                var previousRepetition = previousElement.AscendTo(HL7ElementType.FieldRepetition);
                if (previousRepetition != null)
                {
                    _parentElement = previousRepetition.Parent;
                }
                else
                {
                    var field = previousElement.AscendTo(HL7ElementType.Field);
                    if (field == null) throw new
                              ArgumentOutOfRangeException("Unexpected Repetition Encountered. Repetition followed an ELement without parent.");
                    // now I have to reparent the LAST field in the segment onto this repetition
                    // this only works if we are adding them in order.  Could be a problem for INSERT
                    _parentElement = field;
                    field[field.Count - 1].Parent = this;
                }
            }
            if (Parent != null)
            {
                _reference = (HL7Reference)Parent.Reference.Clone();
                _reference.FieldRepetition = Parent.Count;
            }
        }

        private void AddComponentInternal(HL7Element previousElement)
        {
            if (previousElement == null)
            {
                throw new ArgumentOutOfRangeException("Unexpected Component encountered. Expected Field, Segment or Message");
            }
            else
            {
                HL7Element parent;
                if (previousElement.ElementType < HL7ElementType.Component)
                {
                    var parentType = HL7ElementType.Field;
                    parent = new HL7Element();
                    parent.AddElement(parentType, null, this, previousElement);
                }
                else
                {
                    parent = previousElement.AscendTo(HL7ElementType.FieldRepetition);
                    if (parent == null) parent = previousElement.AscendTo(HL7ElementType.Field);
                    if (parent == null) throw new ArgumentException("Component types cannot be set without a field");
                    _parentElement = parent;
                    Parent.Add(this);
                }
            }
            if (Parent != null)
            {
                _reference = (HL7Reference)Parent.Reference.Clone();
                _reference.Component = Parent.Count;
            }
        }

        private void AddSubComponentInternal(HL7Element previousElement)
        {
            if (previousElement == null)
            {
                throw new ArgumentOutOfRangeException("Unexpected Component encountered. Expected Field, Segment or Message");
            }
            else
            {
                HL7Element parent;
                if (previousElement.ElementType < HL7ElementType.Component)
                {
                    var parentType = HL7ElementType.Component;
                    parent = new HL7Element();
                    parent.AddElement(parentType, null, this, previousElement);
                }
                else
                {
                    parent = previousElement.AscendTo(HL7ElementType.Component);
                    if (parent == null) throw new ArgumentException("SubComponent types cannot be set without a component");
                    _parentElement = parent;
                    Parent.Add(this);
                }
            }
            if (Parent != null)
            {
                _reference = (HL7Reference)Parent.Reference.Clone();
                _reference.Component = Parent.Count;
            }
        }

        public HL7Element(Stream data, string separators = Constants.Separators, HL7Element previousElement = null) : this()
        {
            //Manage Separators.  
            _separators = separators;

            HL7Element newElement = null;

            var builder = new StringBuilder();
            var b = data.ReadByte();
            while (b > -1)
            {
                char c = Convert.ToChar(b);
                var index = separators.IndexOf(c);
                if (index >= 0)
                {
                    var et = ElementTypeFromSeparator(index);
                    if (et == HL7ElementType.Escape &&
                        // check for MSH/BHS/FHS 1...
                        (previousElement != null &&
                            (Constants.HeaderTypes.Any(h => h.Equals(previousElement.Reference.Segment))) &&
                            previousElement.Reference.Field == 1
                         )
                        )
                    {
                        builder.Append(c);
                    }
                    else
                    {
                        newElement = (newElement == null) ? this : new HL7Element();
                        newElement.AddElement(et, builder.ToString(), null, previousElement);
                        previousElement = newElement;
                        builder.Clear();
                    }
                }
                else
                {
                    builder.Append(c);
                }
                b = data.ReadByte();
            }
            // check if the message is incomplete (we might be chunking it in.)
            if (builder.Length != 0) data.Position = data.Length - builder.Length - 1;
        }

        public HL7Element(string data, char separator, string separators = Constants.Separators, HL7Element owner = null)
        {
            _separator = separator;
            _separators = separators;
            _parentElement = owner;

            if (owner == null || separator == '\r')
            {
                _reference = new HL7Reference();
            }
            else
            {
                _reference = (HL7Reference)owner.Reference.Clone();
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
            foreach (HL7Element hl7Element in this)
            {
                result.Append(hl7Element.ToString());
                result.Append(SeparatorForChildren);
            }
            // remove trailing remnants...
            return result.ToString(0, result.Length - 1);
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
            foreach (char c in Constants.Separators)
            {
                found = (text.IndexOf(c) > 0);
                if (found)
                {
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
                        var searchElement = (criteria.Repetition < 1) ? e : e[criteria.Repetition];
                        if (searchElement.Value != null && searchElement.Value.Equals(criteria.Value))
                        {
                            result = searchElement;
                            break;
                        }
                    }
                    if (result == null) return null;
                }
                else
                {
                    var position = (criteria.Position < 1) ? 0 : criteria.Position;
                    result = (position <= result.Count) ? result[position - fieldOffset] : (position == 1) ? result : null;
                    if (result != null && result.ElementType == HL7ElementType.FieldRepetition)
                    {
                        var repetition = (criteria.Repetition < 1) ? 0 : criteria.Repetition - 1;
                        result = (repetition < result.Count) ? result[repetition] :
                                (result.Count == 0) ? result : null;
                    }
                }
                fieldOffset = 1;
            }
            return (result == this) ? null : result;
        }
    }

}
