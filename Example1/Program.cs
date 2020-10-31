using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HL7Enumerator;
using HL7Enumerator.HL7Tables;
using HL7Enumerator.HL7Tables.Interfaces;
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
                Console.WriteLine("How to use the HL7 Enumerator for Decoding HL7 Messages");
                Console.WriteLine("-------------------------------------------------------\r\n");

                // Dont want to process the whole message? Use the Parse Only Method to check fields before processing
                // Here is an example:
                UseParseOnlyToConfirmWeCanProcessTheMessage(msgText);
                Console.WriteLine("\r\nProcessing ORU Message " + HL7Message.ParseOnly(msgText, "MSH.10"));


                // Process the HL7 message by Creating a HL7 Message Object
                // NOTE: this will always succeed - an exception should never be thrown, so there is no need
                // to test for it.
                HL7Message mesg = msgText;

                // yes, it is that simple, but the method below implements some other syntax versions 
                AlternateMethodsForCreatingAHL7MessageObject(msgText);

                // Extract a Single field - say for logging purposes
                // Use the Element property to extract ANY specific field/subfield OR Group of fields/subFields
                DisplayExtractingAnElementMessage();
                Console.WriteLine($"Message received from Sending system {mesg.Element("MSH.3")}");

                // Extract a Whole Segment
                Console.Write("\r\nOR a whole PD1 Segment :\r\n  ");
                Console.WriteLine(mesg.Element("PD1"));

                // Use the ALLSegments property with LINQ to extract and manipulate any part/subpart of a message
                // as a Low Level HL7Element
                DisplayUsingLinq();
                Console.WriteLine("Eg Test Names: ");
                var OBXTestNames = mesg.AllSegments("OBX").Select(o => o.Element("*.3.2"));
                // Note the use of the "*" wildcard - ovoids needing to specify the Segment level.

                // Use Implicit String Casting to Output any Element as a string.
                Console.WriteLine($"\r\nThe message contains {OBXTestNames.Count()} Test Results:");
                foreach (string obx in OBXTestNames) Console.WriteLine($"  {obx}");

                // Use LINQ on ANY element at any level to iterate over sub elements
                Console.Write("\r\nYou can also use LINQ on any element to get information\r\nEg:  ");
                var NonEmptyComponentsInPD1 =
                    mesg.Element("PD1.4").Where(f => !string.IsNullOrWhiteSpace(f));
                Console.WriteLine($"PID.3 has {(mesg.Element("PD1.4").Count)} components, and {NonEmptyComponentsInPD1.Count()} have data");
                foreach (var component in NonEmptyComponentsInPD1) Console.WriteLine($"  {component.ToString()}");

                // Use self defined Constants for field definitions to make your code cleaner.
                DisplayConstantsMessage();
                const string PatientFamilyName = "PID.5.1";
                const string PatientGivenName = "PID.5.2";
                Console.WriteLine($"Patients Name is : {mesg.Element(PatientFamilyName)}, {mesg.Element(PatientGivenName)}.");

                DisplayDataTypesMessage();
                // Use the HL7 Types Namespace 






                // Use the Data Type 

                // Use the DataType Extensions for Strict Typing of known Data Types.




                // Use The DataType Extensions Get Intuitive Extraction of elements.






                //Extracting IDs from PID
                Console.WriteLine("\r\nExtract Ids: (PID-3[])");
                var ids = mesg.Element("PID.3[]");

                // we can now use this potentially replicating field either with DataTypes OR at a low level

                // Using a CX Data Type to Interpret the data.

                var patientIds = ids.AsCXs();
                // we know from the spec, that PID.3 is a replicating field of CX 's.  The included AsCXs method is  
                // a simple extension method returning a CX_CompositeId type (supporting the IHL7Type interface).
                // If the include CX datatype is not suitable for your purpose, it is very easy to make your own.  
                // Just Implement the HL7Type interface using the low level methods.
                //
                // NOTE: if you ask for a simple CX for a Replicating field, you will get a DEBUG exception indicating 
                // that you should use an Enumerable type for this field

                Console.WriteLine($"Found {patientIds?.Count()} Ids using DataType Assigment");
                foreach (var cx in patientIds)
                {
                    // remember that certain elements in a CX MAY be null.
                    Console.WriteLine($"ID:       {cx?.ID}");
                    Console.WriteLine($"  IDType:   {cx?.IdentifierTypeCode}");
                    Console.WriteLine($"  Authority:{cx?.AssigningAuthority?.NamespaceId?.BestValue}");
                    Console.WriteLine($"  Facility: {cx?.AssigningFacility?.NamespaceId?.BestValue}");
                }

                /// Alternatively - low level access is available 
                Console.WriteLine($" Found {ids.Count} Ids Using Elements (low level)");
                foreach (var id in ids)
                {
                    if (id.Count > 0)
                    {
                        Console.WriteLine($"ID: {id[0]}");
                    }
                    else
                    {
                        Console.WriteLine($"ID: {id}"); // implict string casting
                    }
                    if (id.Count > 4) Console.WriteLine($"  IDType:{id[4]}");
                    if (id.Count > 3) Console.WriteLine($"  Issuer:{id[3]}");
                }

                // Manullay Create a Data Table into In- Memory Provider
                Console.WriteLine("Manually Create Tables to validate the properties");
                var tables = ManuallyCreateTablesIntoAInMemoryProvider();
                // Attach manually created Table to CX we just read. 




                // Automatically Attach suitable tables to the Data type as we read Assign it.
                // Create the Required Tables.
                var validatedPatientIds = ids.AsCX();


                // Compose a Segment
                Console.WriteLine("\r\n Composing a Segment using DataTypes");
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
            }
            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception : {0}", e.Message);
            }

            Console.ReadLine();
        }

        private static void DisplayDataTypesMessage()
        {
            Console.WriteLine("\r\nData Type Extensions");
            Console.WriteLine("--------------------");
            Console.WriteLine("The HL7Types Namespace is a set of helpers and extenions to automatically");
            Console.WriteLine("assign defined HL7 Types (eg HD, CX, ID, IS) to HL7Elements");
            Console.WriteLine("There are extensions for intuitive field and component value extraction ");
            Console.WriteLine("with implicit Numeric, DateTime and Coded Element conversion");
            Console.WriteLine("greatly reducing the complexity of the code you need to write.");
        }

        private static void DisplayUsingLinq()
        {
            Console.WriteLine("\r\nUsing LINQ");
            Console.WriteLine("----------");

            Console.WriteLine("You can use LINQ and the HL7Message.AllSegments Method to ");
            Console.WriteLine("extract Components over multple rows of a message.");
            Console.WriteLine("You can also use Implicit string casting to output the result as Text.");
        }

        private static void DisplayConstantsMessage()
        {
            Console.WriteLine("\r\nDefine constants to make your Code Cleaner:");
            Console.WriteLine("-------------------------------------------\r\n");
            Console.WriteLine("  const string PatientFamilyName = \"PID.5.1\";");
            Console.WriteLine("  const string PatientGivenName = \"PID.5.2\";");
            Console.WriteLine("  Console.Write($\"Patients Name is : {mesg.Element(PatientFamilyName)},");
            Console.WriteLine("  Console.Writeln({mesg.Element(PatientGivenName)}.");
            Console.WriteLine("\r\nProduces:\r\n");

        }

        private static void DisplayExtractingAnElementMessage()
        {
            Console.WriteLine("\r\nExtract an 'Element'");
            Console.WriteLine("-------------------");
            Console.WriteLine("An Element (HL7Element) is any part of sub part of a HL7Message:");
            Console.WriteLine("Eg MSH.3 (Sending System):\r\n");
        }

        private static void UseParseOnlyToConfirmWeCanProcessTheMessage(string msgText)
        {
            Console.WriteLine("Parse Only");
            Console.WriteLine("----------");
            Console.WriteLine("  Used when you don't want to proces the whole message, just check fields");
            Console.WriteLine("  Eg: determine if they message type is supported by the application");
            Console.WriteLine("  OR  for logging purposes where the system is simply storing the whole message");

            string messageType = HL7Message.ParseOnly(msgText, "MSH.9");
            if (!messageType.Substring(0, 3).Equals("ORU", StringComparison.CurrentCultureIgnoreCase))
            {
                Console.WriteLine();
                throw new FormatException($"Only processing ORU MessageType. Found message type {messageType}");
            }
        }

        private static IDataTableProvider ManuallyCreateTablesIntoAInMemoryProvider()
        {
            var tables = new InMemoryDataTableProvider();
            /* According to (say) Australian Standard 
             * https://confluence.hl7australia.com/display/OO/2+Patient+Administration+for+Pathology#id-2PatientAdministrationforPathology-PID-32.2.1.3PID-3Patientidentifierlist(CX)00106
              -- we need At least 0203 for the Identifier Type, 0363 for Assigning Authority, 
              and for check digit schemes we need 0061
             */

            // we'll make a subset of table 0203 


            return tables;
        }
        private static void AlternateMethodsForCreatingAHL7MessageObject(string msgText)
        {
            Console.WriteLine("\r\nCreating a HL7Message Object");
            Console.WriteLine("----------------------------");
            Console.WriteLine("Method 1 : Implicit cast: HL7Message mesg = msgText;");
            HL7Message mesgImplicit = msgText;

            // Alternatively = use an Explicit cast
            Console.WriteLine("Method 2 : Explicit cast: mesg = (HL7Message)msgText;");
            var mesgExplicit = (HL7Message)msgText;

            // OR simply use the constructor
            Console.WriteLine("Method 3 : Constructor: mesg = new HL7Message(msgText);");
            var mesgFromCtor = new HL7Message(msgText);

            // All Three methods produce the same result.
            Console.WriteLine("\r\nAll produce the same result:");
            Console.WriteLine($"TypeOf mesgImplicit {mesgImplicit.GetType()} - {mesgImplicit.Element("MSH")}");
            Console.WriteLine($"TypeOf mesgExplicit {mesgExplicit.GetType()} - {mesgExplicit.Element("MSH")}");
            Console.WriteLine($"TypeOf mesgfromCtor {mesgFromCtor.GetType()} - {mesgFromCtor.Element("MSH")}");
            Console.WriteLine();
        }
    }
}
