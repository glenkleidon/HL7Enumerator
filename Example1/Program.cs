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

           /* //Now All Locate the Tests performed
            var OBXTestNames = mesg.AllSegments("OBX").Select(o => o.Element("3.2").ToString());

            Console.WriteLine("Found Tests:");
            foreach (string obx in OBXTestNames) Console.WriteLine(string.Format("  {0}",obx));
            */

            //Compose an Element - need some extension methods to handle this better
            // create "PD1||||1234567890^LAST^FIRST^M^^^^^NPI|
            HL7Element ptId = new HL7Element("1234567890", '^', "^&\\");
            HL7Element ptLastName = new HL7Element("LAST", '|', "^&\\");
            HL7Element ptFirstName   = new HL7Element("FIRST", '|');
            HL7Element ptInitial     = new HL7Element("M", '|');
            HL7Element EmptyComponent= new HL7Element("", '|');
            HL7Element npi           = new HL7Element("NPI", '|');
            HL7Element patient       = new HL7Element(null, '|');
            patient.Add(ptId);
            patient.Add(ptLastName);
            patient.Add(ptFirstName);
            patient.Add(ptInitial);
            patient.Add(EmptyComponent);
            patient.Add(EmptyComponent);
            patient.Add(EmptyComponent);
            patient.Add(EmptyComponent);
            patient.Add(npi);

            HL7Element EmptyField    = new HL7Element("",'|');
            HL7Element pd1 = new HL7Element("PD1", '\n');
            pd1.Add(EmptyField);
            pd1.Add(EmptyField);
            pd1.Add(EmptyField);
            pd1.Add(patient);

            Console.WriteLine(pd1);


            Console.ReadLine();
        }
    }
}
