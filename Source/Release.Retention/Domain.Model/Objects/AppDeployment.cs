using System;

namespace Release.Retention.Domain.Model.Objects
{
    public class AppDeployment
    {
        public string Id { get; private set; }
        public string ReleaseId { get; private set; }
        public AppEnvironment Environment { get; private set; }
        public DateTime DeployedAt { get; private set; }

        public AppDeployment(string id, string releaseId, AppEnvironment environment, DateTime deployedAt)
        {
            Id = id;
            ReleaseId = releaseId;
            Environment = environment;
            DeployedAt = deployedAt;
        }
    }
}