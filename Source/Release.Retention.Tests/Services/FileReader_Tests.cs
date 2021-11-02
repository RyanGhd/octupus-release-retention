using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Xunit;
// ReSharper disable InconsistentNaming

namespace Release.Retention.Services
{
    public class FileReader_Tests
    {
        public static IEnumerable<object[]> Paths => new List<object[]>
        {
            new object[]{ Path.Combine("TestData", "Deployments.json")},
            new object[]{ Path.Combine("TestData", "Environments.json") },
            new object[]{ Path.Combine("TestData", "Releases.json") },
         };

        [Theory]
        [MemberData(nameof(FileReader_Tests.Paths), MemberType=typeof(FileReader_Tests))]
        public async Task Service_can_read_files(string path)
        {
            var sut = new FileReader();
            
            var result = await sut.ReadAsync(path);

            Assert.True(result.Length > 0);
        }
    }
}
