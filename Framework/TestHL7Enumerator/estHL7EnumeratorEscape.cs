using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HL7Enumerator.Extensions;

namespace TestHL7Enumerator
{
    [TestClass]
    public class TestHL7EnumeratorEscape
    {
        private const string ExampleHeader =
               @"MSH|^~\&|CERNER||PriorityHealth||||ORU^R01|Q479004375T431430612|P|2.3|" + "\r" +
               @"PID|||001677980||SMITH^CURTIS||19680219|M||||||||||929645156318|123456789|" + "\r" +
               @"PD1||||1234567890^LAST^FIRST^M^^^^^NPI|" + "\r";
        private const string ExampleOBX =
              @"OBR|1|341856649^HNAM_ORDERID|000002006326002362|648088^Basic Metabolic Panel|||20061122151600|||||||||1620^Hooker^Robert^L||||||20061122154733|||F|||||||||||20061122140000|" + "\r" +
              @"OBX|1|NM|GLU^Glucose Lvl|59|mg/dL|65-99^65^99|L|||F|||20061122154733|" + "\r" +
              @"OBX|2|NM|ALT^Alanine Aminotransferase|13|umol/L|2-20^65^1000|N|||F|||20061122154733|" + "\r"+
              @"OBX|3|NM|TEST^A Test \T\ another \R\|13|umol/L|2-20^65^1000|N|||F|||20061122154733|" + "\r";

        private const string Example1 = ExampleHeader + ExampleOBX;

        [TestMethod]
        public void TestHL7EnumeratorExtensions_Escape_text_returns_Escaped_Result()
        {
            string obx3_3 = @"Testing \ HL7 | Escape ~ chars & "+"\r\nSecond\t Line";
            string escapedText = obx3_3.EscapeText();
            Console.WriteLine(escapedText);
            Assert.AreEqual(
                 @"Testing \E\ HL7 \F\ Escape \R\ chars \T\ \X0D0A\Second\X09\ Line", escapedText);
        }
    }
}
