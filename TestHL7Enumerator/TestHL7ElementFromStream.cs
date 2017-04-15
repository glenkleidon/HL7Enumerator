using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;

namespace TestHL7Enumerator
{
    [TestClass]
    public class TestHL7ElementFromStream
    {
        private const string MSH = @"MSH|^~\&|CERNER||PriorityHealth||||ORU^R01|Q479004375T431430612|P|2.3|";
        private const string PID = @"PID|||001677980||SMITH^CURTIS||19680219|M||||||||||929645156318|123456789|";

        [TestMethod]
        public void TestHL7Element_ChildSeparatorFromElementType_returns_correct_Values()
        {
            var element = new HL7Enumerator.HL7Element();
            Assert.AreEqual('\0', element.ChildSeparatorFromElementType(HL7Enumerator.HL7ElementType.Batch));
            Assert.AreEqual('\0', element.ChildSeparatorFromElementType(HL7Enumerator.HL7ElementType.Message));
            Assert.AreEqual('|', element.ChildSeparatorFromElementType(HL7Enumerator.HL7ElementType.Segment));
            Assert.AreEqual('^', element.ChildSeparatorFromElementType(HL7Enumerator.HL7ElementType.Field));
            Assert.AreEqual('^', element.ChildSeparatorFromElementType(HL7Enumerator.HL7ElementType.FieldRepetition));
            Assert.AreEqual('&', element.ChildSeparatorFromElementType(HL7Enumerator.HL7ElementType.Component));
            Assert.AreEqual('\0', element.ChildSeparatorFromElementType(HL7Enumerator.HL7ElementType.SubComponent),"Subcomponent");
        }

        [TestMethod]
        public void TestHL7Element_Add_Single_Segment_Field_returns_segment()
        {
            byte[] text = Encoding.UTF8.GetBytes("PID|");
            var sMsgElement = new MemoryStream(text);
            var msgElement = new HL7Enumerator.HL7Element(sMsgElement);
            Assert.AreEqual("PID", msgElement.Value);
            Assert.AreEqual("PID", msgElement.Reference.Segment);
            var segment = msgElement.AscendTo(HL7Enumerator.HL7ElementType.Segment);
            Assert.IsNotNull(segment);
            Assert.AreEqual(HL7Enumerator.HL7ElementType.Segment, segment.ElementType);
            Assert.AreEqual("PID", segment.Value);
            Assert.AreEqual("PID", segment.Reference.Segment);
            Console.WriteLine(segment);
        }

        [TestMethod]
        public void TestHL7Element_Ascend_to_Message_returns_Message_Type()
        {
            byte[] text = Encoding.UTF8.GetBytes("PID|");
            var sMsgElement = new MemoryStream(text);
            var msgElement = new HL7Enumerator.HL7Element(sMsgElement);
            var msg = msgElement.AscendTo(HL7Enumerator.HL7ElementType.Message);
            Assert.IsNotNull(msg);
            Assert.AreEqual(HL7Enumerator.HL7ElementType.Message, msg.ElementType);
        }

        [TestMethod]
        public void TestHL7Element_Add_multiple_Fields_returns_segment_with_correct_Fields()
        {
            byte[] text = Encoding.UTF8.GetBytes(@"PID|||001677980||SMITH^CURTIS|");
            var sMsgElement = new MemoryStream(text);
            var msgElement = new HL7Enumerator.HL7Element(sMsgElement);
            Assert.AreEqual("PID", msgElement.Value);
            Assert.AreEqual("PID", msgElement.Reference.Segment);
            var segment = msgElement.AscendTo(HL7Enumerator.HL7ElementType.Segment);
            Assert.IsNotNull(segment);
            Console.WriteLine(segment);
            Assert.AreEqual(5, segment.Count);
        }

        [TestMethod]
        public void TestHL7Element_Add_Segment_matches_Original_Text()
        {
            byte[] text = Encoding.UTF8.GetBytes(PID);
            var sMsgElement = new MemoryStream(text);
            var msgElement = new HL7Enumerator.HL7Element(sMsgElement);
            Assert.AreEqual("PID", msgElement.Value);
            Assert.AreEqual("PID", msgElement.Reference.Segment);
            var segment = msgElement.AscendTo(HL7Enumerator.HL7ElementType.Segment);
            Assert.IsNotNull(segment);
            Console.WriteLine(segment);
            Assert.AreEqual(PID.Substring(0,PID.Length-1), segment+"");
        }

    }
}
