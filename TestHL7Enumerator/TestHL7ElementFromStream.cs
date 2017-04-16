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
        private const string PD1 = @"PD1||||1234567890^LAST^FIRST^M^^^^^NPI|";
        private const string OBR = @"OBR|1|341856649^HNAM_ORDERID|000002006326002362|648088^Basic Metabolic Panel|||20061122151600|||||||||1620^Hooker^Robert^L~1624^Smith^Bill^R||||||20061122154733|||F|||||||||||20061122140000|";
        private const string OBX1 = @"OBX|1|NM|GLU^Glucose Lvl|59|mg/dL|65-99^65^99|L|||F|||20061122154733|";
        private const string OBX2 = @"OBX|2|NM|ALT^Alanine Aminotransferase|13|umol/L|2-20^65^1000|N|||F|||20061122154733|";

        private const string MSHA = @"MSH|^~\&|MERIDIAN|Demo Server|||20100202163120+1100||ORU^R01|XX02021630854-1539|P|2.3.1^AUS&&ISO^AS4700.2&&L|||||AUS|ASCII~8859/1";
        private const string MSHB = @"MSH|^~\&|MERIDIAN|Section1\\3\&4|||20100202163120+1100||ORU^R01|XX02021630854-1539|P|2.3.1^AUS&&ISO^AS4700.2&&L|||||AUS|ASCII~8859/1";
        private const string PIDA = @"PID|1||||SMITH^Jessica^^^^^L||19700201|F|||1 Test Street^^WODEN^ACT^2606^AUS^C~2 Test Street^^WODEN^ACT^2606^AUS^C";
        private const string PIDB = @"PID|1||||SMITH^Jessica^^^^^L||19700201|F|||CRN 1st \& 2nd Test Street^^WODEN^ACT^2606^AUS^C~2 Test Street^^WODEN^ACT^2606^AUS^C";

        private const string Example1 = MSH + "\r" + PID + "\r" + PD1 + "\r" + OBR + "\r" + OBX1 + "\r" + OBX2 + "\r";
        private const string Example2 = MSHA + "\r" + PIDA + "\r" + PD1 + "\r" + OBR + "\r" + OBX1 + "\r" + OBX2 + "\r";
        private const string Example3 = MSHB + "\r" + PIDB + "\r" + PD1 + "\r" + OBR + "\r" + OBX1 + "\r" + OBX2 + "\r";

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
            Assert.AreEqual(6, segment.Count);
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

        [TestMethod]
        public void TestHL7Element_Segment_Text_returns_Correct_segment()
        {
            byte[] text = Encoding.UTF8.GetBytes(Example1);
            var sMsgElement = new MemoryStream(text);
            var msgElement = new HL7Enumerator.HL7Element(sMsgElement);
            Console.WriteLine(msgElement);

            string segment = msgElement.Element("MSH");
            Assert.AreEqual(MSH, segment);

            string obx2 = msgElement.Element("OBX[2]");
            Assert.AreEqual(OBX2, obx2);
        }

        [TestMethod]
        public void TestHL7Element_Stream_Timing_10000_OBR()
        {
            byte[] text = Encoding.UTF8.GetBytes(OBR);
            var sMsgElement = new MemoryStream(text);
            for (int i = 0; i < 10000; i++)
            {
                var msgElement = new HL7Enumerator.HL7Element(sMsgElement);
                sMsgElement.Position = 0;
            }
        }













    }
}
