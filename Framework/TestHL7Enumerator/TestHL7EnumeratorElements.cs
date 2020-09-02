using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestHL7Enumerator
{
    [TestClass]
    public class TestHL7EnumeratorElements
    {
        private const string MSH = @"MSH|^~\&|CERNER||PriorityHealth||||ORU^R01|Q479004375T431430612|P|2.3|" ;
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
        public void TestHL7Enumerator_Element_Segment_Text_returns_Correct_segment()
        {
            HL7Enumerator.HL7Message msg = Example1;
            string segment = msg.Element("MSH");
            Assert.AreEqual(MSH, segment);

            string obx2 = msg.Element("OBX[2]");
            Assert.AreEqual(OBX2, obx2);
        }

        [TestMethod]
        public void TestHL7Enumerator_Element_Field_Text_returns_Correct_Field()
        {
            HL7Enumerator.HL7Message msg = Example1;
            string PIDfield = msg.Element("PID.5");
            Assert.AreEqual("SMITH^CURTIS", PIDfield);

            string OBX2field = msg.Element("OBX[2]/2");
            Assert.AreEqual("NM", OBX2field);
        }

        [TestMethod]
        public void TestHL7Enumerator_MSH_Element_Field_Text_returns_Correct_Field()
        {
            HL7Enumerator.HL7Message msg = Example1;
            string PIDfield = msg.Element("MSH.12");
            Assert.AreEqual("2.3", PIDfield);

            string OBX2field = msg.Element("MSH.3");
            Assert.AreEqual("CERNER", OBX2field);
        }

        [TestMethod]
        public void TestHL7Enumerator_Element_Field_Text_returns_Correct_Component()
        {
            HL7Enumerator.HL7Message msg = Example1;
            string PIDfield = msg.Element("PID/5/1");
            Assert.AreEqual("SMITH", PIDfield);

            string OBX2field = msg.Element("OBX[2].3.2");
            Assert.AreEqual("Alanine Aminotransferase", OBX2field);
        }

        [TestMethod]
        public void TestHL7Enumerator_MSH_Element_Field_Text_returns_Correct_Component()
        {
            HL7Enumerator.HL7Message msg = Example1;
            string PIDfield = msg.Element("MSH.12.2");
            Assert.AreEqual(string.Empty, PIDfield);

            string OBX2field = msg.Element("MSH.3.1");
            Assert.AreEqual("CERNER", OBX2field);
        }

        [TestMethod]
        public void TestHL7Enumerator_Element_Field_Text_returns_Correct_subComponent()
        {
            HL7Enumerator.HL7Message msg = Example2;
            Assert.AreEqual(Example2, "" + msg);
            string field = msg.Element("MSH.12.2.3");
            Assert.AreEqual("ISO", field);

        }

        [TestMethod]
        public void TestHL7Enumerator_Element_Repeating_Field_Text_returns_Correct_Component()
        {
            HL7Enumerator.HL7Message msg = Example2;
            Assert.AreEqual(Example2, "" + msg);
            string field = msg.Element("OBR[1].16[2]");
            Assert.AreEqual("1624^Smith^Bill^R", field);

            string PIDfield = msg.Element("PID.11[2].3");
            Assert.AreEqual("WODEN", PIDfield);

            string MSHfield = msg.Element("MSH/18[2]");
            Assert.AreEqual("8859/1", MSHfield);

            string EmptyMSHfield = msg.Element("MSH/18[3]");
            Assert.AreEqual(string.Empty, EmptyMSHfield);
        }

        [TestMethod]
        public void TestSearchCriteria_Element_with_Escape_returns_correct_field()
        {
            HL7Enumerator.HL7Message msg = Example3;
            Assert.AreEqual(PIDB, "" + msg.Element("PID"));

            Assert.AreEqual(@"CRN 1st \& 2nd Test Street", "" + msg.Element("PID.11.1"));
        }

        [TestMethod]
        public void TestSearchCriteria_MSH_Element_with_Escape_returns_correct_field()
        {
            HL7Enumerator.HL7Message msg = Example3;
            Assert.AreEqual(MSHB, "" + msg.Element("MSH"));

            Assert.AreEqual(@"Section1\\3\&4", "" + msg.Element("MSH.4"));
        }



    }
}
