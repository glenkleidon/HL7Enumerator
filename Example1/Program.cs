using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HL7Enumerator;
using HL7Enumerator.HL7Tables;
using HL7Enumerator.HL7Tables.Interfaces;
using HL7Enumerator.Types;
using static HL7Enumerator.Types.DataTypes;
using System.Reflection;

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
              @"OBX|1|NM|GLU^Glucose Lvl||59|mg/dL|65-99^65^99|L|||F|||20061122154733|" + "\r" +
              @"OBX|2|NM|ALT^Alanine Aminotransferase||23|umol/L|2-20^65^1000|H|||F|||20061122154733|" + "\r" +
              @"OBX|3|NM|8893-0^Pulse rate^LN^78564009^^SCT|1.9.2.1|70|bpm^^ISO+||N|||F|||201706071449+1000" + "\r";

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

                // Extract a Repeating Field 
                Console.WriteLine("\r\nOR a SET of repeating Fields: eg PID.3[]");
                var c = 0;
                mesg.Element("PID.3[]").ForEach(id => Console.WriteLine($"Repitition {++c}: {id}"));

                // Use the ALLSegments property with LINQ to extract and manipulate any part/subpart of a message
                // as a Low Level HL7Element
                DisplayUsingLinq();
                Console.WriteLine("Eg Test Names: ");
                var OBXTestNames = mesg.AllSegments("OBX").Select(o => o.Element("*.3.2"));
                // Note the use of the "*" wildcard - ovoids needing to specify the Segment level.

                // Use Implicit String Casting to Output any Element as a string.
                Console.WriteLine($"\r\nThe message contains {OBXTestNames.Count()} Test Results:");
                foreach (string testName in OBXTestNames) Console.WriteLine($"  {testName}");

                // Use LINQ on ANY element at any level to iterate over sub elements
                Console.Write("\r\nUse LINQ at the Element (field/component) Level to get information\r\nEg:  ");
                var NonEmptyComponentsInPD1 =
                    mesg.Element("PD1.4").Where(f => !string.IsNullOrWhiteSpace(f));
                Console.WriteLine($"PID.3 has {(mesg.Element("PD1.4").Count)} components, and {NonEmptyComponentsInPD1.Count()} have data");
                foreach (var component in NonEmptyComponentsInPD1) Console.WriteLine($"  {component}");

                // Use self defined Constants for field definitions to make your code cleaner.
                DisplayConstantsMessage();
                const string PatientFamilyName = "PID.5.1";
                const string PatientGivenName = "PID.5.2";
                Console.WriteLine($"Patients Name is : {mesg.Element(PatientFamilyName)}, {mesg.Element(PatientGivenName)}.");

                DisplayDataTypesMessage();
                // Use the "HL7Types" Namespace Helpers to get safely get Numbered Field Values from components
                // Remember that Element FIEDLS are ZERO based but in Segments the Segment Name is 0 so the FIELD Numbers
                // still correspond to the HL7 field numbers.  
                const int fldSequence = 1;
                const int fldDataType = 2;
                const int fldTestName = 3;
                const int fldValue = 5;
                const int fldUnits = 6;
                const int fldRefRange = 7;
                const int fldAbFlag = 8;

                // BUT Components are numbered 0 to n (not 1 to n).
                const int cvTestName = 1;
                const int cvTestCode = 0;

                // Now use our defintions to extract using Data Type primitives ElementValue and IndexedElement
                Console.WriteLine("\r\nUse Safe Element Options to Output only the Numeric test Results:\r\n");
                mesg.AllSegments("OBX") // Test Segments
                    .Where(o => o.ElementValue(fldDataType).Equals("NM")) // numeric fields
                    .Select(o => $"{o.ElementValue(fldSequence),2}. " +
                                 $"{o.IndexedElement(fldTestName).ElementValue(cvTestName),-30} " + // test name
                                 $"({o.IndexedElement(fldTestName).ElementValue(cvTestCode),-6}) " +  // test code
                                 $"{o.IndexedElement(fldValue),6} " +     // test value
                                 $"{o.IndexedElement(fldUnits).ElementValue(0),-6} " + // units
                                 $"({o.IndexedElement(fldRefRange).ElementValue(0),-6}) " + // Range first field
                                 $"{(o.ElementValue(fldAbFlag).Equals("N") ? "" : o.ElementValue(fldAbFlag))}" // Abflag 
                           ).ToList()
                    .ForEach(testResult => Console.WriteLine($"  {testResult}"));

                // Use the HL7 Data Type 
                DisplayCommonDataTypes();

                // Use the DataType Extensions for Strict Typing of known Data Types.
                Console.WriteLine("\r\nUse A Common Data type to Extract Type safe properties");
                Console.WriteLine("Eg: PID.5 (Patient Name) is an XPN DataType (often repeating)");
                var ptName = mesg.Element("PID.5").AsXPN();
                // note, in this case it is ok AsXPN() because we know there is only 1 replicate.
                // for repeating fields, use AsXPNs() to return an IEnumerable of XPN
                Console.WriteLine($"Patients Name is : {ptName.FamilyName}, {ptName.GivenName}.");

                // Using a CX Data Type for repeating fields..
                var ids = mesg.Element("PID.3[]");
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

                /// One Alternative in the case where no type has been defined, 
                /// you can use the low level elements OR Self Element references to extract the data
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

                /// HOWEVER - it is better to define your own Custom Type.
                /// See the Custom Data Types example in this repository.
                DisplayCustomDataTypesMessage();

                // Data Type Support.
                DisplayHL7TablesMessage();

                // Coded Values have 3 propertes. Value, CodedValue and Best Value.
                // Use the Property 'BestValue' in most cases for display as it returns either Value 
                // or Coded Value depending on whether a table is attached and the value is known in the table.
                // When no table is defined for the Coded Value will always be empty
                var pt1IdTypeCode = patientIds.Skip(1).First().IdentifierTypeCode;
                Console.WriteLine("\r\nIdentifier Properties: with NO table attached.");
                Console.WriteLine($" Value      (Unvalidated)   : {pt1IdTypeCode.Value}");
                Console.WriteLine($" CodedValue (Validated)     : {pt1IdTypeCode.CodedValue}");
                Console.WriteLine($" BestValue  (Coded Or Value): {pt1IdTypeCode.BestValue}");
                Console.WriteLine($" Description                : {pt1IdTypeCode.Description}");
                Console.WriteLine($" Notes                      : {pt1IdTypeCode.Notes?.First()}...");

                // Attach manually created Table to CX we just read. 
                // Manually Create a Data Table into In-Memory Provider
                Console.WriteLine("\r\nManually Create Tables to validate the properties");
                var tables = ManuallyCreateTablesIntoAInMemoryProvider();
                pt1IdTypeCode.Table = tables.GetCodeTable("0203");

                Console.WriteLine("\r\nIdentifier Properties: with table 0203 attached.");
                Console.WriteLine($" Value      (Unvalidated)   : {pt1IdTypeCode.Value}");
                Console.WriteLine($" CodedValue (Validated)     : {pt1IdTypeCode.CodedValue}");
                Console.WriteLine($" BestValue  (Coded Or Value): {pt1IdTypeCode.BestValue}");
                Console.WriteLine($" Description                : {pt1IdTypeCode.Description}");
                Console.WriteLine($" Notes                      : {pt1IdTypeCode.Notes.First()}...");

                // Automatically Attach suitable tables to the Data type as we read Assign it.
                // Assuming we hav the required tables, each datatype consists of a set of 
                // Identifiers where data might be coded.
                DisplayAutomaticDataTableAssignment();

                // First Use a Table Provider as a source of tables. 
                // The File Provider can access any table in a folder by TableID is very useful for this purpose
                var HL7TablesFolder = Path.Combine(
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase).Substring(6), 
                    "ExampleTables");
                Console.WriteLine($"The process is: First, Get an instance of a table provider");
                Console.WriteLine("Eg: The 'FolderDataTableProvider'");

                var tablesFromFolder = new FolderDataTableProvider(HL7TablesFolder);
                Console.WriteLine($"Using tables in folder:{tablesFromFolder.Folder}");

                Console.WriteLine("Next, Look at the DataType (eg CX) to determine the number of Coded Elements in total");
                Console.WriteLine($"A CX data has {CX_CompositeId.TotalCodedFieldCount} Coded elements. I need a List of corresponding table names");

                // You will need 1 table reference for each Coded Item, in exact order that they will be constructed.
                Console.WriteLine("Add Required Table names to a String List IN Constructor Order\r\n");
                var tableIds = new List<string>();
                tableIds.Add("0061"); // CX_CompositeId.CheckDigitScheme needs table 0061 (we dont have that, but include it anyway)
                tableIds.Add("0363"); // CX_CompositeId.AssigningAuthority.NamespaceId needs table 0363
                tableIds.Add("0301"); // CX_CompositeId.AssigningAuthority.UniversalIDType needs table 0301
                tableIds.Add("0203"); // CX_CompositeId.IdentifierTypeCode needs table 0203
                tableIds.Add("");     // CX_CompositeId.AssigningFacility.NamespaceId needs some unknown table - leave it blank.
                tableIds.Add("0301"); // CX_CompositeId.AssigningFacility.UniversalIDType needs 0301 again (include it again)

                Console.WriteLine("Table Ids required for PID.5 (CX)");
                tableIds.ForEach(t=> Console.WriteLine($"  {t}"));

                var validatedPatientIds = ids.AsCXs(tableIds, tablesFromFolder);
                c = 0;
                validatedPatientIds.ToList().ForEach(id =>
                   {
                       Console.WriteLine($"\r\nID {++c} {id.ID}");
                       var bestValue = id.CheckDigitScheme?.BestValue;
                       var isValid = (id.CheckDigitScheme == null) ? "" : (id.CheckDigitScheme.IsValid.Value) ? "Yes" : "No";
                       var description = id.CheckDigitScheme.Description;
                       Console.WriteLine($"  CheckDigitScheme                  : {bestValue,7} Valid: {isValid} {description}");

                       bestValue = id.AssigningAuthority?.NamespaceId.BestValue;
                       isValid = (id.AssigningAuthority?.NamespaceId == null) ? "" : (id.AssigningAuthority.NamespaceId.IsValid.Value) ? "Yes" : "No";
                       description = id.AssigningAuthority?.NamespaceId.Description;
                       Console.WriteLine($"  AssigningAuthority.NamespaceId    : {bestValue,7} Valid: {isValid} {description}");

                       bestValue = id.AssigningAuthority?.UniversalIdType.BestValue;
                       isValid = (id.AssigningAuthority?.UniversalIdType == null) ? "" : (id.AssigningAuthority.UniversalIdType.IsValid.Value) ? "Yes" : "No";
                       description = id.AssigningAuthority?.UniversalIdType.Description;
                       Console.WriteLine($"  AssigningAuthority.UniversalIdType: {bestValue,7} Valid: {isValid} {description}");

                       bestValue = id.IdentifierTypeCode?.BestValue;
                       isValid = (id.IdentifierTypeCode == null) ? "" : (id.IdentifierTypeCode.IsValid.Value) ? "Yes" : "No";
                       description = id.IdentifierTypeCode?.Description;
                       Console.WriteLine($"  IdentifierTypeCode                : {bestValue,7} Valid: {isValid} {description}");

                       bestValue = id.AssigningFacility?.NamespaceId.BestValue;
                       isValid = (id.AssigningFacility?.NamespaceId == null) ? "" : (id.AssigningFacility.NamespaceId.IsValid.Value) ? "Yes" : "No";
                       description = id.AssigningFacility?.NamespaceId.Description;
                       Console.WriteLine($"  AssigningFacility.NamespaceId     : {bestValue,7} Valid: {isValid} {description}");

                       bestValue = id.AssigningFacility?.UniversalIdType?.BestValue;
                       isValid = (id.AssigningFacility?.UniversalIdType == null) ? "" : (id.AssigningFacility.UniversalIdType.IsValid.Value) ? "Yes" : "No";
                       description = id.AssigningFacility?.UniversalIdType?.Description;
                       Console.WriteLine($"  AssigningFacility.UniversalIdType : {bestValue,7} Valid: {isValid} {description}");

                   }
                );


                DisplayComposingMessages();

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

        private static void DisplayAutomaticDataTableAssignment()
        {
            Console.WriteLine("\r\nAutomated Table Assignment to Coded Elements");
            Console.WriteLine(    "--------------------------------------------");
            Console.WriteLine("The library includes a primitive for autmatically ");
            Console.WriteLine("assigning the data tables to a data type. As there ");
            Console.WriteLine("are a very large number of possible combinations of");
            Console.WriteLine("datatype and Field across languages and HL7 versions");
            Console.WriteLine("it is strongly recommended that the developer write");
            Console.WriteLine("extension methods based on specific requirements of ");
            Console.WriteLine("the application.");

        }

        private static void DisplayComposingMessages()
        {
            Console.WriteLine("\r\nComposing messages");
            Console.WriteLine("------------------");
            Console.WriteLine("While it is not the primary purpose of this libary at this time");
            Console.WriteLine("to compose messages, the ToString() in both DataTypes Objects");
            Console.WriteLine("and the HL7Element make this possible.");

        }

        private static void DisplayHL7TablesMessage()
        {
            Console.WriteLine("\r\nHL7 Tables");
            Console.WriteLine("---------------------------------");
            Console.WriteLine("The Library supports automatic Data Table Validation.  Any Defined");
            Console.WriteLine("or custom Data Table can be associated with Coded data types in any");
            Console.WriteLine("HL7Element either autmatically or manually");
        }

        private static void DisplayCommonDataTypes()
        {
            Console.WriteLine("\r\nCommon Pre-defined HL7 Data Types");
            Console.WriteLine("---------------------------------");
            Console.WriteLine("The HL7Types Namespace contains commonly used HL7 DataTypes such as: ");
            Console.WriteLine("  HD-Hierarchic Designator, CX-Extended Composite, DR-Date Range ");
            Console.WriteLine("These types are often constructed from more primitive types like:");
            Console.WriteLine("  ST-String, DT-Date, TM-Time, TS-Timestamp, NM-Numeric, ID-Coded Identifier ");
            Console.WriteLine("The Library implements many of these types as Extension Methods and");
            Console.WriteLine("ANY HL7Element can be converted to these types automatically.\r\n");

        }

        private static void DisplayCustomDataTypesMessage()
        {
            Console.WriteLine("\r\nDefining Custom Data Types");
            Console.WriteLine("--------------------------");
            Console.WriteLine("This library is using the Australian Reference Website:");
            Console.WriteLine("https://confluence.hl7australia.com/ for definitions.");
            Console.WriteLine("The Framework is easily extended to make Custom Data Types");
            Console.WriteLine("See the Creating Custom Types Example program in this repo.");
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

            DisplaySafeElementMessage();
        }

        private static void DisplaySafeElementMessage()
        {
            Console.WriteLine("\r\nSafe Indexed Element References");
            Console.WriteLine("-------------------------------");
            Console.WriteLine("With DataType Extensions, the IndexedElement and ElementValue primitives allow");
            Console.WriteLine("you to safely access the field and component elements without null checking.");

        }
        private static void DisplayCreateAnExtensionMethod()
        {
            Console.WriteLine("\r\nCreating Custom HL7 DataTypes");
            Console.WriteLine("-----------------------------");
            Console.WriteLine("The primary purpose of Indexed Element References is to allow extension methods");
            Console.WriteLine("to be defined that describe HL7 DataTypes. ");
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
            Console.WriteLine("Used when you don't want to proces the whole message, just check fields");
            Console.WriteLine("Eg: Determine if the message type is supported by the application");
            Console.WriteLine("OR  For logging purposes where the system is simply storing the whole message");

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
            /* According to (say) Australian Standard Patient Identifiers -
             * https://confluence.hl7australia.com/display/OO/2+Patient+Administration+for+Pathology#id-2PatientAdministrationforPathology-PID-32.2.1.3PID-3Patientidentifierlist(CX)00106
              -- need At least 0203 for the Identifier Type, 0363 for Assigning Authority, 
              and for check digit schemes we need 0061
             */
            // We are using MR - Medical record number for the type, so we'll make a quick subset of 0203
            var tbl0203 = new Dictionary<string, string>();
            tbl0203.Add("MR", "Medical record number<br>An identifier that is unique to a patient within a set of medical records, not necessarily unique within an application.");
            tbl0203.Add("MC", "Patient's Medicare number<br>Class: Insurance");

            // Because our test message does not comply with that standard, we need a custom table for Authority and 
            var tbl0001 = new Dictionary<string, string>();

            // add the create tables to the provider...
            tables.AddCodeTable("0203", tbl0203);
            tables.AddCodeTable("0001", tbl0001);
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
