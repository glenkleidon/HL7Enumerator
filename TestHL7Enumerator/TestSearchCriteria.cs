using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestHL7Enumerator
{
    [TestClass]
    public class TestSearchCriteria
    {
        [TestMethod]
        [ExpectedExceptionAttribute(typeof(ArgumentException))]
        public void TestSearchCriteria_faulty_string_throws_argumentException()
        {
            var searchCriteria = new HL7Enumerator.SearchCriteria("34A2");
        }

        [TestMethod]
        public void TestSearchCriteria_Parse_returns_an_instance() 
        {
            var searchCriteria = new HL7Enumerator.SearchCriteria("MSH");
            Assert.AreEqual("MSH", searchCriteria.Segment);
        }

        [TestMethod]
        public void TestSearchCriteria_implicit_string_cast_returns_an_instance()
        {
            HL7Enumerator.SearchCriteria searchCriteria = "MSH.3";
            Assert.AreEqual("MSH", searchCriteria.Segment);
            Assert.AreEqual(3, searchCriteria.Field.Position);
        }

        [TestMethod]
        public void TestSearchCriteria_implicit_skip_string_cast_returns_an_instance()
        {
            HL7Enumerator.SearchCriteria searchCriteria = "*.3.2";
            Assert.IsTrue(string.IsNullOrEmpty(searchCriteria.Segment));
            Assert.AreEqual(3, searchCriteria.Field.Position);
            Assert.AreEqual(2, searchCriteria.Component.Position);
        }
    }
}
