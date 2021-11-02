using System.Collections.Generic;
using System.Threading.Tasks;

namespace Release.Retention.Domain.Model.Service.Contracts
{
    public interface IRetentionRule
    {
        public Task<IEnumerable<Objects.AppRelease>> SelectReleasesToKeepAsync(IEnumerable<Objects.AppRelease> releases);
    }
}