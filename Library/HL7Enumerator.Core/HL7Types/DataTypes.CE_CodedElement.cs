using System.Collections.Generic;

namespace HL7Enumerator.Types
{
    public static partial class DataTypes
    {
        public class CE_CodedElement
        {
            public CE_CodedElement()
            {

            }
            /// <summary>
            /// Populates a CE CodedElement from a HL7Element accepting Coding Table names
            /// NOTE: the HL7Tables must be pre-populated with the required table names
            /// prior to calling the method.
            /// </summary>
            /// <param name="element">HL7 Element</param>
            /// <param name="tableIds">The list of required table names For IS and ID data types
            /// in the order they appear in the element. If All values use the same Table, it may 
            /// be added once.</param>
            public CE_CodedElement(HL7Element element, IEnumerable<string> tableIds=null)
            {
                
                Identifier = element.ElementValue(0);
                
                Text = element.ElementValue(1);

                var idIndex = 0;
                NameOfCodingSystem = new IS_CodedValue(
                    element.ElementValue(2), NextTableId(tableIds,ref idIndex));

                AlternateIdentifier = element.ElementValue(3);

                AlternateText = element.ElementValue(4);

                NameOfAlternateCodingSystem = new IS_CodedValue( 
                    element.ElementValue(5), NextTableId(tableIds, ref idIndex));
            }

            public string Identifier { get; set; }
            public string Text { get; set; }
            public IS_CodedValue NameOfCodingSystem { get; set; }
            public string AlternateIdentifier { get; set; }
            public string AlternateText { get; set; }
            public IS_CodedValue NameOfAlternateCodingSystem { get; set; }
            public override string ToString()
            {
                return ToString('^');
            }
            public virtual string ToString(char sepatator)
            { 
                return $"{Identifier}{sepatator}{Text}{sepatator}{NameOfCodingSystem.BestValue}" +
                    $"{sepatator}{AlternateIdentifier}{sepatator}{AlternateText}" +
                    $"{sepatator}{NameOfAlternateCodingSystem.BestValue}";
            }
        }

    }
}

