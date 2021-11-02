using System;

namespace Release.Retention.Domain.Model.Objects
{
    public class AppRelease
    {
        public string Id { get; private set; }
        public string ProjectId { get; private set; }
        public string Version { get; private set; }
        public DateTime Created { get; private set; }
        public AppDeployment Deployment { get; private set; }

        public AppRelease(string id, string projectId, string version, DateTime created, AppDeployment deployment)
        {
            Id = id;
            ProjectId = projectId;
            Version = version;
            Created = created;
            Deployment = deployment;
        }
    }
}