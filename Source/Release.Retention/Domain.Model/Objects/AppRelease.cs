using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Release.Retention.Domain.Model.Objects
{
    public class AppRelease
    {

        public string Id { get; private set; }
        public string ProjectId { get; private set; }
        public string Version { get; private set; }
        public DateTime Created { get; private set; }
        public IEnumerable<AppDeployment> LatestDeploymentsPerEnvironment { get; private set; }

        public AppRelease(string id, string projectId, string version, DateTime created, IEnumerable<AppDeployment> deployments)
        {
            Id = id;
            ProjectId = projectId;
            Version = version;
            Created = created;
            LatestDeploymentsPerEnvironment = deployments == null
                ? ImmutableList<AppDeployment>.Empty
                : ImmutableList<AppDeployment>.Empty.AddRange(GetLatestDeploymentsPerEnvironment(deployments));
        }

        private IEnumerable<AppDeployment> GetLatestDeploymentsPerEnvironment(IEnumerable<AppDeployment> deployments)
        {
            var deploymentList = deployments.ToList();

            if (!deploymentList.Any())
                return deploymentList;

            var latestDeployments =
                deploymentList.GroupBy(d => d.Environment.Id)
                              .Select(g => g.OrderByDescending(v => v.DeployedAt).First());

            return latestDeployments;
        }
    }
}