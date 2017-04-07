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
              @"OBX|1|NM|GLU^Glucose Lvl|59|mg/dL|65-99^65^99|L|||F|||20061122154733|" + "\n";

        [TestMethod]
        public void TestHL7Enumerator_create_intialises_a_instance()
        {
            HL7Enumerator.HL7Message msg = new HL7Enumerator.HL7Message(Example1);
            Assert.IsFalse(string.IsNullOrEmpty(msg.Segments[0].ToString()));
            Console.WriteLine(msg.Segments[0].ToString());
        }
    }
}
