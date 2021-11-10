using System;
using Release.Retention.Domain.Model.Objects;

namespace Release.Retention.Test.Facilities
{
    public class AppDeploymentBuilder
    {
        string Id { get; set; }
        string ReleaseId { get; set; }
        AppEnvironment Environment { get; set; }
        DateTime DeployedAt { get; set; }

        public AppDeploymentBuilder Start()
        {
            Id = Guid.NewGuid().ToString();
            ReleaseId = "r-1";
            Environment = new AppEnvironment("e-1", "e-1");
            DeployedAt = DateTime.Now;

            return this;
        }

        public AppDeploymentBuilder WithReleaseId(string releaseId)
        {
            ReleaseId = releaseId;
            return this;
        }

        public AppDeploymentBuilder WithDeployedAt(DateTime deployedAt)
        {
            DeployedAt = deployedAt;

            return this;
        }
        
        public AppDeploymentBuilder WithEnv(AppEnvironment env)
        {
            Environment = env;

            return this;
        }


        public AppDeployment Build()
        {
            return new AppDeployment(Id, ReleaseId, Environment, DeployedAt);
        }
    }
}