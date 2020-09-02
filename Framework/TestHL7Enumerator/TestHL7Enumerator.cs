using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestHL7Enumerator
{
    [TestClass]
    public class UnitTestHL7Enumerator
    {
        private const string ExampleHeader =
              @"MSH|^~\&|CERNER||PriorityHealth||||ORU^R01|Q479004375T431430612|P|2.3|" + "\r" +
              @"PID|||001677980||SMITH^CURTIS||19680219|M||||||||||929645156318|123456789|" + "\r" +
              @"PD1||||1234567890^LAST^FIRST^M^^^^^NPI|" + "\r";
        private const string ExampleOBX =         
              @"OBR|1|341856649^HNAM_ORDERID|000002006326002362|648088^Basic Metabolic Panel|||20061122151600|||||||||1620^Hooker^Robert^L||||||20061122154733|||F|||||||||||20061122140000|" + "\r" +
              @"OBX|1|NM|GLU^Glucose Lvl|59|mg/dL|65-99^65^99|L|||F|||20061122154733|" + "\r" +
              @"OBX|2|NM|ALT^Alanine Aminotransferase|13|umol/L|2-20^65^1000|N|||F|||20061122154733|" + "\r";

        private const string OBXBase64 =
              @"OBX|1|ED|ClinicalPDFReport1^Clinical PDF Report MR077065T-1^^ClinicalPDFReport1^" +
              @"Clinical PDF Report MR077065T-1||1||MIA^Image^PDF^Base64^JVBERi0xLjQKJeLjz9MKMyAwIG9iaiA8PC9MZW5n" + "\r\n" +
              @"        dGggMTYyMC9GaWx0ZXIvRmxhdGVEZWNvZGU+PnN0" + "\r\n" +
              @"        cmVhbQp4nK2Y227aShSG7/0U6zKVEpijD0j7whiT" + "\r\n" +
              @"        TLexqe0kjdpe0MRJqAi0QFr17ff4bGMaD9V2FTE1" + "\r\n" +
              @"        nm/9s04z5oeGwLAIvMhPBCtNx6T6TO+vtGftVltr" + "\r\n" +
              @"        GH5pBN7Lp75pGMFM+/QFwYP2I5uGYPukMYTBYIYk" + "\r\n" +
              @"        MUSz0SofEZaNcHOUffusPVb3XzRulU/kI1LNIq35" + "\r\n" +
              @"        1azMVvZsRstHvLLAO7a4hapZpJpFCgvZt/WomkXk" + "\r\n" +
              @"        yvNFhpcaMYAbuiTocmXpaKVF2U3GypssFZmPUlj2||||||F" + "\r";

        private const string OBXBase64Escaped =
              @"OBX|1|ED|ClinicalPDFReport1^Clinical PDF Report MR077065T-1^^ClinicalPDFReport1^" +
              @"Clinical PDF Report MR077065T-1||1||MIA^Image^PDF^Base64^JVBERi0xLjQKJeLjz9MKMyAwIG9iaiA8PC9MZW5n\X0D\\X0A\" +
              @"        dGggMTYyMC9GaWx0ZXIvRmxhdGVEZWNvZGU+PnN0\X0D\\X0A\" +
              @"        cmVhbQp4nK2Y227aShSG7/0U6zKVEpijD0j7whiT\X0D\\X0A\" +
              @"        TLexqe0kjdpe0MRJqAi0QFr17ff4bGMaD9V2FTE1\X0D\\X0A\" +
              @"        nm/9s04z5oeGwLAIvMhPBCtNx6T6TO+vtGftVltr\X0D\\X0A\" +
              @"        GH5pBN7Lp75pGMFM+/QFwYP2I5uGYPukMYTBYIYk\X0D\\X0A\" +
              @"        MUSz0SofEZaNcHOUffusPVb3XzRulU/kI1LNIq35\X0D\\X0A\" +
              @"        1azMVvZsRstHvLLAO7a4hapZpJpFCgvZt/WomkXk\X0D\\X0A\" +
              @"        yvNFhpcaMYAbuiTocmXpaKVF2U3GypssFZmPUlj2||||||F" + "\r";

        private const string Example1 = ExampleHeader + ExampleOBX;


        [TestMethod]
        public void TestHL7Enumerator_EscapeOBXCRLF_Returns_Escaped_Values()
        {
            Assert.AreEqual(Example1 + OBXBase64Escaped,
                HL7Enumerator.HL7Message.EscapeOBXCRLF(Example1 + OBXBase64));
            Assert.AreEqual(Example1 + OBXBase64Escaped + OBXBase64Escaped + ExampleOBX,
                HL7Enumerator.HL7Message.EscapeOBXCRLF(Example1 + OBXBase64 + OBXBase64 + ExampleOBX ));
        }

        [TestMethod]
        public void TestHL7Enumerator_EscapeOBXCRLF_CR_Only_Returns_Escaped_Values()
        {
            Assert.AreEqual(Example1 + OBXBase64Escaped.Replace("\r\n", "\r"),
                HL7Enumerator.HL7Message.EscapeOBXCRLF((Example1 + OBXBase64)).Replace("\r\n", "\r"));
            Assert.AreEqual((Example1 + OBXBase64Escaped + OBXBase64Escaped + ExampleOBX).Replace("\r\n", "\r"),
                HL7Enumerator.HL7Message.EscapeOBXCRLF((Example1 + OBXBase64 + OBXBase64 + ExampleOBX)).Replace("\r\n", "\r"));
        }

        [TestMethod]
        public void TestHL7Enumerator_EscapeOBXCRLF_With_NO_Trailing_CR_Returns_Escaped_Values()
        {
            Assert.AreEqual(Example1 + OBXBase64Escaped.Substring(0,OBXBase64Escaped.Length-1),
                HL7Enumerator.HL7Message.EscapeOBXCRLF(Example1 + OBXBase64.Substring(0,OBXBase64.Length-1)));
            Assert.AreEqual(Example1 + OBXBase64Escaped + OBXBase64Escaped + ExampleOBX.Substring(0, ExampleOBX.Length - 1),
                HL7Enumerator.HL7Message.EscapeOBXCRLF(Example1 + OBXBase64 + OBXBase64 + ExampleOBX.Substring(0, ExampleOBX.Length - 1)));
        }

        [TestMethod]
        public void TestHL7Enumerator_EscapeOBXCRLF_WithoutEscapes_Returns_Unchanged_Values()
        {
            Assert.AreEqual(Example1,
                HL7Enumerator.HL7Message.EscapeOBXCRLF(Example1));
            Assert.AreEqual(Example1 + ExampleOBX,
                HL7Enumerator.HL7Message.EscapeOBXCRLF(Example1 + ExampleOBX));
        }

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
