using HL7Enumerator;
using HL7Enumerator.HL7Tables;
using HL7Enumerator.HL7Tables.Interfaces;
using HL7Enumerator.Types;
using System;
using System.Collections.Generic;
using Xunit;

namespace TestHL7Enumerator
{
    public class TestHL7Tables
    {
        IDataTableProvider tables = new InMemoryDataTableProvider();
        private const string testSeg = "TTT|1|";
        private const string addr1 = @"119 SMITH STREET^^SWANSEE^NSW^2281^AUS^H^^^^^20140430121110+1000&20200131121008+1100";
        private const string addr2 = @"27 MANN AVENUE^^PATTERSON LAKES^NSW^3197^AUS^H^^^^^20150530121110+1000";

        private const string test1 = testSeg + addr1;
        private const string test2 = testSeg + addr2;

        private void AddTables()
        {
            // Add the Address Type Table 0190
            
            var addrTypes0190 = new Dictionary<string, string>();
            addrTypes0190.Add("C", "Current Address");
            addrTypes0190.Add("H", "Home Address");
            addrTypes0190.Add("M", "Mailing Address");

            var country0399 = new Dictionary<string, string>();
            country0399.Add("AUS", "Australia");
            country0399.Add("NZL", "New Zealand");
            country0399.Add("PCN", "Pitcairn");
            country0399.Add("PNG", "Papua New Guinea");


            tables.AddCodeTable("0190", addrTypes0190);
            tables.AddCodeTable("0399", country0399);
        }
        public TestHL7Tables()
        {
            AddTables();
        }

        [Fact]
        public void ShouldConvertXADs()
        {
            //we need 5 altogether, but we dont have those defined, so we dont want to repeat.
            var tbleIds = new List<String>() { "0399","0190", null,null,null };

            HL7Message mesg = test1; 
            var address = mesg.Element("TTT[1].2").AsXAD(tbleIds, tables);
            Assert.Equal("0399",address.Country.TableId);
            Assert.Equal("0190", address.AddressType.TableId);
            Assert.Equal("H", address.AddressType.CodedValue);
            Assert.Equal("H", address.AddressType.Value);
            Assert.Equal("H", address.AddressType.BestValue);
            Assert.Equal("AUS", address.Country.CodedValue);
            Assert.Equal("AUS", address.Country.Value);
            Assert.Equal("AUS", address.Country.BestValue);

        }
        [Fact]
        public void ShouldReturnNullForFaultType()
        {
            var tbleIds = new List<String>() { "0399", "0190", null, null, null };

            HL7Message mesg = test1;
            var address = mesg.Element("TTT[1].2").AsXAD(tbleIds, tables);
            // force value to be wrong.
            address.AddressType.Value = "Q";
            address.Country.Value = "ZZZ";
            Assert.Equal("0399", address.Country.TableId);
            Assert.Equal("0190", address.AddressType.TableId);
            Assert.Equal("", address.AddressType.CodedValue);
            Assert.Equal("Q", address.AddressType.Value);
            Assert.Equal("Q", address.AddressType.BestValue);
            Assert.Equal("", address.Country.CodedValue);
            Assert.Equal("ZZZ", address.Country.Value);
            Assert.Equal("ZZZ", address.Country.BestValue);

        }
    }
}
