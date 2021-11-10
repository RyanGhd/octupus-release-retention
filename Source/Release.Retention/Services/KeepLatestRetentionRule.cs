using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Release.Retention.Domain.Model.Objects;
using Release.Retention.Domain.Model.Service.Contracts;

namespace Release.Retention.Services
{
    public class KeepLatestRetentionRule : IRetentionRule
    {

        public async Task<IEnumerable<(AppRelease Release, string Reason)>> SelectReleasesToKeepAsync(IEnumerable<AppRelease> releases, int numberOfReleasesToKeep)
        {
            // check input
            if (numberOfReleasesToKeep == 0 || releases == null || !releases.Any())
                return new List<(AppRelease Release, string Reason)>();

            var releaseList = releases.ToList();

            // divide releases into buckets
            var buckets = await this.DivideReleasesIntoBucketsAsync(releaseList, numberOfReleasesToKeep);

            // consolidate buckets 
            var result = ConsolidateBuckets(buckets);

            return result;
        }

        private Task<Dictionary<string, AppReleaseBucket>> DivideReleasesIntoBucketsAsync(IEnumerable<AppRelease> releases, int numberOfReleasesToKeep)
        {
            var task = Task.Run(() =>
            {
                var buckets = new Dictionary<string, AppReleaseBucket>();

                foreach (var r in releases)
                {
                    foreach (var d in r.LatestDeploymentsPerEnvironment)
                    {
                        var key = $"{r.ProjectId}#{d.Environment.Id}";

                        if (buckets.TryGetValue(key, out AppReleaseBucket bucket))
                            buckets[key] = bucket.Add(r);
                        else
                        {
                            bucket = new AppReleaseBucket(r.ProjectId, d.Environment, numberOfReleasesToKeep);
                            bucket = bucket.Add(r);
                            buckets.Add(key, bucket);
                        }
                    }
                }

                return buckets;
            });

            return task;
        }

        private IEnumerable<(AppRelease Release, string Reason)> ConsolidateBuckets(Dictionary<string, AppReleaseBucket> buckets)
        {
            var result =
                buckets.Values.SelectMany(b => b.GetReleases())
                    .GroupBy(b => b.Release.Id)
                    .Select(g =>
                    {
                        if (g.Count() == 1)
                            return g.First();

                        return g.Aggregate((s, v) => (s.Release, $"{s.Reason}{Environment.NewLine}{v.Reason}"));
                    }).ToList();

            return result;
        }
    }
}