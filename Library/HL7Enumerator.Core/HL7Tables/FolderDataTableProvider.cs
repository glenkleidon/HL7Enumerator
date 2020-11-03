using HL7Enumerator.HL7Tables.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HL7Enumerator.HL7Tables
{
    public class FolderDataTableProvider : IDataTableProvider
    {
        private string fileExnt = ".txt";
        private InMemoryDataTableProvider cachedProvider = null;

        public string Folder { get; set; }
        public string FileExtension
        {
            get
            {
                return fileExnt;
            }
            set
            {
                fileExnt = (value != null && value.StartsWith(".")) ? value : fileExnt = $".{value}";
            }
        }
        public void AddCodeTable(string tableId, Dictionary<string, string> table)
        {
            if (table == null || string.IsNullOrWhiteSpace(tableId) || Folder == null) return;
            if (!Directory.Exists(Folder)) Directory.CreateDirectory(Folder);
            var fileName = TableFileName(tableId);
            if (File.Exists(fileName)) throw new IOException($"Table {tableId} exists");
            SaveCodeTable(table, fileName);
        }

        private static void SaveCodeTable(Dictionary<string, string> table, string fileName)
        {
            var builder = new StringBuilder();
            foreach (var keyPair in table)
            {
                builder.AppendLine($"{keyPair.Key}={keyPair.Value.Replace("\n", "<br>").Replace("\r", "")}");
            }
            File.WriteAllText(fileName, builder.ToString(), Encoding.UTF8);
        }

        public string TableFileName(string tableId)
        {
            return Path.Combine(Folder, $"{tableId}{FileExtension}");
        }

        public Dictionary<string, string> GetCodeTable(string tableId)
        {
            var tbl = cachedProvider?.GetCodeTable(tableId);
            if (tbl != null) return tbl;

            if (string.IsNullOrWhiteSpace(tableId) || Folder == null) return null;
            var fileName = TableFileName(tableId);
            if (!File.Exists(fileName)) return null;

            tbl = LoadCodeTable(fileName);
            cachedProvider?.AddCodeTable(tableId, tbl);
            return tbl;
        }

        private static Dictionary<string, string> LoadCodeTable(string fileName)
        {
            Dictionary<string, string> tbl;
            var tableContents = File.ReadAllText(fileName, Encoding.UTF8).Replace("\r", "").Split('\n');
            tbl = new Dictionary<string, string>();
            foreach (var line in tableContents)
            {
                var keyPair = line.Split('=');
                if (keyPair.Length > 0) tbl.Add(keyPair[0], (keyPair.Length > 1) ? keyPair[1].Replace("<br>","\n") : "");
            }

            return tbl;
        }

        public void Clear(string tableId = null)
        {
            if (tableId != null)
            {
                File.Delete(TableFileName(tableId));
                cachedProvider?.Clear(tableId);
            }
        }

        public string GetTableValue(string tableId, string key)
        {
            var tbl = GetCodeTable(tableId);
            return (tbl?.ContainsKey(key) == true) ? tbl[key] : "";
        }

        public FolderDataTableProvider()
        {
        }
        public FolderDataTableProvider(string folder, bool cached =true)
        {
            if (cached) this.cachedProvider = new InMemoryDataTableProvider();
            this.Folder = folder;
        }
    }
}
