﻿using System;
using Xunit;

namespace TestHL7Enumerator
{
    
    public class TestHL7EnumeratorParseOnly
    {
        private const string MSH = @"MSH|^~\&|CERNER||PriorityHealth||||ORU^R01|Q479004375T431430612|P|2.3|";
        private const string PID = @"PID|||001677980||SMITH^CURTIS||19680219|M||||||||||929645156318|123456789|";
        private const string PD1 = @"PD1||||1234567890^LAST^FIRST^M^^^^^NPI|";
        private const string OBR = @"OBR|1|341856649^HNAM_ORDERID|000002006326002362|648088^Basic Metabolic Panel|||20061122151600|||||||||1620^Hooker^Robert^L~1624^Smith^Bill^R||||||20061122154733|||F|||||||||||20061122140000|";
        private const string OBX1 = @"OBX|1|NM|GLU^Glucose Lvl&Blood Sugar|59|mg/dL|65-99^65^99|L|||F|||20061122154733|";
        private const string OBX2 = @"OBX|2|NM|ALT^Alanine Aminotransferase&Liver ALT|13|umol/L|2-20^65^1000|N|||F|||20061122154733|";

        private const string Example1 = MSH + "\r" + PID + "\r" + PD1 + "\r" + OBR + "\r" + OBX1 + "\r" + OBX2 + "\r";


        [Fact]
        public void TestHL7Enumerator_ParseOnly_Null_String_Returns_Empty_string()
        {
            Assert.Throws<ArgumentException>(
                () =>
                {
                    HL7Enumerator.HL7Element segment = HL7Enumerator.HL7Message.ParseOnly("", "MSH.9");
                    Assert.Equal(string.Empty, "" + segment);
                }
            );
        }

        [Fact]
        public void TestHL7Enumerator_ParseOnly_Msg_Returns_correct_field()
        {
            HL7Enumerator.HL7Element field = HL7Enumerator.HL7Message.ParseOnly(Example1, "MSH.9");
            Console.WriteLine(string.Format("'{0}'", field));
            Assert.Equal("ORU^R01", "" + field);
            field = HL7Enumerator.HL7Message.ParseOnly(Example1, "PD1.4");
            Assert.Equal("1234567890^LAST^FIRST^M^^^^^NPI", "" + field);
        }

        [Fact]
        public void TestHL7Enumerator_ParseOnly_Msg_Returns_correct_component()
        {
            HL7Enumerator.HL7Element Component = HL7Enumerator.HL7Message.ParseOnly(Example1, "MSH.9.1");
            Assert.Equal("ORU", "" + Component);
            Component = HL7Enumerator.HL7Message.ParseOnly(Example1, "MSH.9.2");
            Assert.Equal("R01", "" + Component);
            Component = HL7Enumerator.HL7Message.ParseOnly(Example1, "PD1.4.1");
            Assert.Equal("1234567890", "" + Component);
            Component = HL7Enumerator.HL7Message.ParseOnly(Example1, "PD1.4.3");
            Assert.Equal("FIRST", "" + Component);
        }
        [Fact]
        public void TestHL7Enumerator_ParseOnly_Msg_Returns_segement_Repitition()
        {
            var field = HL7Enumerator.HL7Message.ParseOnly(Example1, "OBX.3");
            Assert.Equal("GLU^Glucose Lvl&Blood Sugar", "" + field);
            field = HL7Enumerator.HL7Message.ParseOnly(Example1, "OBX[2].3");
            Assert.Equal("ALT^Alanine Aminotransferase&Liver ALT", "" + field);
            field = HL7Enumerator.HL7Message.ParseOnly(Example1, "OBX[3].3");
            Assert.Equal(string.Empty, "" + field);
        }

        [Fact]
        public void TestHL7Enumerator_ParseOnly_Msg_Returns_correct_subcomponent()
        {
            var subComponent = HL7Enumerator.HL7Message.ParseOnly(Example1, "OBX.3.2");
            Assert.Equal("Glucose Lvl&Blood Sugar", "" + subComponent);
            subComponent = HL7Enumerator.HL7Message.ParseOnly(Example1, "OBX.3.2.2");
            Assert.Equal("Blood Sugar", "" + subComponent);
            subComponent = HL7Enumerator.HL7Message.ParseOnly(Example1, "OBX.3.2.3");
            Assert.Equal("", "" + subComponent);
        }
    }
}
