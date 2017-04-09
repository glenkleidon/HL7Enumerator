# HL7Enumerator
C# Library for enumerating (Using LINQ) HL7 Messages
## History
This project was started as a simple conversation with my colleague who thought that of the avaialable C# HL7 parser libraries
out there, none seem to be simple to use and were overly complex for the requirements.

We wanted to be able to perform actions such as
```
     HL7Message mesg = @"MSH|^~\&|CERNER||PriorityHealth||||ORU^R01|Q479004375T...
     string SendingSystem = mesg.Element("MSH.3");
     console.writeline(format("Sending system {0}",SendingSystem);
     
     // Now LINQ query...
     var AbnormalOBXs = mesg.AllSegments( o = > o.Element("OBX.8").startsWith("A"));
```
I agreed that the task should not be that hard to do.  And so the ball started rolling....

     
