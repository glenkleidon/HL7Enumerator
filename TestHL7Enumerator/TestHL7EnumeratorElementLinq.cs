using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using HL7Enumerator;

namespace TestHL7Enumerator
{
    [TestClass]
    public class TestHL7EnumeratorElementLinq
    {
        private const string MSH = @"MSH|^~\&|CERNER||PriorityHealth||||ORU^R01|Q479004375T431430612|P|2.3|";
        private const string PID = @"PID|||001677980||SMITH^CURTIS||19680219|M||||||||||929645156318|123456789|";
        private const string PD1 = @"PD1||||1234567890^LAST^FIRST^M^^^^^NPI|";
        private const string OBR = @"OBR|1|341856649^HNAM_ORDERID|000002006326002362|648088^Basic Metabolic Panel|||20061122151600|||||||||1620^Hooker^Robert^L~1624^Smith^Bill^R||||||20061122154733|||F|||||||||||20061122140000|";
        private const string OBX1 = @"OBX|1|NM|GLU^Glucose Lvl|59|mg/dL|65-99^65^99|L|||F|||20061122154733|";
        private const string OBX2 = @"OBX|2|NM|ALT^Alanine Aminotransferase|13|umol/L|2-20^65^1000|N|||F|||20061122154733|";

        private const string MSHA = @"MSH|^~\&|MERIDIAN|Demo Server|||20100202163120+1100||ORU^R01|XX02021630854-1539|P|2.3.1^AUS&&ISO^AS4700.2&&L|||||AUS|ASCII~8859/1";
        private const string PIDA = @"PID|1||||SMITH^Jessica^^^^^L||19700201|F|||1 Test Street^^WODEN^ACT^2606^AUS^C~2 Test Street^^WODEN^ACT^2606^AUS^C";

        private const string Example1 = MSH + "\r" + PID + "\r" + PD1 + "\r" + OBR + "\r" + OBX1 + "\r" + OBX2 + "\r";
        private const string Example2 = MSHA + "\r" + PIDA + "\r" + PD1 + "\r" + OBR + "\r" + OBX1 + "\r" + OBX2 + "\r";

        [TestMethod]
        public void TestHL7Enumerator_Element_Simple_Linq_expression_returns_rows()
        {
            HL7Enumerator.HL7Message mesg = Example1;
            var OBXTestNames = mesg.AllSegments("OBX").Select( o => o.Element("*.3.2").ToString() );
            Console.WriteLine("Found Tests:");
            var i = 0;
            string[] expected = { "Glucose Lvl", "Alanine Aminotransferase" };
            foreach (string obx in OBXTestNames)
            {
                Console.WriteLine(string.Format("  {0}", obx));
                Assert.AreEqual(expected[i++], obx);
            }
        }
    }
}
