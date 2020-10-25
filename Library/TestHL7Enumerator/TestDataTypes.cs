using HL7Enumerator;
using HL7Enumerator.Types;
using System;
using System.Globalization;
using System.Linq;
using Xunit;

namespace TestHL7Enumerator
{
    public class TestDataTypes
    {
        private const string MSH = "MSH|^~\\&|MEDIPATH1|1191|EPIC|1191^EPIC^L|20200827103131+1000||ORU^R01|20200827.95359193|P|2.4|11445703||AL|NE|AUS|ASCII||\r";
        private const string PID1 = "PID|1||8003608166690501^^^AUZIC&UID&UIDTYPE^NI~3071D2942H^^^AUSLINK^AN~QPCA9034W^^^AUSDVA^DVG~3071D2942H^^^AUSLINK^HC~307111942HR^^^AUSLINK^PEN~307111942HR^^^AUSLINK^SEN~2428778132^^^AUSHIC&211&300^MC^RMH^20140430^2020||PARTELLO VASQUES^Avita^KatharineMaria^^NSP||20140430+1000|F|||119 SMITH STREET^^SWANSEE^NSW^2281^AUS^H^^^^^20140430121110+1000&20200131121008+1100~27 MANN AVENUE^^PATTERSON LAKES^NSW^3197^AUS^H^^^^^20150530121110+1000||^PRN^PH^^^04^22713163|||||||||||||||||";
        private const string PID2 = "PID|1||8003608166690501^01^IDCODE^AUSIC&UID&UIDTYPE^NI^RMH^20200701000000+1000^20250701000000+1000~3071D2942H^^^AUSLINK^AN~QPCA9034W^^^AUSDVA^DVG~3071D2942H^^^AUSLINK^HC~307111942HR^^^AUSLINK^PEN~307111942HR^^^AUSLINK^SEN~2428778132^^^AUSHIC&211&300^MC^RMH^20140430^2020||PARTELLO VASQUES^Avita^KatharineMaria^^NSP||20140430+1000|F|||119 SMITH STREET^^SWANSEE^NSW^2281^AUS^H^^^^^20140430121110+1000&20200131121008+1100~27 MANN AVENUE^^PATTERSON LAKES^NSW^3197^AUS^H^^^^^20150530121110+1000||^PRN^PH^^^04^22713163|||||||||||||||||";
        private const string OBR1 = @"OBR|1|341856649^HNAM_ORDERID|000002006326002362|648088^Basic Metabolic Panel|||20061122151600|||||||||0191324T^MCINTYRE^ANDREW^^^^^^AUSHICPR^L^^^UPIN||||||20061122154733|||F|||||||||||20061122140000|" + "\r";
        private const string OBR2 = @"OBR|2|341856649^HNAM_ORDERID|000002006326002362|648088^Basic Metabolic Panel|||20061122151600|||||||||0191324T^MCINTYRE^ANDREW^^^^^^AUSHICPR^L^^^UPIN~1620^McIntyre^Andrew^L||||||20061122154733|||F|||||||||||20061122140000|" + "\r";
        private const string OBX1 = @"OBX|1|ED|ClinicalPDFReport1^Clinical PDF Report MR077065T-1^^PDF^Display format in PDF^AUSPDI" +
          @"||MIA^Image^PDF^Base64^JVBERi0xLjQKJeLjz9MKMyAwIG9iaiA8PC9MZW5n\X0D\\X0A\" +
          @"        dGggMTYyMC9GaWx0ZXIvRmxhdGVEZWNvZGU+PnN0\X0D\\X0A\" +
          @"        cmVhbQp4nK2Y227aShSG7/0U6zKVEpijD0j7whiT\X0D\\X0A\" +
          @"        TLexqe0kjdpe0MRJqAi0QFr17ff4bGMaD9V2FTE1\X0D\\X0A\" +
          @"        nm/9s04z5oeGwLAIvMhPBCtNx6T6TO+vtGftVltr\X0D\\X0A\" +
          @"        GH5pBN7Lp75pGMFM+/QFwYP2I5uGYPukMYTBYIYk\X0D\\X0A\" +
          @"        MUSz0SofEZaNcHOUffusPVb3XzRulU/kI1LNIq35\X0D\\X0A\" +
          @"        1azMVvZsRstHvLLAO7a4hapZpJpFCgvZt/WomkXk\X0D\\X0A\" +
          @"        yvNFhpcaMYAbuiTocmXpaKVF2U3GypssFZmPUlj2||||||F" + "\r";

        private const string MSH2 = @"MSH|^~\&|CERNER||PriorityHealth||||ORU^R01|Q479004375T431430612|P|2.3|";
        private const string PID3 = @"PID|||001677980||SMITH^CURTIS||19680219|M||||||||||929645156318|123456789|";
        private const string PD1 = @"PD1||||1234567890^LAST^FIRST^M^^^^^NPI|";
        private const string OBR3 = @"OBR|1|341856649^HNAM_ORDERID|000002006326002362|648088^Basic Metabolic Panel|||20061122151600|||||||||1620^Hooker^Robert^L~1624^Smith^Bill^R||||||20061122154733|||F|||||||||||20061122140000|";
        private const string OBR4 = @"OBR|1|341856649^HNAM_ORDERID|000002006326002362|648088^Basic Metabolic Panel|||20061122151600|||||||||1620^Hooker^Robert^L||||||20061122154733|||F|||||||||||20061122140000|";
        private const string OBX3 = @"OBX|1|NM|GLU^Glucose Lvl||59|mg/dL|65-99^65^99|L|||F|||20061122154733|";
        private const string OBX4 = @"OBX|2|NM|ALT^Alanine Aminotransferase||13|umol/L|2-20^65^1000|N|||F|||20061122154733|";
        private const string OBR5 = @"OBR|3||3BE7CECB-AD59-4C46-B711-5F6E137C2890^1234^ACME Pathology^7654^AUSNATA|84907-5^Colorectal Cancer Structured Pathology Report^LN|||20161102+1100|" +
                                    @"||||||||||||LN=3BE7CECB-AD59-4C46-B711-5F6E137C2890||201611021450+1100||SP|F||^^^20161102+1100 ||||^Insurance Claim^^INS^Reason:Insurance^Local~INV1^Acute Disorder^";

        /*
                private const string MSHA = @"MSH|^~\&|MERIDIAN|Demo Server|||20100202163120+1100||ORU^R01|XX02021630854-1539|P|2.3.1^AUS&&ISO^AS4700.2&&L|||||AUS|ASCII~8859/1";
                private const string MSHB = @"MSH|^~\&|MERIDIAN|Section1\\3\&4|||20100202163120+1100||ORU^R01|XX02021630854-1539|P|2.3.1^AUS&&ISO^AS4700.2&&L|||||AUS|ASCII~8859/1";
                private const string PIDA = @"PID|1||||SMITH^Jessica^^^^^L||19700201|F|||1 Test Street^^WODEN^ACT^2606^AUS^C~2 Test Street^^WODEN^ACT^2606^AUS^C";
                private const string PIDB = @"PID|1||||SMITH^Jessica^^^^^L||19700201|F|||CRN 1st \& 2nd Test Street^^WODEN^ACT^2606^AUS^C~2 Test Street^^WODEN^ACT^2606^AUS^C";
        */
        private const string Example1 = MSH2 + "\r" + PID3 + "\r" + PD1 + "\r" + OBR3 + "\r" + OBX3 + "\r" + OBX4 + "\r";
        /*
              //  private const string Example2 = MSHA + "\r" + PIDA + "\r" + PD1 + "\r" + OBR3 + "\r" + OBX3 + "\r" + OBX4 + "\r";
              //  private const string Example3 = MSHB + "\r" + PIDB + "\r" + PD1 + "\r" + OBR3 + "\r" + OBX3 + "\r" + OBX4 + "\r";
        */
        [Fact]
        public void ElementSegmentTextReturnsCorrectSegment()
        {
            HL7Enumerator.HL7Message msg = Example1;
            string segment = msg.Element("MSH");
            Assert.Equal(MSH2, segment);

            string obx4 = msg.Element("OBX[2]");
            Assert.Equal(OBX4, obx4);
        }



        [Theory]
        [InlineData("1994", "1994")]
        [InlineData("199411", "1994-11")]
        [InlineData("19941101", "1994-11-01")]
        [InlineData("199411011", "1994-11-01")]
        [InlineData("1994123011", "1994-12-30T11")]
        [InlineData("19941230110", "1994-12-30T11")]
        [InlineData("199412301105", "1994-12-30T11:05")]
        [InlineData("1994123011055", "1994-12-30T11:05")]
        [InlineData("19941230110559", "1994-12-30T11:05:59")]
        [InlineData("19941230110559+1000", "1994-12-30T11:05:59+10:00")]
        [InlineData("19941230110559.", "1994-12-30T11:05:59")]
        [InlineData("19941230110559.1", "1994-12-30T11:05:59.1")]
        [InlineData("19941230110559.12", "1994-12-30T11:05:59.12")]
        [InlineData("19941230110559.12+0000", "1994-12-30T11:05:59.12Z")]
        public void ShouldConvertDateTextToISODate(string input, string expected)
        {
            var result = DataTypes.HL7DateTextAsISODateText(input);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("1994", "19940101")]
        [InlineData("199411", "19941101")]
        [InlineData("19941230", "19941230")]
        public void ShouldConvertDate(string input, string expected)
        {
            var result = DataTypes.ToLocalDateTime(input);
            Assert.Equal(expected, result.Value.ToString("yyyyMMdd"));
        }
        [Theory]
        [InlineData("200101011022", true)]
        [InlineData("200101011022+0900", false)]
        public void ShouldIndicateLocalTime(string input, bool isLocal)
        {
            var dt = DataTypes.AsDateTime(input);
            var localDt = DataTypes.ToLocalDateTime(input);
            var utcDt = DataTypes.ToUTCDateTime(input);
            var dtText = dt.Value.ToString("o");
            var compareText = (isLocal) ? localDt.Value.ToString("o") : utcDt.Value.ToString("o");
            Assert.Equal(dtText, compareText);
        }

        [Theory]
        [InlineData("20200707101010", "20200707 10:10:10")]      //Offset not specified so Local Time
        [InlineData("20200707101010+1000", "20200707 10:10:10")] //Local Time
        [InlineData("20201207101010+1100", "20201207 10:10:10")] //Local Time during daylight saving
        [InlineData("20201018235010-0400", "20201019 14:50:10")] //NewYork -4 as 2020-10-19
        [InlineData("20201018041010-0000", "20201018 15:10:10")] //GMT
        [InlineData("20201019121510+0800", "20201019 15:15:10")] //Perth +8 as 2020-10-19
        public void ShouldConvertToLocalDateTime(string input, string expected)
        {
            var result = DataTypes.ToLocalDateTime(input);
            Assert.Equal(expected, result.Value.ToString("yyyyMMdd HH:mm:ss"));
        }
        [Fact]
        public void ShouldConvertDateRange()
        {
            var txt = MSH + PID1;


            var message = new HL7Message(txt);

            var firstAddress = message.Element($"PID.11[1]");
            var firstFrom = firstAddress[11].AsDateRange().DateFrom;
            var firstTo = firstAddress[11].AsDateRange().DateTo;
            var secondAddress = message.Element($"PID.11[2]");
            var secondFrom = secondAddress[11].AsDateRange().DateFrom;
            var secondTo = secondAddress[11].AsDateRange().DateTo;
            Assert.Equal(new DateTime(2014, 04, 30, 12, 11, 10), firstFrom.Value);
            Assert.Equal(new DateTime(2020, 01, 31, 12, 10, 08), firstTo.Value);
            Assert.Equal(new DateTime(2015, 5, 30, 12, 11, 10), secondFrom.Value);
            Assert.False(secondTo.HasValue);
        }
        [Fact]
        public void ShouldConvertIdentifier()
        {
            HL7Message message = MSH + PID2;
            var firstIdentifier = message.Element($"PID.3[1]");
            var hl7Id = firstIdentifier.AsCX();
            Assert.Equal("01", hl7Id.CheckDigit);
            Assert.Equal("IDCODE", hl7Id.CheckDigitScheme.Value);
            Assert.Equal("AUSIC", hl7Id.AssigningAuthority.NamespaceId.BestValue);
            Assert.Equal("NI", hl7Id.IdentifierTypeCode);
            Assert.Equal("RMH", hl7Id.AssigningFacility.NamespaceId.BestValue);
            Assert.Equal(new DateTime(2020, 07, 01).ToUniversalTime(), hl7Id.EffectiveDate.Value.ToUniversalTime());
            Assert.Equal(new DateTime(2025, 07, 01).ToUniversalTime(), hl7Id.ExpirationDate.Value.ToUniversalTime());
        }

        [Fact]
        public void ShouldConvertMultipleIdentifiers()
        {
            HL7Message message = MSH + PID2;
            var firstIdentifier = message.Element($"PID.3[]");
            var hl7Ids = firstIdentifier.AsCXs();
            var id1 = hl7Ids.FirstOrDefault();
            var id2 = hl7Ids.Skip(1).FirstOrDefault();
            var id5 = hl7Ids.Skip(4).FirstOrDefault();
            /*
             * 8003608166690501^01^IDCODE^AUSIC&UID&UIDTYPE^NI^RMH^20200701000000+1000^20250701000000+1000
             * 3071D2942H^^^AUSLINK^AN
             * QPCA9034W^^^AUSDVA^DVG
             * 3071D2942H^^^AUSLINK^HC
             * 307111942HR^^^AUSLINK^PEN
             * 307111942HR^^^AUSLINK^SEN
             * 2428778132^^^AUSHIC&211&300^MC^RMH^20140430^2020
             */
            Assert.Equal("8003608166690501", id1.ID);
            Assert.Equal("01", id1.CheckDigit);
            Assert.Equal("IDCODE", id1.CheckDigitScheme.BestValue);
            Assert.Equal("AUSIC", id1.AssigningAuthority.NamespaceId.BestValue);
            Assert.Equal("NI", id1.IdentifierTypeCode);
            Assert.Equal("RMH", id1.AssigningFacility.NamespaceId.BestValue);
            Assert.Equal(new DateTime(2020, 07, 01).ToUniversalTime(), id1.EffectiveDate.Value.ToUniversalTime());
            Assert.Equal(new DateTime(2025, 07, 01).ToUniversalTime(), id1.ExpirationDate.Value.ToUniversalTime());

            Assert.Equal("3071D2942H", id2.ID);
            Assert.Equal("", id2.CheckDigitScheme.BestValue);
            Assert.Equal("AUSLINK", id2.AssigningAuthority.NamespaceId.BestValue);
            Assert.Equal("AN", id2.IdentifierTypeCode);
            Assert.Null(id2.AssigningFacility);

            Assert.Equal("307111942HR", id5.ID);
            Assert.Equal("AUSLINK", id5.AssigningAuthority.NamespaceId.BestValue);
            Assert.Equal("PEN", id5.IdentifierTypeCode);
        }

        [Fact]
        public void ShouldConvertED()
        {
            var txt = MSH + PID2 + OBR1 + OBX1;

            var message = new HL7Message(txt);
            var OBXResult = message.Element("OBX[1].5");
            var hl7Ed = OBXResult.AsED();
            //MIA^Image^PDF^Base64
            Assert.Equal("MIA", hl7Ed.SourceApplication.NamespaceId.BestValue);
            Assert.Equal("Image", hl7Ed.TypeOfData.BestValue);
            Assert.Equal("PDF", hl7Ed.DataSybType.BestValue);
            Assert.Equal("Base64", hl7Ed.Encoding.BestValue);
            Assert.StartsWith("JVBERi0xLjQKJeLjz9", hl7Ed.Data, StringComparison.InvariantCultureIgnoreCase);
        }
        [Fact]
        public void ShouldConvertASingleCE()
        {
            var message = new HL7Message(OBX1);
            var OBXResult = message.Element("OBX[1].3");
            var hl7CE = OBXResult.AsCE();

            //ClinicalPDFReport1^Clinical PDF Report MR077065T-1^^PDF^Display format in PDF^AUSPDI

            Assert.Equal("ClinicalPDFReport1", hl7CE.Identifier);
            Assert.Equal("Clinical PDF Report MR077065T-1", hl7CE.Text);
            Assert.Equal("", hl7CE.NameOfCodingSystem.Value);
            Assert.Equal("PDF", hl7CE.AlternateIdentifier);
            Assert.Equal("Display format in PDF", hl7CE.AlternateText);
            Assert.Equal("AUSPDI", hl7CE.NameOfAlternateCodingSystem.Value);

        }
        [Fact]
        public void ShouldConvertMultipleCEs()
        {
            var message = new HL7Message(OBR5);
            var reasonsForStudy = message.Element("OBR[1].31[]");
            var hl7CEs = reasonsForStudy.AsCEs();
            var ce1 = hl7CEs.FirstOrDefault();
            var ce2 = hl7CEs.Skip(1).FirstOrDefault();
            //^Insurance Claim^^INS^Reason:Insurance^Local~INV1^Acute Disorder^

            Assert.Equal("", ce1.Identifier);
            Assert.Equal("Insurance Claim", ce1.Text);
            Assert.Equal("", ce1.NameOfCodingSystem.Value);
            Assert.Equal("INS", ce1.AlternateIdentifier);
            Assert.Equal("Reason:Insurance", ce1.AlternateText);
            Assert.Equal("Local", ce1.NameOfAlternateCodingSystem.Value);

            Assert.Equal("INV1", ce2.Identifier);
            Assert.Equal("Acute Disorder", ce2.Text);
            Assert.Equal("", ce2.NameOfCodingSystem.Value);
            Assert.Equal("", ce2.AlternateIdentifier);
            Assert.Equal("", ce2.AlternateText);
            Assert.Equal("", ce2.NameOfAlternateCodingSystem.Value);
        }


        [Fact]
        public void ShouldConvertFromASingleXCNElement()
        {
            HL7Message mesg = OBR1;
            var xcn1 = mesg.Element("OBR.16").AsXCN();
            Assert.Equal("MCINTYRE", xcn1.FamilyName);
        }
        [Fact(Skip ="To Do")]
        public void ShouldPopulateXCNHDUsingTables()
        {
            // XCN_ExtendedCompositeIDAndName
            throw new NotImplementedException();
        }
        [Fact]
        public void ShouldThrowExceptionForRepeatingXCNElement()
        {
            HL7Message mesg = OBR2;
            var obr = mesg.Element("OBR[1]");
            var xcns = obr.IndexedElement(16);
            Assert.Throws<InvalidOperationException>( ()=> xcns.AsXCN());
        }
        [Fact(Skip ="To Do")]
        public void ShouldConvertAnXAD()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void ShouldConvertToAIEnumerableForRepeatingXCNElements()
        {
            HL7Message mesg = OBR2;
            var obr = mesg.Element("OBR[1]");
            var xcns = obr.IndexedElement(16);
            var xcnList = xcns.AsXCNs();
            Assert.Equal(2,xcnList.Count());
            Assert.Equal("MCINTYRE", xcnList.FirstOrDefault().FamilyName);
            Assert.Equal("McIntyre", xcnList.Skip(1).FirstOrDefault().FamilyName);
            Assert.Equal("1620", xcnList.Skip(1).FirstOrDefault().ID);
        }

        [Fact]
        public void ShouldConvertToAIEnumerableForOneXCNElement()
        {
            HL7Message mesg = OBR4;
            var obr = mesg.Element("OBR[1]");
            var xcns = obr.IndexedElement(16);
            var xcnList = xcns.AsXCNs();
            Assert.Single(xcnList);
            Assert.Equal("Hooker", xcnList.FirstOrDefault().FamilyName);
            Assert.Equal("1620", xcnList.FirstOrDefault().ID);
        }

        [Fact]
        public void ShouldConvertFromARepeatingXCNElement()
        {
            HL7Message mesg = OBR2;
            var obr = mesg.Element("OBR[1]");
            var xcns = obr.IndexedElement(16);
            var xcn1 = xcns[0].AsXCN();
            
            Assert.Equal("MCINTYRE", xcn1.FamilyName);
            Assert.Equal("0191324T", xcn1.ID);
            Assert.Equal("ANDREW", xcn1.GivenName);
            Assert.Empty(xcn1.SecondGivenNamesOrInitials);
            Assert.Equal("AUSHICPR", xcn1.AssigningAuthority.NamespaceId.BestValue);
            Assert.Equal("L", xcn1.NameTypeCode.BestValue);
            Assert.Equal("UPIN", xcn1.IdentifierTypeCode);
            var xcn2 = xcns[1].AsXCN();
            Assert.Equal("1620", xcn2.ID);
            Assert.Equal("McIntyre", xcn2.FamilyName);
           
        }

        [Fact]
        public void ShouldExtractNumbersFromNM()
        {
            HL7Message mesg = OBX3;
            var obx = mesg.Element("OBX[1]");
            var valueElement = obx.IndexedElement(5);
            DataTypes.NM_Number number = valueElement;
            Assert.True(number.IsNumber);
            Assert.Equal(59, (int)number);
            Assert.Equal((Int64)59, number.AsInt64());
            Assert.Equal((float)59, number.AsFloat());
            Assert.Equal(59M, number.AsDecimal());
            Assert.Equal(59.0D, (double)number);
           
        }
        [Fact]
        public void ShouldAssignNumbersToNM()
        {
            var stringNumber = new DataTypes.NM_Number("21");
            var intNumber = new DataTypes.NM_Number(212);
            var int64Number = new DataTypes.NM_Number((Int64)2100000000003);
            var floatNumber = new DataTypes.NM_Number(21.4);
            var doubleNumber = new DataTypes.NM_Number(21.0000000005D);
            var decimalNumber = new DataTypes.NM_Number(21.6666M);

            Assert.True(stringNumber.IsNumber);
            Assert.True(intNumber.IsNumber);
            Assert.True(int64Number.IsNumber);
            Assert.True(floatNumber.IsNumber);
            Assert.True(doubleNumber.IsNumber);
            Assert.True(decimalNumber.IsNumber);

            Assert.Equal(212, intNumber.AsInteger());
            Assert.Equal(2100000000003, int64Number.AsInt64());
            Assert.True(floatNumber.AsFloat()- 21.4 < 0.01);
            Assert.Equal(212,  intNumber.AsFloat());
            Assert.Equal(21.0000000005D, doubleNumber.AsDouble());
            Assert.Equal(21.6666M, decimalNumber.AsDecimal());
            Assert.Equal(212, intNumber.AsInt64());

            Assert.Equal(21, stringNumber.AsInteger());
            Assert.Equal(21, stringNumber.AsInt64());
            Assert.Equal(21.0, stringNumber.AsFloat());
            Assert.Equal(21.0D, stringNumber.AsDouble());
            Assert.Equal(21.0M, stringNumber.AsDecimal());

            Assert.Equal(int.MinValue,decimalNumber.AsInteger());

        }

    }
}

