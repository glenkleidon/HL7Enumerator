using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HL7Enumerator
{
     public class SearchCriteria
    {
        internal SearchCriteriaElement[] elements = new SearchCriteriaElement[4];
        public string Segment
        {
            get
            {
                return elements[0].Value;
            }
            set
            {
                elements[0] = new SearchCriteriaElement(
                      elements[0].Position,
                      elements[0].Repetition,
                      value);
            }
        }
        public int SegmentRepetition
        {
            get
            {
                return elements[0].Repetition;
            }
            set
            {
                elements[0] = new SearchCriteriaElement(
                      elements[0].Position,
                      value,
                      elements[0].Value
                      );
            }
        }

        public SearchCriteriaElement Field { get { return elements[1]; } set { elements[1] = value; } }
        public SearchCriteriaElement Component { get { return elements[2]; } set { elements[2] = value; } }
        public SearchCriteriaElement SubComponent { get { return elements[3]; } set { elements[3] = value; } }

        private static SearchCriteria Parse(string criteria)
        {
            SearchCriteria result = new SearchCriteria();
            if (criteria.Length == 0) return result;

            var searchCriteriaElements = SearchCriteriaElement.Parse(criteria);
            for (int i = 0; i < 4; i++) {
                if (searchCriteriaElements.Length > i)
                {
                    result.elements[i] = searchCriteriaElements[i];
                }
                else
                {
                    result.elements[i].Enabled = false;
                }
            }

            if (result.Segment.Length == 0 && !result.elements[0].Skip) throw new ArgumentException("Segment type or wildcard expected");
            return result;
        }

        public SearchCriteria() { }
        public SearchCriteria(string criteria)
        {
            var newcriteria = Parse(criteria);
            this.Segment = newcriteria.Segment;
            this.SegmentRepetition = newcriteria.SegmentRepetition;
            this.Field = newcriteria.Field;
            this.Component = newcriteria.Component;
            this.SubComponent = newcriteria.SubComponent; 
            this.elements[0].Skip = newcriteria.elements[0].Skip;
        }

        public static implicit operator SearchCriteria(string criteria) { return new SearchCriteria(criteria); }

    }

    public class HL7Reference : ICloneable
    {
        private SearchCriteria reference = new SearchCriteria();
        public string Segment
        {
            get { return reference.Segment; }
            set {
                if (value != null && value.Length == 3) 
                {
                    reference.Segment = value.Substring(0, 3).ToUpper();
                }
                else 
                {
                    reference.Segment = "";
                }
            }
        }
        public int SegmentIndex {
            get { return reference.SegmentRepetition; }
            set { reference.SegmentRepetition = value; }
        }

        public int Field
        {
            get { return reference.Field.Position; }
            set { reference.Field = new SearchCriteriaElement(value, reference.Field.Repetition); }
        }

        public int FieldRepetition {
            get { return reference.Field.Repetition; }
            set { reference.Field = new SearchCriteriaElement(reference.Field.Position, value); }
        }

        public int Component
        {
            get { return reference.Component.Position; }
            set { reference.Component = new SearchCriteriaElement(value, 0); }
        }

        public int SubComponent
        {
            get { return reference.SubComponent.Position; }
            set { reference.SubComponent = new SearchCriteriaElement(value, 0); }
        }
        public HL7Reference() {
            Segment = "";
        }
        public HL7Reference(string referenceString)
        {
            reference = referenceString;
        }

        public static implicit operator HL7Reference(string referenceString) {
            return new HL7Reference(referenceString);
        }

        public static implicit operator string(HL7Reference hl7Reference) {
            return hl7Reference.reference.ToString();
        }

        public object Clone()
        {
            HL7Reference result = new HL7Reference();
            result.Segment = Segment+"";
            result.SegmentIndex = SegmentIndex + 0;
            result.Field = Field + 0;
            result.FieldRepetition = FieldRepetition + 0;
            result.Component = Component + 0;
            result.SubComponent = SubComponent + 0;
            return result;
        }
    }
}
