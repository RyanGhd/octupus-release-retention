using System;

namespace Release.Retention.Data.Entities
{
    public class AppDeploymentEntity
    {
        public string Id { get; set; }
        public string ReleaseId { get; set; }
        public string EnvironmentId { get; set; }
        public DateTime DeployedAt { get; set; }
    }
}