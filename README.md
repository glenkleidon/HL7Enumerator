# HL7Enumerator
C# Library for parsing and enumerating HL7 Messages (with LINQ) 
## History
This project was started as a simple conversation with my colleague who thought that of the 
available C# HL7 parser libraries out there, none seem to be simple to use and were 
overly complex for the requirements.

We wanted to be able to perform actions such as
```
    HL7Message mesg = @"MSH|^~\&|CERNER||PriorityHealth||||ORU^R01|Q479004375T431430612|P|2.3|" + "\n" +
        ...

    string SendingSystem = mesg.Element("MSH.3");
    Console.WriteLine(string.Format("Message received from Sending system {0}", SendingSystem));

    Console.WriteLine("PD1 Segment    : " + mesg.Element("PD1"));
    Console.WriteLine("2nd OBX Segment: " + mesg.Element("OBX[2]"));

    //Now Locate the Test names in all the OBX records 
    var OBXTestNames = mesg.AllSegments("OBX").Select(o => o.Element("*.3.2").ToString());

    Console.WriteLine("Found Tests:");
    foreach (string obx in OBXTestNames) Console.WriteLine(string.Format("  {0}",obx));
    
    ... 
    
    ```


I agreed that the task should not be that hard to do.  And so the ball started rolling....

     
