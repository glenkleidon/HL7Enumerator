using System;
using Xunit;

namespace TestHL7Enumerator
{
    
    public class TestSearchCriteria
    {
        [Fact]
        public void TestSearchCriteria_faulty_string_throws_argumentException()
        {
            Assert.Throws<ArgumentException> ( () =>
            {
                var searchCriteria = new HL7Enumerator.SearchCriteria("34A2");
            }
            );
        }

        [Fact]
        public void TestSearchCriteria_Parse_returns_an_instance() 
        {
            var searchCriteria = new HL7Enumerator.SearchCriteria("MSH");
            Assert.Equal("MSH", searchCriteria.Segment);
        }

        [Fact]
        public void TestSearchCriteria_implicit_string_cast_returns_an_instance()
        {
            HL7Enumerator.SearchCriteria searchCriteria = "MSH.3";
            Assert.Equal("MSH", searchCriteria.Segment);
            Assert.Equal(3, searchCriteria.Field.Position);
        }

        [Fact]
        public void TestSearchCriteria_implicit_skip_string_cast_returns_an_instance()
        {
            HL7Enumerator.SearchCriteria searchCriteria = "*.3.2";
            Assert.True(string.IsNullOrEmpty(searchCriteria.Segment));
            Assert.Equal(3, searchCriteria.Field.Position);
            Assert.Equal(2, searchCriteria.Component.Position);
        }

    }
}
