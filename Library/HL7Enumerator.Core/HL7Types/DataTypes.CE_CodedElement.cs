using HL7Enumerator.HL7Tables.Interfaces;
using HL7Enumerator.Types.Interfaces;
using System.Collections.Generic;

namespace HL7Enumerator.Types
{
    public static partial class DataTypes
    {
        public class CE_CodedElement : HL7TypeBase, IHL7Type
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
            public CE_CodedElement(HL7Element element, IEnumerable<string> tableIds = null, IDataTableProvider tables = null)
                : base(element, tableIds, tables)
            {
            }

            public override void Populate(HL7Element element, IEnumerable<string> tableIds = null)
            {
                var tablesUsed = 0;
                Identifier = element.ElementValue(0);

                Text = element.ElementValue(1);

                NameOfCodingSystem = NewIS(
                    element.ElementValue(2), NextTableId(tableIds, ref tablesUsed));

                AlternateIdentifier = element.ElementValue(3);

                AlternateText = element.ElementValue(4);

                NameOfAlternateCodingSystem = NewIS(
                    element.ElementValue(5), NextTableId(tableIds, ref tablesUsed));

            }

            public string Identifier { get; set; }
            public string Text { get; set; }
            public IS_CodedValue NameOfCodingSystem { get; set; }
            public string AlternateIdentifier { get; set; }
            public string AlternateText { get; set; }
            public IS_CodedValue NameOfAlternateCodingSystem { get; set; }

            public static int TablesRequired => 2; // two ISs.

            public int DataTablesRequired => TablesRequired;

            IDataTableProvider IHL7Type.Tables { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

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

