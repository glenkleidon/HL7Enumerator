﻿using System;
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
                      elements[0].Repitition,
                      value);
            }
        }
        public int SegmentRepitition
        {
            get
            {
                return elements[0].Repitition;
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
        public SearchCriteriaElement Subcomponent { get { return elements[3]; } set { elements[3] = value; } }

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

            if (result.Segment.Length == 0) throw new ArgumentException("Segment Type Expected");
            return result;
        }

        public SearchCriteria() { }
        public SearchCriteria(string criteria)
        {
            var newcriteria = Parse(criteria);
            this.Segment = newcriteria.Segment;
            this.SegmentRepitition = newcriteria.SegmentRepitition;
            this.Field = newcriteria.Field;
            this.Component = newcriteria.Component;
            this.Subcomponent = newcriteria.Subcomponent;
        }

        public static implicit operator SearchCriteria(string criteria) => new SearchCriteria(criteria);

    }
}
