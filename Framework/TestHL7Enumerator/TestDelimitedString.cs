using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestHL7Enumerator
{
    [TestClass]
    public class TestDelimitedString
    {
        [TestMethod]
        public void TestDelimitedString_null_returns_empty()
        {
            Assert.AreEqual(string.Empty,HL7Enumerator.DelimitedString.Field(null, "|", 1));
        }
        [TestMethod]
        public void TestDelimitedString_no_Delim_returns_string()
        {
            Assert.AreEqual("ABC", HL7Enumerator.DelimitedString.Field("ABC", "|", 1));
            Assert.AreEqual(string.Empty,HL7Enumerator.DelimitedString.Field("ABC", "|", 2));
        }
        [TestMethod]
        public void TestDelimitedString_field_1_returns_first_field()
        {
            Assert.AreEqual("ABC", HL7Enumerator.DelimitedString.Field("ABC|DEF", "|", 1));
            Assert.AreEqual("ABC", HL7Enumerator.DelimitedString.Field("ABC||DEF", "||", 1));
        }
        [TestMethod]
        public void TestDelimitedString_field_2_returns_Second()
        {
            Assert.AreEqual("DEF", HL7Enumerator.DelimitedString.Field("ABC|DEF|GHI", "|", 2));
            Assert.AreEqual("DEF", HL7Enumerator.DelimitedString.Field("ABC||DEF||GHI", "||", 2));
        }
        [TestMethod]
        public void TestDelimitedString_last_field_returns_correct_value()
        {
            Assert.AreEqual("GHI",HL7Enumerator.DelimitedString.Field("||GHI", "|", 3));
            Assert.AreEqual("GHI", HL7Enumerator.DelimitedString.Field("||||GHI", "||", 3));
        }

        [TestMethod]
        public void TestDelimitedString_non_existant_field_returns_empty()
        {
            Assert.AreEqual(string.Empty, HL7Enumerator.DelimitedString.Field("||GHI", "|", 4));
            Assert.AreEqual(string.Empty, HL7Enumerator.DelimitedString.Field("||||GHI", "||", 4));
        }
        [TestMethod]
        public void TestDelimitedString_boundedBy_null_returns_empty()
        {
            Assert.AreEqual(string.Empty, HL7Enumerator.DelimitedString.BoundedBy(null, "", "\r\n"));
            Assert.AreEqual(string.Empty, HL7Enumerator.DelimitedString.BoundedBy("", "", "\r\n"));
        }

        [TestMethod]
        public void TestDelimitedString_boundedBy_not_present_returns_empty()
        {
            Assert.AreEqual(string.Empty, HL7Enumerator.DelimitedString.BoundedBy("Abcdef", "X", "z"));
            Assert.AreEqual(string.Empty, HL7Enumerator.DelimitedString.BoundedBy("fasdfasdfcvvwwevw", "\"", "\""));
        }
        [TestMethod]
        public void TestDelimitedString_boundedBy_common_delimiters_returns_value()
        {
            Assert.AreEqual("de", HL7Enumerator.DelimitedString.BoundedBy("Abcdef", "c", "f"));
            Assert.AreEqual("ABCDEF", HL7Enumerator.DelimitedString.BoundedBy("1 \"ABCDEF\"\r\n", "\"", "\""));
            Assert.AreEqual("ABCDEF", HL7Enumerator.DelimitedString.BoundedBy(",\"ABCDEF\",", "\"", "\""));
            Assert.AreEqual("123", HL7Enumerator.DelimitedString.BoundedBy(",\"AB[123]CDEF\",[456]\r\n", "[", "]"));
        }
        [TestMethod]
        public void TestDelimitedString_boundedBy_occurrence_returns_value()
        {
            Assert.AreEqual("..", HL7Enumerator.DelimitedString.BoundedBy("Abcdefc..f", "c", "f",2));
            Assert.AreEqual("ghijkl", HL7Enumerator.DelimitedString.BoundedBy("1 \"ABCDEF\"\r\n \"ghijkl\"", "\"", "\"",3));
            Assert.AreEqual("", HL7Enumerator.DelimitedString.BoundedBy(",\"ABCDEF\",", "\"", "\"",2));
        }
        [TestMethod]
        public void TestDelimitedString_boundedBy_edge_cases_returns_value()
        {
            Assert.AreEqual("bcde", HL7Enumerator.DelimitedString.BoundedBy("Abcdef", "A", "f"));
            Assert.AreEqual("ABCDEF", HL7Enumerator.DelimitedString.BoundedBy("\"ABCDEF\"", "\"", "\""));
            Assert.AreEqual("ABCDEF", HL7Enumerator.DelimitedString.BoundedBy("'ABCDEF'", "'","'"));
            Assert.AreEqual("123", HL7Enumerator.DelimitedString.BoundedBy("(123)", "(", ")"));
        }

    }
}
