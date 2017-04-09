using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestHL7Enumerator
{
    [TestClass]
    public class UnitTestHL7Enumerator
    {
        private const string Example1 =
              @"MSH|^~\&|CERNER||PriorityHealth||||ORU^R01|Q479004375T431430612|P|2.3|" + "\n" +
              @"PID|||001677980||SMITH^CURTIS||19680219|M||||||||||929645156318|123456789|" + "\n" +
              @"PD1||||1234567890^LAST^FIRST^M^^^^^NPI|" + "\n" +
              @"OBR|1|341856649^HNAM_ORDERID|000002006326002362|648088^Basic Metabolic Panel|||20061122151600|||||||||1620^Hooker^Robert^L||||||20061122154733|||F|||||||||||20061122140000|" + "\n" +
              @"OBX|1|NM|GLU^Glucose Lvl|59|mg/dL|65-99^65^99|L|||F|||20061122154733|" + "\n" +
              @"OBX|2|NM|ALT^Alanine Aminotransferase|13|umol/L|2-20^65^1000|N|||F|||20061122154733|" + "\n";


        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestHL7Enumerator_Empty_string_throws_exception ()
        {
            HL7Enumerator.HL7Message msg = new HL7Enumerator.HL7Message("");
        }



        [TestMethod]
        public void TestHL7Enumerator_create_intialises_a_instance()
        {
            HL7Enumerator.HL7Message msg = new HL7Enumerator.HL7Message(Example1);

            Assert.IsFalse(string.IsNullOrEmpty(msg.Segments[0].ToString()));
            Console.WriteLine(msg.Segments[0].ToString());

            Assert.AreEqual("MSH", msg.Segments[0][0].Value);
            Assert.AreEqual("PID", msg.Segments[1][0].Value);
            Assert.AreEqual("PD1", msg.Segments[2][0].Value);
            Assert.AreEqual("OBR", msg.Segments[3][0].Value);
            Assert.AreEqual("OBX", msg.Segments[4][0].Value);
        }

        [TestMethod]
        public void TestHL7Enumerator_implicit_casting_works_in_both_Directions()
        {
            HL7Enumerator.HL7Message msg = Example1;

            Assert.IsFalse(string.IsNullOrEmpty(msg.Segments[0].ToString()));
            Console.WriteLine(msg.Segments[0].ToString());

            Assert.AreEqual("MSH", msg.Segments[0][0].Value);
            Assert.AreEqual("PID", msg.Segments[1][0].Value);
            Assert.AreEqual("PD1", msg.Segments[2][0].Value);
            Assert.AreEqual("OBR", msg.Segments[3][0].Value);
            Assert.AreEqual("OBX", msg.Segments[4][0].Value);

            Assert.AreEqual(Example1, ""+msg);

            HL7Enumerator.HL7Message nullMessage = null;
            Assert.AreEqual("", "" + nullMessage);
        }

        [TestMethod]
        public void TestHL7Enumerator_implicit_cast_intialises_a_instance()
        {
            HL7Enumerator.HL7Message msg = Example1;

            Assert.IsFalse(string.IsNullOrEmpty(msg.Segments[0].ToString()));
            Console.WriteLine(msg.Segments[0].ToString());

            Assert.AreEqual("MSH", msg.Segments[0][0].Value);
            Assert.AreEqual("PID", msg.Segments[1][0].Value);
            Assert.AreEqual("PD1", msg.Segments[2][0].Value);
            Assert.AreEqual("OBR", msg.Segments[3][0].Value);
            Assert.AreEqual("OBX", msg.Segments[4][0].Value);
        }


        [TestMethod]
        public void TestHL7Enumerator_Segment_returns_list_of_correct_segments()
        {
            HL7Enumerator.HL7Message msg = new HL7Enumerator.HL7Message(Example1);
            var pd1Segments = msg.AllSegments("OBX");
            Assert.IsTrue(pd1Segments.Count == 2);
            Console.WriteLine(pd1Segments[0]);
            Console.WriteLine(pd1Segments[1]);
        }

        [TestMethod]
        public void TestHL7Enumerator_Element_returns_correct_field()
        {
            HL7Enumerator.HL7Message msg = new HL7Enumerator.HL7Message(Example1);

            var HL7Field = msg.Element("OBR.2");

            Assert.AreEqual("341856649^HNAM_ORDERID",HL7Field+"");
        }

        [TestMethod]
        public void TestHL7Element_implicit_string_cast_returns_correct_string()
        {
            string segment = @"PID|||001677980||SMITH^CURTIS||19680219|M||||||||||929645156318|123456789|";
            HL7Enumerator.HL7Element element = new HL7Enumerator.HL7Element(segment, '|');
            Assert.IsTrue(element is HL7Enumerator.HL7Element);
            Console.WriteLine(element);
            Assert.AreEqual(segment, element+"");
            // check null element returns empty string;
            HL7Enumerator.HL7Element nullElement = null;
            Assert.AreEqual("", "" + nullElement);
        }

        [TestMethod]
        public void TestHL7Element_Element_Segment_returns_correct_field()
        {
            string segment = @"PID|||001677980||SMITH^CURTIS||19680219|M||||||||||929645156318|123456789|";
            HL7Enumerator.HL7Element element = segment;
            var field = element.Element("'001677980'");
            //note importantly this was a search for a field where the VALUE was PID, not where it was a PID segment...
            Assert.AreEqual("001677980", "" + field); 
        }

    }
}
