using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Release.Retention.Domain.Model.Service.Contracts;

namespace Release.Retention.Services
{
    public class FileReader : IFileReader
    {
        public async Task<string> ReadAsync(string path)
        {
            using (var sr = new StreamReader(path))
            {
                return await sr.ReadToEndAsync();
            }
        }
    }
}
