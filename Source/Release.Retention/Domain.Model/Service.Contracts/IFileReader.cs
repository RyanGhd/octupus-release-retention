using System.Reflection;
using System.Threading.Tasks;

namespace Release.Retention.Domain.Model.Service.Contracts
{
    public interface IFileReader
    {
        Task<string> ReadAsync(string path);
    }
}