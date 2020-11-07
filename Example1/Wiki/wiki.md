# How to use the HL7 Enumerator for Decoding HL7 Messages
  -------------------------------------------------------

_All of these examples are implemented in **Example1.csproj** in the repository._

## Test Data
For the following examples, lets assume we have the following test data defined

```
var msgText = @"MSH|^~\&|CERNER||PriorityHealth||20191020050600+1000||ORU^R01|Q479004375T431430612|P|2.3|" + "\r" +
 @"PID|||001677980~212323112^^^AUTH^MR^TESTING||SMITH^CURTIS||19680219|M||||||||||929645156318|123456789|" + "\r" +
 @"PD1||||1234567890^LAST^FIRST^M^^^^^NPI|" + "\r" +
 @"OBR|1|341856649^HNAM_ORDERID|000002006326002362|648088^Basic Metabolic Panel|||20061122151600|||||||||1620^Hooker^Robert^L||||||20061122154733|||F|||||||||||20061122140000|" + "\r" +
 @"OBX|1|NM|GLU^Glucose Lvl||59|mg/dL|65-99^65^99|L|||F|||20061122154733|" + "\r" +
 @"OBX|2|NM|ALT^Alanine Aminotransferase||23|umol/L|2-20^65^1000|H|||F|||20061122154733|" + "\r" +
 @"OBX|3|NM|8893-0^Pulse rate^LN^78564009^^SCT|1.9.2.1|70|bpm^^ISO+||N|||F|||201706071449+1000" + "\r";
```

## Parse Only
Used when you don't want to proces the whole message, just check fields.  For example,
 + Determine if the message type is supported by the application
 + For logging purposes where the system is simply storing the whole message

```
  // Dont want to process the whole message? Use the Parse Only Method to check fields before processing
  Console.WriteLine("\r\nProcessing ORU Message " + HL7Message.ParseOnly(msgText, "MSH.10"));
```
Produces:
```
Processing ORU Message Q479004375T431430612
```

## Creating a HL7Message Object

There are 3 methods for creating a HL7Message object instantce from Text;

### Method 1 : Implicit cast

```
  HL7Message mesgImplicit = msgText;
```
### Method 2 : Explicit cast
```
  var mesgExplicit = (HL7Message)msgText;
```

### Method 3 : Use the Constructor
```
  var mesgFromCtor = new HL7Message(msgText);
```

**_All Three methods produce the same result._**

```
  Console.WriteLine($"TypeOf mesgImplicit {mesgImplicit.GetType()} - {mesgImplicit.Element("MSH")}");
  Console.WriteLine($"TypeOf mesgExplicit {mesgExplicit.GetType()} - {mesgExplicit.Element("MSH")}");
  Console.WriteLine($"TypeOf mesgfromCtor {mesgFromCtor.GetType()} - {mesgFromCtor.Element("MSH")}");
```

Output:

```
  TypeOf mesgImplicit HL7Enumerator.HL7Message - MSH|^~\&|CERNER||PriorityHealth||||ORU^R01|Q479004375T431430612|P|2.3|
  TypeOf mesgExplicit HL7Enumerator.HL7Message - MSH|^~\&|CERNER||PriorityHealth||||ORU^R01|Q479004375T431430612|P|2.3|
  TypeOf mesgfromCtor HL7Enumerator.HL7Message - MSH|^~\&|CERNER||PriorityHealth||||ORU^R01|Q479004375T431430612|P|2.3|
```


## Extracting Segments, Fields and Subfields.

The Property `HL7Message.Element` allows you to access any part of a HL7 Message from a Segment to 
a sub-field using a simple Text Descriptor.

### Extract a Single field

```
  // Use the Element property to extract ANY specific field/subfield OR Group of fields/subFields
  Console.WriteLine($"Message received from Sending system {mesg.Element("MSH.3")}");
```

```
Message received from Sending system CERNER
```

### Extract a Whole Segment
```
  Console.Write("A whole PD1 Segment :\r\n  ");
  Console.WriteLine(mesg.Element("PD1"));
```

```
A whole PD1 Segment :
  PD1||||1234567890^LAST^FIRST^M^^^^^NPI|

```


### Extract a Repeating Field 
```
  Console.WriteLine("A SET of repeating Fields: eg PID.3[]");
  var c = 0;
  mesg.Element("PID.3[]").ForEach(id => Console.WriteLine($"Repitition {++c}: {id}"));
```

```
A SET of repeating Fields: eg PID.3[]
Repitition 1: 001677980
Repitition 2: 212323112^^^AUTH^MR^TESTING
```


## Using LINQ with HL7Messages.

Use the `HL7Message.ALLSegments` property with LINQ to extract and manipulate any part/subpart of a message

### Using Primitive (Low Level) Elements

Extract the Name Part of each OBX Result Name field into an IEnumerable<HL7Element>.
```
  Console.WriteLine("Test Names: ");
  var OBXTestNames = mesg.AllSegments("OBX").Select(o => o.Element("*.3.2"));
  // Note the use of the "*" wildcard - avoids needing to specify the Segment level.
```
Implicit string casting allows you to treat any **Element** as a string

```
  Console.WriteLine($"The message contains {OBXTestNames.Count()} Test Results:");
  foreach (string testName in OBXTestNames) Console.WriteLine($"  {testName}");
```

```
The message contains 3 Test Results:
  Glucose Lvl
  Alanine Aminotransferase
  Pulse rate
```


### Use LINQ on any Element at any level to iterate over sub-elements

Get just the populated fields of a PD1.4 (Primary Care provider)
```
  var NonEmptyComponentsInPD1 =
  mesg.Element("PD1.4").Where(f => !string.IsNullOrWhiteSpace(f));
  Console.WriteLine($"PID.4 has {(mesg.Element("PD1.4").Count)} components, and {NonEmptyComponentsInPD1.Count()} have data");
  foreach (var component in NonEmptyComponentsInPD1) Console.WriteLine($"  {component}");
```

Output:
```
PID.3 has 9 components, and 5 have data
  1234567890
  LAST
  FIRST
  M
  NPI
```

## Use Self Defined Constants for Cleaner Code.
It is recommended that for each field you intend to use in your application a 
constant be defined.  

```
  const string PatientFamilyName = "PID.5.1";
  const string PatientGivenName = "PID.5.2";

  // now use the constants in your code.
  Console.WriteLine($"Patients Name is : {mesg.Element(PatientFamilyName)}, {mesg.Element(PatientGivenName)}.");
```

```
Patients Name is : SMITH, CURTIS.
```
This make the code a lot easier to read and maintain.

Depending on your application, the right approach is probably defined a constants
sub class in a class the implements the specific behaviour you want to address.  For example
a class that extract the numeric and structured numeric result data from a results, Create \
a `class NumericResults` with a subclass of `NumericResults.Constants`.

Then `constants` might contain _ResultValue = "*.5"_, _ResultName = "*.3.2"_ ,
_ResultCode="*.3.1"_, _ResultCodingSystem = "*3.2.3"_ (and similar for alternates).  

Alternatively (and preferrably) you could define _ResultField="*.5.1"_ and 
_ResultIdentifiers = "*.3"_ and then use these in a Linq query to extract all Numeric and 
Structured numerics OBX values from each OBR.  Then cast the returned elements as an 
approrpriate Datatype (**NM**, **SN** etc for result and **CE** for the Identifiers).


## Use the "HL7Types" Namespace Helpers to safely get contents without Null checking

The `HL7Element` class is an _**IEnumerable class of itself**_.  This means that there **may** be a 
populated `HL7Elements.Value` property OR the values may be contained in nested List<HL7Elements>.

The Helper methods _**ElementValue**_ and _**IndexedElement**_ abstract this for you 
and provide **safe** access to list `HL7Elements` without the need for null checking.

It is important to remember that `HL7Element` Lists are Zero Based.  Text Descriptors are 1 based.
In the case of _**SEGMENT FIELDS**_ are ZERO based but in Segments the Segment Name is Element 0, so 
the Element Field Numbers still correspond to the HL7 field numbers.  However, the components 
and subcomponents are zero based.

```
// OBX Segment fields (correspond to field Ids)
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
```

Now use our defintions to extract using Data Type primitives ElementValue and IndexedElement

```
Console.WriteLine("Use Safe Element Options to Output only the Numeric test Results:\r\n");
mesg.AllSegments("OBX") // Test Segments
.Where(o => o.ElementValue(fldDataType).Equals("NM")) // numeric fields
.Select(o => $"{o.ElementValue(fldSequence),2}. " +
 $"{o.IndexedElement(fldTestName).ElementValue(cvTestName),-30} " + // test name
 $"({o.IndexedElement(fldTestName).ElementValue(cvTestCode),-6}) " +  // test code
 $"{o.IndexedElement(fldValue),6} " + // test value
 $"{o.IndexedElement(fldUnits).ElementValue(0),-6} " + // units
 $"({o.IndexedElement(fldRefRange).ElementValue(0),-6}) " + // Range first field
 $"{(o.ElementValue(fldAbFlag).Equals("N") ? "" : o.ElementValue(fldAbFlag))}" // Abflag 
   ).ToList()
.ForEach(testResult => Console.WriteLine($"  {testResult}"));
```

```
Use Safe Element Options to Output only the Numeric test Results:

   1. Glucose Lvl(GLU   ) 59 mg/dL  (65-99 ) L
   2. Alanine Aminotransferase   (ALT   ) 23 umol/L (2-20  ) H
   3. Pulse rate (8893-0) 70 bpm(  )
```


# Using the HL7 DataType Namespace

The primary purpose of the HL7 DataTypes Namespace is to provide and extensible framework
for accessing HL7Elements in a type-safe way.

The implementation is there NOT exhaustive.  That also means that it is not _**restrictive**_ 
meaning you can extend the the library any way you want.

## Common Pre-defined HL7 Data Types

The HL7Types Namespace contains commonly used HL7 DataTypes such as:
  HD-Hierarchic Designator, CX-Extended Composite, DR-Date Range

These types are often constructed from more primitive types like:
  ST-String, DT-Date, TM-Time, TS-Timestamp, NM-Numeric, ID-Coded Identifier

The Library implements many of these types as Extension Methods and
ANY HL7Element can be converted to these types automatically.

| Type     |  Description |
|----------|--------------|
| CE       |  Coded Elements |
| CNE | Coded Elements No Exceptions |
| CWE | Coded Element With Exceptions |
| CX | Composite Id |
| DR | Date Range |
| ED | Encapsulated Data |
| EI | Entity Identifiers |
| HD | Hierachic Designator |
| ID | Coded Value |
| IS | Coded Value Custom |
| NM | Numeric |
| SAD | Street Address  |
| SN | Structured Numeric |
| TN | Telephone number |
| VID | Version Identifier |
| XAD | Extended Address |
| XCN | Extended Composite Id and Name |
| XON | Extended Composite for Organisations |
| XPN | Extended Person Name |
| XTN | Extended Telecommunications |
 

## Use the DataType Extensions for Strict Typing of known Data Types.

Use A Common Data type to extract Type-safe properties. For example: the XPN datatype.

```
Console.WriteLine("Eg: PID.5 (Patient Name) is an XPN DataType (often repeating)");
var ptName = mesg.Element("PID.5").AsXPN();
Console.WriteLine($"Patients Name is : {ptName.FamilyName}, {ptName.GivenName}.");
```

```
Patients Name is : SMITH, CURTIS.
```

## Using a Data Type for repeating fields.

Note, in the case above, it is ok to use _AsXPN()_ because we know there is only 1 replicate in our example
data. But **PID.5** should typically be address using the _AsXPN**s**()_ to return an IEnumerable of 
_XPN_.

From the 2.4 Spec, **PID.3** is a replicating field of **CX**'s.  The included _AsCX**s**()_ method is
a simple extension method returning a CX_CompositeId type (supporting the IHL7Type interface).
If the include CX datatype is not suitable for your purpose, it is very easy to make your own.  
Just Implement the HL7Type interface using the low level methods.

_NOTE: if you ask for a simple CX for a Replicating field, you will get a DEBUG exception indicating 
that you should use an Enumerable type for this field._

```
var ids = mesg.Element("PID.3[]");
var patientIds = ids.AsCXs();

Console.WriteLine($"Found {patientIds?.Count()} Ids using DataType Assigment");
foreach (var cx in patientIds)
{
// remember that certain elements in a CX MAY be null.
Console.WriteLine($"ID:   {cx?.ID}");
Console.WriteLine($"  IDType:   {cx?.IdentifierTypeCode}");
Console.WriteLine($"  Authority:{cx?.AssigningAuthority?.NamespaceId?.BestValue}");
Console.WriteLine($"  Facility: {cx?.AssigningFacility?.NamespaceId?.BestValue}");
}
```

```
Found 2 Ids using DataType Assigment
ID:       001677980
  IDType:
  Authority:
  Facility:
ID:       212323112
  IDType:
  Authority:AUTH
  Facility: TESTING
 Found 2 Ids Using Elements (low level)
ID: 001677980
ID: 212323112
  IDType:MR
  Issuer:AUTH
```

## Date Conversions

HL7 DataTypes supports DateTime conversion automatically managing TimeZone information.

The following time examples use these elements...

```
var ptDOB = "PID.7";
var mesgTime = "MSH.7";

var dob = mesg.Element(ptDOB);
var mesgTs = mesg.Element(mesgTime);
```

### HL7Element.AsDateTime([DateTimeStyle])

Convert Dates in UTC and Local Format
Converts to DateTime storing timezone information if included and converting to UTC if possible.

```
Console.WriteLine($"DOB.AsDateTime() {mesg.Element(ptDOB)} does not have Timezone, so assume Local");
Console.WriteLine($"  {dob.AsDateTime()} = {dob.AsDateTime().Value.ToString("o")}");
Console.WriteLine($"msgTs.AsDateTime() {mesg.Element(mesgTime)} has Time Timezone. Stored as UTC.");
Console.WriteLine($"  {mesgTs.AsDateTime()} = {mesgTs.AsDateTime().Value.ToString("o")}");
```

```
DOB.AsDateTime() 19680219 does not have Timezone, so assume Local
  19/02/1968 12:00:00 AM = 1968-02-19T00:00:00.0000000
msgTs.AsDateTime() 20191020050600+1000 has Time Timezone. Stored as UTC.
  19/10/2019 7:06:00 PM = 2019-10-19T19:06:00.0000000Z
```  

### HL7Element.AsLocalTime()

If you want to work in localTime, ALWAYS use the AsLocalTime() method
Converts to current Local regardless of Original TimeZone
and current daylight saving settings.

```
Console.WriteLine($"DOB {mesg.Element(ptDOB)} No timezone, so display Current LOCAL");
Console.WriteLine($"  {dob.AsLocalTime()} = {dob.AsLocalTime().Value.ToString("o")}");
Console.WriteLine($"msgTime {mesg.Element(mesgTime)} Has TimeZone Shows in Current local");
Console.WriteLine($"  {mesgTs.AsLocalTime()} = {mesgTs.AsLocalTime().Value.ToString("o")}");
```

```
DOB 19680219 No timezone, so display Current LOCAL
  19/02/1968 12:00:00 AM = 1968-02-19T00:00:00.0000000+11:00
msgTime 20191020050600+1000 Has TimeZone Shows in Current local
  20/10/2019 6:06:00 AM = 2019-10-20T06:06:00.0000000+11:00
```

### HL7Element.AsUTCTime()

If you want to work in UTC time, ALWAYS use the AsUTCTime() method.
Converts to UTC regardless of the original timezone but preserves
local behaviour for timestamps without Timezone information.

```
DisplayAsUTCDateTimeInformation();
Console.WriteLine($"DOB {mesg.Element(ptDOB)} No Timezone, So safe to assume local to you");
Console.WriteLine($"  {dob.AsUTCTime()} = {dob.AsUTCTime().Value.ToString("o")}");
Console.WriteLine($"DOB {mesg.Element(ptDOB)} No Timezone, but can be forced to UTC.");
Console.WriteLine($"  {dob.AsDateTime(DateTimeStyles.AssumeUniversal).Value.ToString("o")}");
Console.WriteLine($"msgTime {mesg.Element(mesgTime)} is converted to UTC Time.");
Console.WriteLine($"  {mesgTs.AsUTCTime()} = {mesgTs.AsUTCTime().Value.ToString("o")}" );
```

```
DOB 19680219 No Timezone, So safe to assume local to you
  19/02/1968 12:00:00 AM = 1968-02-19T00:00:00.0000000
DOB 19680219 No Timezone, but can be forced to UTC.
  1968-02-19T11:00:00.0000000+11:00
msgTime 20191020050600+1000 is converted to UTC Time.
  19/10/2019 7:06:00 PM = 2019-10-19T19:06:00.0000000Z
```

## Numeric Data Type Conversions

There are Extension Methods for the _**NM_Numeric**_ Datatype allowing
for both **implicit** and **explicit** numeric conversion.   Use
which ever method you prefer (they are equivalent).

Exceptions wil be raised if the value will not cast.

```
const string ResultValue = "OBX[1].5";

// Examples:
var result1 = mesg.Element(ResultValue).AsNM();
int nmAsInt = result1.AsInteger();
var nmAsFloat = result1.AsFloat();
double nmAsdouble = result1;
var nmAsCurrency = (decimal)result1;
Console.WriteLine($"Result 1 ToString() {result1}");
Console.WriteLine($"Result 1 {nmAsInt.GetType().Name} value {nmAsInt}");
Console.WriteLine($"Result 1 {nmAsFloat.GetType().Name} value {nmAsFloat:0.0}");
Console.WriteLine($"Result 1 {nmAsdouble.GetType().Name} value {nmAsdouble:0.00000000}");
Console.WriteLine($"Result 1 {nmAsCurrency.GetType().Name} value {nmAsCurrency:0.00}");

```

```
Result 1 ToString() 59
Result 1 Int32 value 59
Result 1 Single value 59.0
Result 1 Double value 59.00000000
Result 1 Decimal value 59.00
```

## Defining Custom Data Types

This library bases the defined data types using the [Australian Reference Website](https://confluence.hl7australia.com/) as a reference.  However, the 
the Framework is easily extended to make **Custom Datatypes** and is _no way restricted_ to these definitions.

In the case where no type has been defined in the libarary, it is possible to use the low level elements 
OR the Safe Element references to extract the data.

```
...
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
```

_HOWEVER_ it is better to define your own Custom Type extension.  This is simple to do and
makes your code far more maintainable.

```
...
Console.WriteLine($" Found {ids.Count} Ids Using Elements (low level)");
var myElements = ids.AsMyElements();

myElements.toList().ForEach( 
   e=> 
    Console.Writeln(IdType: {e.IdType}";
    Console.Writeln(Issuer: {e.Issuer}";
 )

```

See the Custom Data Types example [TO DO!]() in this repository.
_note: the example does not yet exist, but look at **DataTypes.SAD_StreetAddress** for example 
without Coded Elements and **DataTypes.HD_HierarchicDesignator** for an example with 
coded elements deriving from `HL7TypeBase` and implementing `Ihl7Type`._

# HL7 Data Table Support.

The Coded Value Data types **IS** and **ID** may contain values that need to be linked
to a Defined Table to explain their meaning.

The Library supports automatic Data Table Validation and linking.  Any Defined
or custom Data Table can be associated with Coded data types in any
HL7Element either manually or automatically.

The library's base type **DataTypes.CodedDataValue** implements an interface _**ICodedDataValue**_

```
   public interface ICodedDataValue
    {
        string CodedValue { get; }
        string Description { get; }
        IEnumerable<string> Notes { get; }
        bool? IsValid { get; }
        string Value { get; set; }
        string BestValue { get; }
        string TableId { get; set; }
        Dictionary<string, string> Table { get; set; }
        IDataTableProvider TableProvider { get; set; }
    }
```

## Understanding the Value properties of the ICodedDataValue Interface.

Values have 3 propertes. _Value_, _CodedValue_ and _BestValue_.
Use the Property 'BestValue' in most cases as it returns either Value 
or Coded Value depending on whether a table is attached and the value is known in the table.

When no table is attached to the instance, _CodedValue_ will always be empty.  _Value_ will always
be populated whether the value is valid or not.

```
var pt1IdTypeCode = patientIds.Skip(1).First().IdentifierTypeCode;
Console.WriteLine("\r\nIdentifier Properties: with NO table attached.");
Console.WriteLine($" Value  (Unvalidated)   : {pt1IdTypeCode.Value}");
Console.WriteLine($" CodedValue (Validated) : {pt1IdTypeCode.CodedValue}");
Console.WriteLine($" BestValue  (Coded Or Value): {pt1IdTypeCode.BestValue}");
Console.WriteLine($" Description: {pt1IdTypeCode.Description}");
Console.WriteLine($" Notes  : {pt1IdTypeCode.Notes?.First()}...");
```

```
Identifier Properties: with NO table attached.
 Value      (Unvalidated)   : MR
 CodedValue (Validated)     :
 BestValue  (Coded Or Value): MR
 Description                :
 Notes                      : ...
```

## Manually Attach a Data Table from the In-Memory Provider

Tables are made available through an interface _IDataTableProvider_. 

```
namespace HL7Enumerator.HL7Tables.Interfaces
{
    public interface IDataTableProvider
    {
        Dictionary<string, string> GetCodeTable(string tableId);
        void AddCodeTable(string tableId, Dictionary<string, string> table);
        void Clear(string tableId = null);
        string GetTableValue(string tableId, string key);

    }
}
```

Assuming we have access to a provider, we can manually attach a Table to the CX 
extracted in the example above.

```
var tables = ManuallyCreateTablesIntoAInMemoryProvider();
pt1IdTypeCode.Table = tables.GetCodeTable("0203");

Console.WriteLine("Identifier Properties: with table 0203 attached.");
Console.WriteLine($" Value  (Unvalidated)   : {pt1IdTypeCode.Value}");
Console.WriteLine($" CodedValue (Validated) : {pt1IdTypeCode.CodedValue}");
Console.WriteLine($" BestValue  (Coded Or Value): {pt1IdTypeCode.BestValue}");
Console.WriteLine($" Description: {pt1IdTypeCode.Description}");
Console.WriteLine($" Notes  : {pt1IdTypeCode.Notes.First()}...");
```

## Automated Table Assignment to Coded Elements

The library includes a primitive for autmatically assigning the data tables to a
data type. As there are a very large number of possible combinations of Datatypes
and Field across languages and HL7 versions it is strongly recommended that the 
developer write extension methods based on specific requirements of the application.

The process is: 

    1. Get an instance of a table provider.
	Eg: The built-in _**FolderDataTableProvider**_

	The File Provider can access any table in a folder by TableID is very useful for this purpose

	```
	var HL7TablesFolder = Path.Combine(
	Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase).Substring(6), 
	"ExampleTables");

	var tablesFromFolder = new FolderDataTableProvider(HL7TablesFolder);
	Console.WriteLine($"Using tables in folder:{tablesFromFolder.Folder}");

	```

	```
	Using tables in folder:C:\Users\glen\Documents\HL7Enumerator\Example1\bin\Debug\ExampleTables
	```
	2. Look at the DataType (eg CX) to determine the number of Coded Elements in total.
	A CX data has 6 Coded elements. Each element needs a corresponding table name.

	Add Required Table names to a String List in the order that they will be constructed.
	If you dont have or care about a particular element, then leave a empty entry for that
	table in the list.

	Table Ids required for PID.5 (CX)
	  0061
	  0363
	  0301
	  0203
	  _empty_
	  0301

	  
	```
	// You will need 1 table reference for each Coded Item, in exact order that they will be constructed.
	Console.WriteLine("Add Required Table names to a String List IN Constructor Order\r\n");
	var tableIds = new List<string>();
	tableIds.Add("0061"); // CX_CompositeId.CheckDigitScheme needs table 0061 (we dont have that, but include it anyway)
	tableIds.Add("0363"); // CX_CompositeId.AssigningAuthority.NamespaceId needs table 0363
	tableIds.Add("0301"); // CX_CompositeId.AssigningAuthority.UniversalIDType needs table 0301
	tableIds.Add("0203"); // CX_CompositeId.IdentifierTypeCode needs table 0203
	tableIds.Add(""); // CX_CompositeId.AssigningFacility.NamespaceId needs some unknown table - leave it blank.
	tableIds.Add("0301"); // CX_CompositeId.AssigningFacility.UniversalIDType needs 0301 again (include it again)
	```

_Note: Different CXs will have different purposes, this list of TableIds is suitable for PID 5, but 
other CX data types will need their own table list._

Now we can simply Cast the PID.5 element as an IEnumerable of CXs withe the _AsCX**s** method 
passing in the `TableIds` and `TableProvider`.

```
var validatedPatientIds = ids.AsCXs(tableIds, tablesFromFolder);
```

Outputing the objects shows that the nested properties are automatically linked and
validated against the tables.

```
c = 0;
validatedPatientIds.ToList().ForEach(id =>
   {
   Console.WriteLine($"\r\nID {++c} {id.ID}");
   var bestValue = id.CheckDigitScheme?.BestValue;
   var isValid = (id.CheckDigitScheme == null) ? "" : (id.CheckDigitScheme.IsValid.Value) ? "Yes" : "No";
   var description = id.CheckDigitScheme.Description;
   Console.WriteLine($"  CheckDigitScheme  : {bestValue,7} Valid: {isValid} {description}");

   bestValue = id.AssigningAuthority?.NamespaceId.BestValue;
   isValid = (id.AssigningAuthority?.NamespaceId == null) ? "" : (id.AssigningAuthority.NamespaceId.IsValid.Value) ? "Yes" : "No";
   description = id.AssigningAuthority?.NamespaceId.Description;
   Console.WriteLine($"  AssigningAuthority.NamespaceId: {bestValue,7} Valid: {isValid} {description}");

   bestValue = id.AssigningAuthority?.UniversalIdType.BestValue;
   isValid = (id.AssigningAuthority?.UniversalIdType == null) ? "" : (id.AssigningAuthority.UniversalIdType.IsValid.Value) ? "Yes" : "No";
   description = id.AssigningAuthority?.UniversalIdType.Description;
   Console.WriteLine($"  AssigningAuthority.UniversalIdType: {bestValue,7} Valid: {isValid} {description}");

   bestValue = id.IdentifierTypeCode?.BestValue;
   isValid = (id.IdentifierTypeCode == null) ? "" : (id.IdentifierTypeCode.IsValid.Value) ? "Yes" : "No";
   description = id.IdentifierTypeCode?.Description;
   Console.WriteLine($"  IdentifierTypeCode: {bestValue,7} Valid: {isValid} {description}");

   bestValue = id.AssigningFacility?.NamespaceId.BestValue;
   isValid = (id.AssigningFacility?.NamespaceId == null) ? "" : (id.AssigningFacility.NamespaceId.IsValid.Value) ? "Yes" : "No";
   description = id.AssigningFacility?.NamespaceId.Description;
   Console.WriteLine($"  AssigningFacility.NamespaceId : {bestValue,7} Valid: {isValid} {description}");

   bestValue = id.AssigningFacility?.UniversalIdType?.BestValue;
   isValid = (id.AssigningFacility?.UniversalIdType == null) ? "" : (id.AssigningFacility.UniversalIdType.IsValid.Value) ? "Yes" : "No";
   description = id.AssigningFacility?.UniversalIdType?.Description;
   Console.WriteLine($"  AssigningFacility.UniversalIdType : {bestValue,7} Valid: {isValid} {description}");

   }
);
```

```
ID 1 001677980
  CheckDigitScheme                  :         Valid: No
  AssigningAuthority.NamespaceId    :         Valid:
  AssigningAuthority.UniversalIdType:         Valid:
  IdentifierTypeCode                :         Valid: No
  AssigningFacility.NamespaceId     :         Valid:
  AssigningFacility.UniversalIdType :         Valid:

ID 2 212323112
  CheckDigitScheme                  :         Valid: No
  AssigningAuthority.NamespaceId    :    AUTH Valid: No
  AssigningAuthority.UniversalIdType:         Valid: No
  IdentifierTypeCode                :      MR Valid: Yes Medical record number
  AssigningFacility.NamespaceId     : TESTING Valid: No
  AssigningFacility.UniversalIdType :         Valid: No

```

# Composing messages

While it is not the primary purpose of this libary at this time to compose messages, 
the ToString() in both DataTypes Objects and the HL7Element make this possible.


##  Compose a Segment using DataTypes

Compose the following PID Segment.

```
PID||1234567^4^M11^ADT01^MR^University Hospital|1234567^4^M11^ADT01^MR^University Hospital~8003608833357361^^^AUSHIC^NI^
```

We'll make a IEnumerable<CX_CompositeId>;  

```
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

`PID.2` is a legacy field containing only a single CX.  But `PID.3` is a repeating field
of CX.

```
// Compose a PID 
var pid = new HL7Segment("PID");
pid.SetField(2, patientIdentifiers.First());
pid.SetField(3, patientIdentifiers.AsElement());

Console.WriteLine("\r\nComposed PID: " + pid);
```


```
Composed PID: PID||1234567^4^M11^ADT01^MR^University Hospital|1234567^4^M11^ADT01^MR^University Hospital~8003608833357361^^^AUSHIC^NI^
```
----------------------------------------------------------------------------
