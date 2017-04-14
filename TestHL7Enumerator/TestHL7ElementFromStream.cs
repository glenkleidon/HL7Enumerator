using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;

namespace TestHL7Enumerator
{
    [TestClass]
    public class TestHL7ElementFromStream
    {
        [TestMethod]
        public void TestHL7Element_Add_Single_Segment_returns_segment()
        {
            byte[] text = Encoding.UTF8.GetBytes("PID|");
            var sMsg = new MemoryStream(text);
            var msg = new HL7Enumerator.HL7Element(sMsg, '\r');
            Assert.Equals("PID", msg.Reference.Segment);
            Assert.Equals("PID", msg.Value);
        }
    }
}
