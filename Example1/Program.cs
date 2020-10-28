using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HL7Enumerator;
using HL7Enumerator.HL7Tables;
using static HL7Enumerator.Types.DataTypes;

namespace Example1
{
    class Program
    {
        static void Main(string[] args)
        {
            
            var msgText = @"MSH|^~\&|CERNER||PriorityHealth||||ORU^R01|Q479004375T431430612|P|2.3|" + "\r" +
              @"PID|||001677980~212323112^^^AUTH^MR^TESTING||SMITH^CURTIS||19680219|M||||||||||929645156318|123456789|" + "\r" +
              @"PD1||||1234567890^LAST^FIRST^M^^^^^NPI|" + "\r" +
              @"OBR|1|341856649^HNAM_ORDERID|000002006326002362|648088^Basic Metabolic Panel|||20061122151600|||||||||1620^Hooker^Robert^L||||||20061122154733|||F|||||||||||20061122140000|" + "\r" +
              @"OBX|1|NM|GLU^Glucose Lvl|59|mg/dL|65-99^65^99|L|||F|||20061122154733|" + "\r" +
              @"OBX|2|NM|ALT^Alanine Aminotransferase|13|umol/L|2-20^65^1000|N|||F|||20061122154733|" + "\r";
            try
            {

                string messageType = HL7Message.ParseOnly(msgText, "MSH.9");
                if (!messageType.Substring(0, 3).Equals("ORU", StringComparison.CurrentCultureIgnoreCase))
                {
                    Console.WriteLine();
                    throw new FormatException(string.Format("Only processing ORU MessageType. Found message type {0}.", messageType));
                }
                else
                {
                    Console.WriteLine("Processing ORU Message " + HL7Message.ParseOnly(msgText, "MSH.10"));
                }

                HL7Message mesg = msgText;
                string SendingSystem = mesg.Element("MSH.3");
                Console.WriteLine(string.Format("Message received from Sending system {0}", SendingSystem));

                Console.WriteLine("\r\nPD1 Segment : " + mesg.Element("PD1"));

                //Now All Locate the Tests performed
                var OBXTestNames = mesg.AllSegments("OBX").Select(o => o.Element("*.3.2").ToString());

                Console.WriteLine("\r\nFound Tests:");
                foreach (string obx in OBXTestNames) Console.WriteLine(string.Format("  {0}", obx));

                //Extracting All IDs as Low Level Elements
                Console.WriteLine("\r\nExtract Ids: (PID-3[])");
                var ids = mesg.Element("PID.3[]");
                Console.WriteLine($" Found {ids.Count} Ids Using Elements (low level)");
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
                // Using a Data Type to extract the element
                var patientIds = ids.AsCXs();
                Console.WriteLine($"Found {patientIds?.Count()} Ids using DataType Assigment");
                foreach (var cx in patientIds)
                {
                    Console.WriteLine($"ID:       {cx?.ID}");
                    Console.WriteLine($"  IDType:   {cx?.IdentifierTypeCode}");
                    Console.WriteLine($"  Authority:{cx?.AssigningAuthority?.NamespaceId?.BestValue}");
                    Console.WriteLine($"  Facility: {cx?.AssigningFacility?.NamespaceId?.BestValue}");
                }



                // Compose a Segment
                // Composed PID: PID||1234567^4^M11^ADT01^MR^University Hospital|1234567^4^M11^ADT01^MR^University Hospital~8003608833357361^^^AUSHIC^NI^
                var patientIdentifiers = new List<CX_CompositeId>()
                {
                    new CX_CompositeId()
                    {
                        ID = "1234567",
                        CheckDigit = "4",
                        CheckDigitScheme = "M11",
                        AssigningAuthority = new HD_HierarchicDesignator() { NamespaceId = "ADT01" },
                        IdentifierTypeCode = "MR",
                        AssigningFacility = new HD_HierarchicDesignator() { NamespaceId = "University Hospital" }
                    },
                    new CX_CompositeId()
                    {
                        ID = "8003608833357361",
                        AssigningAuthority = new HD_HierarchicDesignator() { NamespaceId = "AUSHIC" },
                        IdentifierTypeCode = "NI"
                    }
                };
                
            

                // Compose a PID 
                var pid = new HL7Segment("PID");
                pid.SetField(2, patientIdentifiers.First());
                pid.SetField(3, patientIdentifiers.AsElement());

                Console.WriteLine("\r\nComposed PID: " + pid);
            } catch (Exception e) 
            {
                Console.WriteLine("Unexpected exception : {0}", e.Message );
            }           

            Console.ReadLine();
        }
    }
}
