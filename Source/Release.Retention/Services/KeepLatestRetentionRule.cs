using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Release.Retention.Domain.Model.Objects;
using Release.Retention.Domain.Model.Service.Contracts;

namespace Release.Retention.Services
{
    public class KeepLatestRetentionRule : IRetentionRule
    {
        public Task<IEnumerable<AppRelease>> SelectReleasesToKeepAsync(IEnumerable<AppRelease> releases, int numberOfReleasesToKeep = 1)
        {
            if (numberOfReleasesToKeep == 0 || releases == null || !releases.Any())
                return Task.FromResult(new List<AppRelease>() as IEnumerable<AppRelease>);

            var releaseList = releases.ToList();

            var latestReleases =
                releaseList.GroupBy(r => (r.ProjectId, r.Deployment.Environment.Id))
                           .SelectMany(grp =>
                                        grp.OrderByDescending(d => d.Deployment.DeployedAt)
                                           .Take(numberOfReleasesToKeep));

            return Task.FromResult(latestReleases);
        }
    }
}