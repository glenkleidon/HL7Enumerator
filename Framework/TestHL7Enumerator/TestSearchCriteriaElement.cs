using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestHL7Enumerator
{
    [TestClass]
    public class TestSearchCriteriaElement
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestParseElement_faulty_string_throws_argumentException()
        {
            var searchElement = HL7Enumerator.SearchCriteriaElement.ParseElement("34A2");
        }

        [TestMethod]
        public void TestParseElement_Returns_an_instance()
        {
            var searchElement = HL7Enumerator.SearchCriteriaElement.ParseElement("MSH");
            Assert.AreEqual("MSH", searchElement.Value);

        }

        [TestMethod]
        public void TestParseElement_segment_repetition_returns_Segment_and_repetition()
        {
            var searchElement = HL7Enumerator.SearchCriteriaElement.ParseElement("MSH[1]");
            Assert.AreEqual("MSH", searchElement.Value);
            Assert.AreEqual(1, searchElement.Repetition);
            Assert.AreEqual(0, searchElement.Position);

        }

        [TestMethod]
        public void TestParseElement_position_repetition_returns_position_and_repitition()
        {
            var searchElement = HL7Enumerator.SearchCriteriaElement.ParseElement("12[2]");
            Assert.IsTrue(string.IsNullOrEmpty(searchElement.Value));
            Assert.AreEqual(2, searchElement.Repetition);
            Assert.AreEqual(12, searchElement.Position);

        }

        [TestMethod]
        public void TestParse_with_multiple_elements_returns_a_multiElement_array()
        {
            var SearchElements = HL7Enumerator.SearchCriteriaElement.Parse("MSH");
            Assert.AreEqual(1, SearchElements.Length);

            var TwoSearchElements = HL7Enumerator.SearchCriteriaElement.Parse("MSH.3");
            Assert.AreEqual(2, TwoSearchElements.Length);
            Assert.AreEqual(3, TwoSearchElements[1].Position);

            var ThreeSearchElements = HL7Enumerator.SearchCriteriaElement.Parse("MSH/1/2[3]");
            Assert.AreEqual(3, ThreeSearchElements.Length);
            Assert.AreEqual(1, ThreeSearchElements[1].Position);
            Assert.AreEqual(2, ThreeSearchElements[2].Position);
            Assert.AreEqual(3, ThreeSearchElements[2].Repetition);

        }

    }
}
