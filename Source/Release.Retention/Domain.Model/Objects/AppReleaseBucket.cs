using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace Release.Retention.Domain.Model.Objects
{
    public class AppReleaseBucket : ICloneable
    {
        private SortedDictionary<string, (AppRelease Release, AppDeployment LastDeployedToCurrentEnvironemnt)> _innerBucket;
        private (AppRelease Release, DateTime LastDeployedToCurrentEnvironemnt) _innerInstance;

        public string ProjectId { get; }
        public AppEnvironment Environment { get; }
        public int NumberOfReleasesToKeep { get; }

        public AppReleaseBucket(string projectId, AppEnvironment environment, int numberOfReleasesToKeep)
        {
            ProjectId = projectId;
            Environment = environment;
            NumberOfReleasesToKeep = numberOfReleasesToKeep;
        }

        public AppReleaseBucket Add(AppRelease appRelease)
        {
            if (appRelease == null || !appRelease.ProjectId.Equals(this.ProjectId, StringComparison.InvariantCultureIgnoreCase))
                return this;

            var latestDeployed = appRelease.LatestDeploymentsPerEnvironment.FirstOrDefault(d =>
                d.Environment.Id.Equals(this.Environment.Id, StringComparison.InvariantCultureIgnoreCase));

            if (latestDeployed == null) // means the release hasn't been deployed to the relevant environment. Bucket ignores it in this case.
                return this;

            if (NumberOfReleasesToKeep == 1)
                return AddWhenKeepingOneRelease(appRelease, latestDeployed);

            return AddWhenKeepingMultipleReleases(appRelease, latestDeployed);
        }

        private AppReleaseBucket AddWhenKeepingOneRelease(AppRelease appRelease, AppDeployment latestDeployed)
        {
            if (_innerInstance.Release == null)
            {
                var cloned = (AppReleaseBucket)this.Clone();
                cloned._innerInstance = (appRelease, latestDeployed.DeployedAt);
                return cloned;
            }
            else if (_innerInstance.LastDeployedToCurrentEnvironemnt < latestDeployed.DeployedAt)
            {
                var cloned = (AppReleaseBucket)this.Clone();
                cloned._innerInstance = (appRelease, latestDeployed.DeployedAt);
                return cloned;
            }

            return this; // no change to the current instance
        }

        private AppReleaseBucket AddWhenKeepingMultipleReleases(AppRelease appRelease, AppDeployment latestDeployed)
        {
            var key = $"{latestDeployed.DeployedAt.Ticks}#{latestDeployed.Id}"; // important note: it's required to add deploymentId to this key because there's a chance to separate deployments happen at exact same time. Having deploymentId in the key guarantees that the dic keys are not duplicate

            var cloned = (AppReleaseBucket)this.Clone();

            if (_innerBucket == null)
                cloned._innerBucket =
                    new SortedDictionary<string, (AppRelease Release, AppDeployment LastDeployedToCurrentEnvironemnt
                        )>(); // important note: innerBucket should be instantiated here to improve performance. It will not be created if bucket has to take care of one instance only.
            else
                cloned._innerBucket = new SortedDictionary<string, (AppRelease Release, AppDeployment LastDeployedToCurrentEnvironemnt)>(_innerBucket);

            cloned._innerBucket.Add(key, (appRelease, latestDeployed));

            // keep removing the first item of the bucket until the bucket size matches with number of releases that we want to keep. 
            // the bucket is sortedDic and the items are sorted in ascending order based on deployment time to environment. Removing the first item means removing the oldest from the bucket
            while (cloned._innerBucket.Count > NumberOfReleasesToKeep)
            {
                var deployment = cloned._innerBucket.First().Value.LastDeployedToCurrentEnvironemnt;
                var keyToRemove = key = $"{deployment.DeployedAt.Ticks}#{deployment.Id}";
                cloned._innerBucket.Remove(keyToRemove);
            }

            return cloned;
        }

        public IEnumerable<(AppRelease Release, string Reason)> GetReleases()
        {
            if (NumberOfReleasesToKeep == 1)
            {
                if (_innerInstance.Release == null)
                    return new List<(AppRelease Release, string Reason)>();

                return new List<(AppRelease Release, string Reason)> { (_innerInstance.Release, string.Format(ReasonMessage.MessageFormat, Environment.Id)) };
            }

            if (_innerBucket == null || !_innerBucket.Any())
                return new List<(AppRelease Release, string Reason)>();

            return _innerBucket.Values.Select(item =>
                (item.Release, string.Format(ReasonMessage.MessageFormat, Environment.Id)));
        }

        public object Clone()
        {
            return new AppReleaseBucket(ProjectId, Environment, NumberOfReleasesToKeep);
        }
    }
}
