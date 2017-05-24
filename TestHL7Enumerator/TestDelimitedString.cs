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
            Assert.AreEqual("DEF", HL7Enumerator.DelimitedString.Field("ABC||DEF|GHI", "||", 2));
        }
        [TestMethod]
        public void TestDelimitedString_last_field_returns_correct_value()
        {
            Assert.AreEqual("GHI",HL7Enumerator.DelimitedString.Field("||GHI", "|", 3));
            Assert.AreEqual("GHI", HL7Enumerator.DelimitedString.Field("||||GHI", "||", 3));
        }

    }
}
