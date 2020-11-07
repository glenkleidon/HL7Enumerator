# HL7Enumerator
C# Library for parsing and enumerating HL7 Messages with LINQ support. With Release 1.2.0, there is also 
an extensible framework for HL7 Datatypes and HL7 Tables.

## History
This project was started as a simple conversation with my colleague who thought that of the 
available C# HL7 parser libraries out there, none seem to be simple to use and were 
overly complex for the requirements.

We wanted to be able to perform actions such as
```
    string msgText = @"MSH|^~\&|CERNER||PriorityHealth||||ORU^R01|Q479004375T431430612|P|2.3|" + "\r" +
    
    if !CanHandle(mesgText) throw new ArgumentException("Not a message we can handle");
    
    HL7Message mesg = msgText;
    
    string sendingSystem = mesg.Element("MSH.3");
    Console.WriteLine(string.Format("Message received from Sending System {0}", sendingSystem));

    Console.WriteLine("PD1 Segment    : " + mesg.Element("PD1"));
    Console.WriteLine("2nd OBX Segment: " + mesg.Element("OBX[2]"));

    //Now Locate the Test names in all the OBX records 
    var OBXTestNames = mesg.AllSegments("OBX").Select(o => o.Element("*.3.2").ToString());

    Console.WriteLine("Found Tests:");
    foreach (string obx in OBXTestNames) Console.WriteLine(string.Format("  {0}",obx));
    
    ... 
    
```


I agreed that the task should not be that hard to do.  And so the ball started rolling....

## Core Library Class Structure 

The core library is deliberately very simple.  However, as it leverages the *IEnumerable* interface very complex functionality 
is supported through LINQ.  The DataTypes and HL7 Tables are extension classes that make using core functionality much easier
and allows automatic conversion to commond HL7 Data Types with validation against standard or custom HL7 Tables.

The core library contains 3 operational classes:   
   + *class* **HL7Message** - Top Level element representing a single message
   + *class* **HL7Element** - Any message or part of a message (and the base class of *HL7Message*)
   + *static class* **EscapeOBXCRLF** - Checks for the presence of unescaped CR and LF in the OBX body and escapes it.

2 helper classes relating to Search Criteria: 
   + *class* **SearchCriteria** - A set of criteria representing the full path to a **HL7Element**
   + *class* **SearchCriteriaElement** - A specific element within the search path (base calss of *SearchCriteria*)

and a single Constants Class:
   + *class* Constants 

Each of the 4 main classes implement implicit casting between the class and *string*.  This makes it possible to 
ignore the Search Criteria classes for most operations - criteria is implicitly converted from the text version.  

## Methods

The **HL7Element** class is a descendant of a generic list of *itself* (IE a _**List\<HL7Element\>**_) and therefore 
inherits the *IEnumerable* interface.  This makes the class automatically support LINQ queries.  

There is 1 pubic method : *Element* which provides access to specific HL7 (sub) elements  The *Element* method is typically called from the HL7 within a Linq query to access elements during enumeration.  

It is not usually necessary to call the *Element* method of **HL7Element** directly.  It is more common to use the method
by calling the overridden version of *Element* from the **HL7Message** class.

The **HL7Message** class has one static Method *ParseOnly* which provides efficent access to single fields, and one public method *AllSegments* which returns a generic list of *HL7Element* containing all segments of a particular type (eg return all OBX records). The *ParseOnly* method is typically for ensuring a message can be handled (by checking the message type) or for logging purposes to extract the timestamp and the Message Number before simply storing the message. The *AllSegments* method is not strictly required as it simply encapsulates a LINQ query, although it is convenient.

## Static Methods

### HL7Message Static Methods
   + _**ParseOnly(message, Criteria)**_ - Provides an efficient method of extracting a field from a message without processing the entire message eg
   
   ```if (!HL7Message.ParseOnly(mesgText, "MSH.9").Equals("ORU^R01") throw new ArgumentException("Message type not supported");```

## Properties

Again, to emphasise the simplicity of the class, there are very few properties on the classes.

### HL7Message Properties
   + _HL7Element **Segments**_ Provides access to all the HL7 Segments as a Single Element.
   + _string **Separators**_ a represents the characters used as delimiters in the message.  Note: the order of these is different to 
      those seen in the actual message itself.  There is a helper method _Constants.ToMSHSeparators_ to convert from class format to Message format.

### HL7MessageElement Properties
   + _HL7Element **Parent**_ A reference to the Parent Element (eg the Parent of a HL7 *Field* Element is a HL7 *Segment* Element). This
     property can be use to ascend the tree of Elements to the Message level.
   + _char **Separator**_ The character used to delimit the element when it was created.
   + _string **Value**_ a (nullable) string representing the specific data held in the element. IE Say MSH.9 contains message type 
     \"ORU\^R01\" - This consisting of two elements: Element1.Value = \"ORU\" and Element2.Value = \"R01\"

### SearchCriteria Properties

   + _string **Segment**_  - The 3 Character text of the target Segment (eg MSH)
   + _int **SegmentRepetition**_ - The (1 based) reference to the target occurrence of the segment (eg 2 means find the SECOND occurrence of the segment)
   + _SearchCriteriaElement **Field**_ - A reference to the search criteria for the HL7 Field level element.
   + _SearchCriteriaElement **Component**_ - A reference to the search criteria for the HL7 Component level element. 
   + _SearchCriteriaElement **Subcomponent**_ - A reference to the search criteria for the HL7 SubComponent level element.

### SearchCriteriaElement Properties

   + _readonly int **Repetition**_ Indicates the target (1 based) repeating Field replicate 
   + _readonly int **Position**_ Indicates the target (1 based) element 
   + _bool **Enabled**_ Indicate that this criteria is in force (the parsing engine will abort at the FIRST disabled search criteria)
   + _bool **Skip**_ - The "THIS" indicator used to mean "* at the current level *" eg *.2 find the SECOND element at this level
   + _string **Value**_ - The TEXT to search for (eg SEGMENT NAME or Element Value)

*Reminder: The SearchCriteria classes for the most part can be ignored as they are primarily used to encapsulate the implicit casting of
the string path sequence by the parsing engine. There may be low level use but specific instantiation and manipulation of the SearchCriteria is 
not recommended.**

## Search Criteria Format

The search criteria at the **HL7Message** level is of the following format

```<Segment>[['['<replicate>']']|*[.|/]<Field>['['<replicate>']'][.|/]<Component>[.|/]<SubComponent>]```

*All of Fields after Segment are optional*

For example: 

   ```OBR[2]/16[3]/2/1` alternatively `OBR[2].16[3].2.1` or just `OBR[2].16[3].2``` 

will return the *Surname* of the *3rd* \'Ordering Provider\' in the *2nd* OBR16 encountered in the message.

### Requesting Replicates

You may specify an EMPTY replicate for the FIELD property meaning that ALL replicates for the field will be returned. 

Return All of the Patient Identifiers:  

```PID.3[]```
    
Returns as an Array of HL7Element containing all of the identifier fields.

In contrast `PID.3` is equivalent to `PID.3[1]` 

*This feature was release in Nuget Package Version 1.1.1*

## File Line Endings

As of release *1.1.2* the Library is **agnostic** of line endings provided they are *consistent* across a file.  Prior to this
release only the CR ($0D) line endings were properly supported. Despite the fact that the specification is clear that HL7 Messages **MUST**
use only $0D (Carraige Return), consistent support is difficult due to common operating systems using different line endings by default.

From this release, differences in file line endings can be safely ignored. 


## Implicit *string* Casting

There is no need to create specific instances of Search Criteria classes.  The library will implicitly cast string base search criteria into an array of SearchCriteriaElements.    
Eg 

``` 
     Console.WriteLine("PD1 Field 4: "+mesg.Element("pd1.4") ); 
```

is exactly equivalent to the very long winded version:

```  
     SearchCriteria pd1SearchCriteria = new SearchCriteria();
     pd1SearchCriteria.Segment = "PD1";
     pd1SearchCriteria.SegmentRepitiion = 1;
     pd1SearchCriteria.Field = new SearchCriteriaElement(4);
      
     HL7Element pd1Element = mesg.Element(pd1SearchCriteria);

     Console.WriteLine(string.Format("PD1 Field 4: {0},pd1Element.ToString()));
```

You can choose to return Elements from a *HL7Message* instance as *string* or *HL7Element* classes and use them interchangably.

## Managing Base64 and RTF common failure to Escape CRLF.

It has been observed that many systems implementing HL7 encoding forget to escape the CRLF in the contents of Base64 encoded
data in the OBX record.  The reason being that most Base64 libraries assume mime encoding where the 
recommended limit is 78 chars per line.

The static function *EscapeOBXCRLF* is called automatically in the implicit string cast to HL7Message. However, calling
the constructor directly does not evoke the function.  Because this is a static function, it can be called on any message text 
prior to evoking the constructor or as required.

The method will also work for RTF documents (or any other type for that matter) where CRs or CRLF's have not been 
escaped in the OBX segments.  

*NOTE: The method does not affect segments other than OBX and the CRLFs will be escaped to \\X0D\\\\X0A\\.*

## LINQ Queries

The Base _class **HL7Element**_ is a List\<HL7Element\> which inherits the *IEnumerable* interface for Generic List.
It is therefore possible to apply LINQ expressions to a message or an element within the message.

The *Segments* property is a **HL7MessageElement** which contains all the segments of the message.

For convenience the method **HL7Message.AllSegments**  returns a *reduced* generic List of segments of a 
particular type which if often a sensible starting point.  *[Note: this is not strictly required
within the class and probably should be re-implemented as an extension method]*

Using the implicit string cast of **SearchCriteria** makes LINQ expressions relatively clean. 
For example, the following expression returns all (obx) Test names in the message.

```var OBXTestNames = mesg.AllSegments("OBX").Select(o => o.Element("*.3.2").ToString());```

Note the use of the * wildcard ("this") operater to of the indicate the current object where there may be some ambiguity to at which level
the element resides.
 
## Composing HL7 Message
\
While it is not the primary purpose of this libary at this time to compose messages, the ToString() methods in both the DataTypes Objects
and the HL7Element make this possible.

## The DataTypes Extension Class

These notes are Work in progres...  check out the [WIKI](https://github.com/glenkleidon/HL7Enumerator/wiki)

## The HL7 Tables Extension Class

These notes are Work in progress  check out the [WIKI](https://github.com/glenkleidon/HL7Enumerator/wiki).

### Composing a PID Segment using DataTypes

```  
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
```






