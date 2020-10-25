using HL7Enumerator;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace TestHL7Enumerator
{
    public class TestLineEnding
    {
        private const string ExampleCRMessage =
          @"MSH|^~\&|CERNER||PriorityHealth||||ORU^R01|Q479004375T431430612|P|2.3|" + "\r" +
          @"PID|||001677980||SMITH^CURTIS||19680219|M||||||||||929645156318|123456789|" + "\r" +
          @"PD1||||1234567890^LAST^FIRST^M^^^^^NPI|" + "\r" +
          @"OBR|1|341856649^HNAM_ORDERID|000002006326002362|648088^Basic Metabolic Panel|||20061122151600|||||||||1620^Hooker^Robert^L||||||20061122154733|||F|||||||||||20061122140000|" + "\r" +
          @"OBX|1|NM|GLU^Glucose Lvl|59|mg/dL|65-99^65^99|L|||F|||20061122154733|" + "\r" +
          @"OBX|2|NM|ALT^Alanine Aminotransferase|13|umol/L|2-20^65^1000|N|||F|||20061122154733|" + "\r";
        private const string OBXBase64 =
              @"OBX|3|ED|ClinicalPDFReport1^Clinical PDF Report MR077065T-1^^ClinicalPDFReport1^" +
              @"Clinical PDF Report MR077065T-1||1||MIA^Image^PDF^Base64^JVBERi0xLjQKJeLjz9MKMyAwIG9iaiA8PC9MZW5n" + "\r\n" +
              @"        dGggMTYyMC9GaWx0ZXIvRmxhdGVEZWNvZGU+PnN0" + "\r\n" +
              @"        cmVhbQp4nK2Y227aShSG7/0U6zKVEpijD0j7whiT" + "\r\n" +
              @"        TLexqe0kjdpe0MRJqAi0QFr17ff4bGMaD9V2FTE1" + "\r\n" +
              @"        nm/9s04z5oeGwLAIvMhPBCtNx6T6TO+vtGftVltr" + "\r\n" +
              @"        GH5pBN7Lp75pGMFM+/QFwYP2I5uGYPukMYTBYIYk" + "\r\n" +
              @"        MUSz0SofEZaNcHOUffusPVb3XzRulU/kI1LNIq35" + "\r\n" +
              @"        1azMVvZsRstHvLLAO7a4hapZpJpFCgvZt/WomkXk" + "\r\n" +
              @"        yvNFhpcaMYAbuiTocmXpaKVF2U3GypssFZmPUlj2||||||F" + "\r";
        private const string ExampleCRMessageWithB64 = ExampleCRMessage + OBXBase64;


        private string ExampleLFMessage;
        private string ExampleCRLFMessage;
        HL7Message CrMsg;
        HL7Message LfMsg;
        HL7Message CrLfMsg;

        public TestLineEnding()
        {
            ExampleLFMessage = ExampleCRMessage.Replace('\r', '\n');
            ExampleCRLFMessage = ExampleCRMessage.Replace("\r", "\r\n");

            CrMsg = ExampleCRMessage;
            LfMsg = ExampleLFMessage;
            CrLfMsg = ExampleCRLFMessage;

        }
        [Fact]
        public void MessageTextIsEquivalent()
        {
            Assert.Equal((string)CrMsg, ((string)LfMsg).Replace('\n','\r'));
            Assert.Equal((string)CrMsg, ((string)CrLfMsg).Replace("\r\n","\r"));
        }
        [Fact]
        public void PD1ReturnedForAnyLineEnding()
        {

            var CrLfPD1 = CrLfMsg.Element("PD1");
            var CrPD1 = CrMsg.Element("PD1");
            var LfPD1 = LfMsg.Element("PD1");

            Assert.Equal(CrPD1.ToString(), CrLfPD1.ToString());
            Assert.Equal(CrPD1.ToString(), LfPD1.ToString());
        }

        [Fact]
        public void CRLineEndingsOBX64ReturnsWithEscapedCRLF()
        {
            HL7Message resultMsg = ExampleCRMessageWithB64;
            var obx3 = resultMsg.Element("OBX[3]");
            string obx3txt = obx3;
            Assert.Contains("Clinical PDF Report MR077065T-1", (string)obx3);
            Assert.Contains(@"\X0D\\X0A\", obx3txt);
        }
        [Fact]
        public void CRLFLineEndingsOBX64ReturnsWithEscapedCRLF()
        {
            var crlfWithB64 = ExampleCRMessage.Replace("\r", "\r\n") + OBXBase64 + '\n';
            HL7Message resultMsg = crlfWithB64;
            var obx3 = resultMsg.Element("OBX[3]");
            string obx3txt = obx3;
            Assert.Contains("Clinical PDF Report MR077065T-1", (string)obx3);
            Assert.Contains(@"\X0D\\X0A\", obx3txt);
        }

    }
}
