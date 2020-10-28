using HL7Enumerator.HL7Tables;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace TestHL7Enumerator
{
    public class TestFolderDataTableProvider : IDisposable
    {
        private string testFolder;
        private FolderDataTableProvider sut;
        private FolderDataTableProvider sutNoCache;
        private bool disposed;

        private Dictionary<string, string> AddressTypesInstance()
        {
            var addrTypes0190 = new Dictionary<string, string>();
            addrTypes0190.Add("C", "Current Address");
            addrTypes0190.Add("H", "Home Address");
            addrTypes0190.Add("M", "Mailing Address");
            return addrTypes0190;
        }

        private Dictionary<string, string> TestTypesInstance()
        {
            var testTypes = new Dictionary<string, string>();
            testTypes.Add("test1", "one");
            testTypes.Add("test2", "two");
            testTypes.Add("test3", "three");
            return testTypes;
        }


        public TestFolderDataTableProvider()
        {
            testFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            sut = new FolderDataTableProvider(testFolder);
            sutNoCache = new FolderDataTableProvider(testFolder, false);
        }

        [Fact]
        public void AnNonExistentFolderShouldReturnNullForAnyTable()
        {
            Assert.Null(sut.GetCodeTable("2222"));
            Assert.Null(sut.GetCodeTable(""));
            Assert.Null(sut.GetCodeTable("001\\000"));
        }
        [Fact]
        public void AddingATableShouldCreateAFolderAndFile()
        {
            var tbl = new Dictionary<string, string>();
            tbl.Add("test", "file");
            sut.AddCodeTable("0000", tbl);
            Assert.True(Directory.Exists(Path.GetDirectoryName(sut.TableFileName("0000"))));
            Assert.True(File.Exists(sut.TableFileName("0000")));
        }

        [Fact]
        public void AnAddedTableShouldBeReadable()
        {
            sut.AddCodeTable("0190", AddressTypesInstance());
            Assert.NotNull(sut.GetCodeTable("0190"));
            Assert.Equal("Mailing Address", sut.GetTableValue("0190", "M"));
            Assert.Equal(string.Empty, sut.GetTableValue("0190", "Q"));
        }

        [Fact]
        public void CachedBehaviourWorksAsExpectedWhenFileIsCleared()
        {
            sut.AddCodeTable("0190", AddressTypesInstance());
            sut.AddCodeTable("test", TestTypesInstance());
            Assert.Equal("Mailing Address", sut.GetTableValue("0190", "M")); // cache table.
            Assert.Equal("one", sut.GetTableValue("test", "test1")); // cache table.

            Assert.Equal("Home Address", sutNoCache.GetTableValue("0190", "H"));
            Assert.Equal("two", sutNoCache.GetTableValue("test","test2"));

            // delete the file using the NON cached version, so only the cached version should work.
            sutNoCache.Clear("0190");

            Assert.Equal("Home Address", sut.GetTableValue("0190", "H"));
            Assert.Equal(String.Empty, sutNoCache.GetTableValue("0190", "M"));
            Assert.Equal("three", sutNoCache.GetTableValue("test", "test3"));

            // delete the TEST file using the cached version, neither Test should now work, but 0190 will still work on cached.
            sut.Clear("test");
            Assert.Equal(String.Empty, sutNoCache.GetTableValue("0190", "M"));
            Assert.Equal(String.Empty, sutNoCache.GetTableValue("test", "test3"));
            Assert.Equal(String.Empty, sut.GetTableValue("test", "test3"));
            Assert.Equal("Home Address", sut.GetTableValue("0190", "H"));
            Assert.Equal("Current Address", sut.GetTableValue("0190", "C"));
        }


        #region IDisposable implementation
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (Directory.Exists(testFolder)) Directory.Delete(testFolder, true);
                disposed = true;
            }
        }
        ~TestFolderDataTableProvider()
        {
            Dispose(false);
        }
        #endregion
    }
}
