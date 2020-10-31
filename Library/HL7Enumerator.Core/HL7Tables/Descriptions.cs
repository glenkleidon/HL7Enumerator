using HL7Enumerator.HL7Tables.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;

namespace HL7Enumerator.Core.HL7Tables
{
    public class TableDetails : ITableDetails
    {
        public string Value { get; set; }
        public string ShortDescription { get; set; }
        public IEnumerable<string> Notes { get; set; }
    }
    public static class Extensions
    {
        public static IEnumerable<TableDetails> TableDetails(this Dictionary<string, string> table)
        {

            return table.Select(t => t.TableDetailsRow());
        }

        public static TableDetails TableDetailsRow(this KeyValuePair<string, string> row)
        {
            var notes = (row.Value!=null) ? row.Value.Replace("<br>", "\n").Split('\n') : null;
            return new TableDetails()
            {
                Value = row.Key,
                ShortDescription = notes?.FirstOrDefault(),
                Notes = notes?.Skip(1)
            };

        }
    }
}
