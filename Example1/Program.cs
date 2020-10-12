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
            
            var msgText = @"MSH|^~\&|CERNER||PriorityHealth||||ORU^R01|Q479004375T431430612|P|2.3|" + "\r" +
              @"PID|||001677980~212323112^^^TESTING^MR^AUTH||SMITH^CURTIS||19680219|M||||||||||929645156318|123456789|" + "\r" +
              @"PD1||||1234567890^LAST^FIRST^M^^^^^NPI|" + "\r" +
              @"OBR|1|341856649^HNAM_ORDERID|000002006326002362|648088^Basic Metabolic Panel|||20061122151600|||||||||1620^Hooker^Robert^L||||||20061122154733|||F|||||||||||20061122140000|" + "\r" +
              @"OBX|1|NM|GLU^Glucose Lvl|59|mg/dL|65-99^65^99|L|||F|||20061122154733|" + "\r" +
              @"OBX|2|NM|ALT^Alanine Aminotransferase|13|umol/L|2-20^65^1000|N|||F|||20061122154733|" + "\r";
            try {

                string messageType = HL7Message.ParseOnly(msgText, "MSH.9");
                if (!messageType.Substring(0, 3).Equals("ORU", StringComparison.CurrentCultureIgnoreCase))
                {
                    Console.WriteLine();
                    throw new FormatException(string.Format("Only processing ORU MessageType. Found message type {0}.",messageType));
                }
                else 
                {
                    Console.WriteLine("Processing ORU Message " + HL7Message.ParseOnly(msgText,"MSH.10"));
                }

                HL7Message mesg = msgText;
                string SendingSystem = mesg.Element("MSH.3");
                Console.WriteLine(string.Format("Message received from Sending system {0}", SendingSystem));

                Console.WriteLine("\r\nPD1 Segment : " + mesg.Element("PD1"));

                 //Now All Locate the Tests performed
                 var OBXTestNames = mesg.AllSegments("OBX").Select(o => o.Element("*.3.2").ToString());

                 Console.WriteLine("\r\nFound Tests:");
                 foreach (string obx in OBXTestNames) Console.WriteLine(string.Format("  {0}",obx));

                //Extracting All IDs as 
                Console.WriteLine("\r\nExtract Ids: (PID-3[])");
                var ids = mesg.Element("PID.3[]");
                foreach (var id in ids)
                {
                    if (id.Count > 0)
                    {
                        Console.WriteLine($"ID: {id[0]}");
                    } 
                    else
                    {
                        Console.WriteLine($"ID: {id.ToString()}");
                    }
                    if (id.Count > 4) Console.WriteLine($"  IDType:{id[4]}");
                    if (id.Count > 3) Console.WriteLine($"  Issuer:{id[3]}");
                }


             
                //Compose a Segment - create extension methods to this more cleanly
                // create "PD1||||1234567890^LAST^FIRST^M^^^^^NPI|

                HL7Element pd1 = new HL7Element(null, '|');
                pd1.Add(new HL7Element("PD1", '~'));
                pd1.Add(new HL7Element("", '~'));
                pd1.Add(new HL7Element("", '~'));
                pd1.Add(new HL7Element("", '~'));
                HL7Element patientField = new HL7Element(null, '~');
                pd1.Add(patientField);
                HL7Element patient = new HL7Element(null, '^');
                patientField.Add(patient);
                patient.Add(new HL7Element("1234567890", '&' ));
                patient.Add(new HL7Element("LAST", '&'));
                patient.Add(new HL7Element("FIRST", '&'));
                patient.Add(new HL7Element("M", '&'));
                patient.Add(new HL7Element("", '&'));
                patient.Add(new HL7Element("", '&'));
                patient.Add(new HL7Element("", '&'));
                patient.Add(new HL7Element("", '&'));
                patient.Add(new HL7Element("NPI", '&'));

                Console.WriteLine("\r\nComposed PD1: " + pd1);
            } catch (Exception e) 
            {
                Console.WriteLine("Unexpected exception : {0}", e.Message );
            }           

            Console.ReadLine();
        }
    }
}
