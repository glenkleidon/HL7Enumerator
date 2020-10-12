using System;
using Xunit;

namespace TestHL7Enumerator
{
    
    public class TestSearchCriteriaElement
    {
        [Fact]
        public void TestParseElement_faulty_string_throws_argumentException()
        {
            Assert.Throws<ArgumentException>(
                () =>
                {
                    var searchElement = HL7Enumerator.SearchCriteriaElement.ParseElement("34A2");
                }
            );

        }

        [Fact]
        public void TestParseElement_Returns_an_instance()
        {
            var searchElement = HL7Enumerator.SearchCriteriaElement.ParseElement("MSH");
            Assert.Equal("MSH", searchElement.Value);

        }

        [Fact]
        public void TestParseElement_segment_repetition_returns_Segment_and_repetition()
        {
            var searchElement = HL7Enumerator.SearchCriteriaElement.ParseElement("MSH[1]");
            Assert.Equal("MSH", searchElement.Value);
            Assert.Equal(1, searchElement.Repetition);
            Assert.Equal(0, searchElement.Position);
        }

        [Fact]
        public void TestParseElement_segment_repetition_returns_Segment_and_All_Repititions()
        {
            var searchElement = HL7Enumerator.SearchCriteriaElement.ParseElement("MSH[]");
            Assert.Equal("MSH", searchElement.Value);
            Assert.Equal(-1, searchElement.Repetition);
            Assert.Equal(0, searchElement.Position);
        }

        [Fact]
        public void TestParseElement_position_repetition_returns_position_and_repitition()
        {
            var searchElement = HL7Enumerator.SearchCriteriaElement.ParseElement("12[2]");
            Assert.True(string.IsNullOrEmpty(searchElement.Value));
            Assert.Equal(2, searchElement.Repetition);
            Assert.Equal(12, searchElement.Position);

        }

        [Fact]
        public void TestParse_with_multiple_elements_returns_a_multiElement_array()
        {
            var SearchElements = HL7Enumerator.SearchCriteriaElement.Parse("MSH");
            Assert.Single(SearchElements);

            var TwoSearchElements = HL7Enumerator.SearchCriteriaElement.Parse("MSH.3");
            Assert.Equal(2, TwoSearchElements.Length);
            Assert.Equal(3, TwoSearchElements[1].Position);

            var ThreeSearchElements = HL7Enumerator.SearchCriteriaElement.Parse("MSH/1/2[3]");
            Assert.Equal(3, ThreeSearchElements.Length);
            Assert.Equal(1, ThreeSearchElements[1].Position);
            Assert.Equal(2, ThreeSearchElements[2].Position);
            Assert.Equal(3, ThreeSearchElements[2].Repetition);

        }

    }
}
