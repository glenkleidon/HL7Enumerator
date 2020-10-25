﻿using System;
using System.Collections.Generic;

namespace HL7Enumerator.Types
{
    public static partial class DataTypes
    {
        public class CX_CompositeId
        {
            public CX_CompositeId()
            {

            }
            public CX_CompositeId(HL7Element element, string checkDigitTableId=null)
            {
                ID = element.ElementValue(0);
                CheckDigit = element.ElementValue(1);
                CheckDigitScheme = new ID_CodedValue(element.ElementValue(2), checkDigitTableId);
                AssigningAuthority = element.AsHD(3);
                IdentifierTypeCode = element.ElementValue(4);
                AssigningFacility = element.AsHD(5);
                EffectiveDate = element.FromTS(6);
                ExpirationDate = element.FromTS(7);
            }
            public string ID { get; set; }
            public string CheckDigit { get; set; }
            public ID_CodedValue CheckDigitScheme { get; set; }
            public HD_HierarchicDesignator AssigningAuthority { get; set; }
            public string IdentifierTypeCode { get; set; }
            public HD_HierarchicDesignator AssigningFacility { get; set; }
            public DateTime? EffectiveDate { get; set; }
            public DateTime? ExpirationDate { get; set; }
            public override string ToString()
            {
                return ToString('^');
            }
            public string ToString(char separator)
            {
                var ns = NextSeparator(separator);
                return
                    $"{ID}{separator}{CheckDigit}{separator}{CheckDigitScheme.BestValue}{separator}" +
                    $"{AssigningAuthority.ToString(ns)}{separator}{AssigningFacility.ToString(ns)}" +
                    $"{EffectiveDate?.AsDTLocal()}{separator}{ExpirationDate?.AsDTLocal()}";
            }
        }

    }
}

