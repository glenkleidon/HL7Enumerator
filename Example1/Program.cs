using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HL7Enumerator;

namespace Example1
{
    class Program
    {
        static void Main(string[] args)
        {
            HL7Message mesg = @"MSH|^~\&|CERNER||PriorityHealth||||ORU^R01|Q479004375T431430612|P|2.3|" + "\n" +
              @"PID|||001677980||SMITH^CURTIS||19680219|M||||||||||929645156318|123456789|" + "\n" +
              @"PD1||||1234567890^LAST^FIRST^M^^^^^NPI|" + "\n" +
              @"OBR|1|341856649^HNAM_ORDERID|000002006326002362|648088^Basic Metabolic Panel|||20061122151600|||||||||1620^Hooker^Robert^L||||||20061122154733|||F|||||||||||20061122140000|" + "\n" +
              @"OBX|1|NM|GLU^Glucose Lvl|59|mg/dL|65-99^65^99|L|||F|||20061122154733|" + "\n" +
              @"OBX|2|NM|ALT^Alanine Aminotransferase|13|umol/L|2-20^65^1000|N|||F|||20061122154733|" + "\n";

            string SendingSystem = mesg.Element("MSH.3");
            Console.WriteLine(string.Format("Message received from Sending system {0}", SendingSystem));

            Console.WriteLine("PD1 Segment: " + mesg.Element("PD1"));

            //Now All Locate the Tests performed
            var OBXTestNames = mesg.AllSegments("OBX").Select(o => o.Element("3.2").ToString());

            Console.WriteLine("Found Tests:");
            foreach (string obx in OBXTestNames) Console.WriteLine(string.Format("  {0}",obx));

            Console.ReadLine();
        }
    }
}
