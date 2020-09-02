using System;
using Xunit;

namespace TestHL7Enumerator
{

    public class TestDelimitedString
    {
        [Fact]
        public void TestDelimitedString_null_returns_empty()
        {
            Assert.Equal(string.Empty,HL7Enumerator.DelimitedString.Field(null, "|", 1));
        }
        [Fact]
        public void TestDelimitedString_no_Delim_returns_string()
        {
            Assert.Equal("ABC", HL7Enumerator.DelimitedString.Field("ABC", "|", 1));
            Assert.Equal(string.Empty,HL7Enumerator.DelimitedString.Field("ABC", "|", 2));
        }
        [Fact]
        public void TestDelimitedString_field_1_returns_first_field()
        {
            Assert.Equal("ABC", HL7Enumerator.DelimitedString.Field("ABC|DEF", "|", 1));
            Assert.Equal("ABC", HL7Enumerator.DelimitedString.Field("ABC||DEF", "||", 1));
        }
        [Fact]
        public void TestDelimitedString_field_2_returns_Second()
        {
            Assert.Equal("DEF", HL7Enumerator.DelimitedString.Field("ABC|DEF|GHI", "|", 2));
            Assert.Equal("DEF", HL7Enumerator.DelimitedString.Field("ABC||DEF||GHI", "||", 2));
        }
        [Fact]
        public void TestDelimitedString_last_field_returns_correct_value()
        {
            Assert.Equal("GHI",HL7Enumerator.DelimitedString.Field("||GHI", "|", 3));
            Assert.Equal("GHI", HL7Enumerator.DelimitedString.Field("||||GHI", "||", 3));
        }

        [Fact]
        public void TestDelimitedString_non_existant_field_returns_empty()
        {
            Assert.Equal(string.Empty, HL7Enumerator.DelimitedString.Field("||GHI", "|", 4));
            Assert.Equal(string.Empty, HL7Enumerator.DelimitedString.Field("||||GHI", "||", 4));
        }
        [Fact]
        public void TestDelimitedString_boundedBy_null_returns_empty()
        {
            Assert.Equal(string.Empty, HL7Enumerator.DelimitedString.BoundedBy(null, "", "\r\n"));
            Assert.Equal(string.Empty, HL7Enumerator.DelimitedString.BoundedBy("", "", "\r\n"));
        }

        [Fact]
        public void TestDelimitedString_boundedBy_not_present_returns_empty()
        {
            Assert.Equal(string.Empty, HL7Enumerator.DelimitedString.BoundedBy("Abcdef", "X", "z"));
            Assert.Equal(string.Empty, HL7Enumerator.DelimitedString.BoundedBy("fasdfasdfcvvwwevw", "\"", "\""));
        }
        [Fact]
        public void TestDelimitedString_boundedBy_common_delimiters_returns_value()
        {
            Assert.Equal("de", HL7Enumerator.DelimitedString.BoundedBy("Abcdef", "c", "f"));
            Assert.Equal("ABCDEF", HL7Enumerator.DelimitedString.BoundedBy("1 \"ABCDEF\"\r\n", "\"", "\""));
            Assert.Equal("ABCDEF", HL7Enumerator.DelimitedString.BoundedBy(",\"ABCDEF\",", "\"", "\""));
            Assert.Equal("123", HL7Enumerator.DelimitedString.BoundedBy(",\"AB[123]CDEF\",[456]\r\n", "[", "]"));
        }
        [Fact]
        public void TestDelimitedString_boundedBy_occurrence_returns_value()
        {
            Assert.Equal("..", HL7Enumerator.DelimitedString.BoundedBy("Abcdefc..f", "c", "f",2));
            Assert.Equal("ghijkl", HL7Enumerator.DelimitedString.BoundedBy("1 \"ABCDEF\"\r\n \"ghijkl\"", "\"", "\"",3));
            Assert.Equal("", HL7Enumerator.DelimitedString.BoundedBy(",\"ABCDEF\",", "\"", "\"",2));
        }
        [Fact]
        public void TestDelimitedString_boundedBy_edge_cases_returns_value()
        {
            Assert.Equal("bcde", HL7Enumerator.DelimitedString.BoundedBy("Abcdef", "A", "f"));
            Assert.Equal("ABCDEF", HL7Enumerator.DelimitedString.BoundedBy("\"ABCDEF\"", "\"", "\""));
            Assert.Equal("ABCDEF", HL7Enumerator.DelimitedString.BoundedBy("'ABCDEF'", "'","'"));
            Assert.Equal("123", HL7Enumerator.DelimitedString.BoundedBy("(123)", "(", ")"));
        }

    }
}
