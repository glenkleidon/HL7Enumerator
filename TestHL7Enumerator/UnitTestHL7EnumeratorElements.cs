﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestHL7Enumerator
{
    [TestClass]
    public class UnitTestHL7EnumeratorElements
    {
        private const string MSH = @"MSH|^~\&|CERNER||PriorityHealth||||ORU^R01|Q479004375T431430612|P|2.3|" ;
        private const string PID = @"PID|||001677980||SMITH^CURTIS||19680219|M||||||||||929645156318|123456789|";
        private const string PD1 = @"PD1||||1234567890^LAST^FIRST^M^^^^^NPI|";
        private const string OBR = @"OBR|1|341856649^HNAM_ORDERID|000002006326002362|648088^Basic Metabolic Panel|||20061122151600|||||||||1620^Hooker^Robert^L~1624^Smith^Bill^R||||||20061122154733|||F|||||||||||20061122140000|";
        private const string OBX1 = @"OBX|1|NM|GLU^Glucose Lvl|59|mg/dL|65-99^65^99|L|||F|||20061122154733|";
        private const string OBX2 = @"OBX|2|NM|ALT^Alanine Aminotransferase|13|umol/L|2-20^65^1000|N|||F|||20061122154733|";

        private const string Example1 = MSH + "\n" + PID + "\n" + PD1 + "\n" + OBR + "\n" + OBX1 + "\n" + OBX2 + "\n";

        [TestMethod]
        public void TestHL7Enumerator_Element_Segment_Text_returns_Correct_segment()
        {
            HL7Enumerator.HL7Message msg = Example1;
            string segment = msg.Element("MSH");
            Assert.AreEqual(MSH, segment);

            string obx2 = msg.Element("OBX[2]");
            Assert.AreEqual(OBX2, obx2);
        }

        [TestMethod]
        public void TestHL7Enumerator_Element_Field_Text_returns_Correct_Field()
        {
            HL7Enumerator.HL7Message msg = Example1;
            string PIDfield = msg.Element("PID.5");
            Assert.AreEqual("SMITH^CURTIS", PIDfield);

            string OBX2field = msg.Element("OBX[2]/2");
            Assert.AreEqual("NM", OBX2field);
        }

        [TestMethod]
        public void TestHL7Enumerator_MSH_Element_Field_Text_returns_Correct_Field()
        {
            HL7Enumerator.HL7Message msg = Example1;
            string PIDfield = msg.Element("MSH.12");
            Assert.AreEqual("2.3", PIDfield);

            string OBX2field = msg.Element("MSH.3");
            Assert.AreEqual("CERNER", OBX2field);
        }

        [TestMethod]
        public void TestHL7Enumerator_Element_Field_Text_returns_Correct_Component()
        {
            HL7Enumerator.HL7Message msg = Example1;
            string PIDfield = msg.Element("PID/5/1");
            Assert.AreEqual("SMITH", PIDfield);

            string OBX2field = msg.Element("OBX[2].3.2");
            Assert.AreEqual("Alanine Aminotransferase", OBX2field);
        }

        [TestMethod]
        public void TestHL7Enumerator_MSH_Element_Field_Text_returns_Correct_Component()
        {
            HL7Enumerator.HL7Message msg = Example1;
            string PIDfield = msg.Element("MSH.12.2");
            Assert.AreEqual(string.Empty, PIDfield);

            string OBX2field = msg.Element("MSH.3.1");
            Assert.AreEqual("CERNER", OBX2field);
        }



    }
}
