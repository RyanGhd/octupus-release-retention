using System.Collections.Generic;
using System.Threading.Tasks;
using Release.Retention.Domain.Model.Objects;

namespace Release.Retention.Domain.Model.Service.Contracts
{
    public interface IRetentionRule
    {
        public Task<IEnumerable<(AppRelease Release, string Reason)>> SelectReleasesToKeepAsync(IEnumerable<AppRelease> releases, int numberOfReleasesToKeep);
    }
}