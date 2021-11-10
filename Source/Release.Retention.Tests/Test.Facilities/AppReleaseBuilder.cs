using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using Release.Retention.Domain.Model.Objects;

namespace Release.Retention.Test.Facilities
{
    public class AppReleaseBuilder
    {
        string Id { get; set; }
        string ProjectId { get; set; }
        string Version { get; set; }
        DateTime Created { get; set; }
        List<AppDeployment> Deployments { get; set; }

        public AppReleaseBuilder()
        {

        }

        public AppReleaseBuilder Start()
        {
            Id = Guid.NewGuid().ToString();
            ProjectId = "proj-1";
            Version = "v1";
            Created = DateTime.Now.AddDays(-10);

            var env = new AppEnvironment("env-1", "env one");

            Deployments = new List<AppDeployment>
            {
                new AppDeployment("d-1", "r-1", env, DateTime.Now.AddDays(-7)),
                new AppDeployment("d-2", "r-1", env, DateTime.Now.AddDays(-8)),
                new AppDeployment("d-3", "r-1", env, DateTime.Now.AddDays(-9)),
            };

            return this;
        }

        public AppReleaseBuilder WithReleaseCreated(DateTime created)
        {
            Created = created;
            return this;
        }

        public AppReleaseBuilder WithDeployedAt(params DateTime[] deployedAt)
        {
            for (int i = 0; i < deployedAt.Length && i < Deployments.Count; i++)
            {
                Deployments[i] = new AppDeployment(Deployments[i].Id, Deployments[i].ReleaseId, Deployments[i].Environment, deployedAt[i]);
            }

            return this;
        }

        public AppReleaseBuilder WithProjectId(string projectId)
        {
            ProjectId = projectId;
            return this;
        }

        public AppReleaseBuilder WithNoDeployment()
        {
            Deployments = null;
            return this;
        }

        public AppReleaseBuilder WithEnvironment(params AppEnvironment[] environment)
        {
            for (int i = 0; i < environment.Length && i < Deployments.Count; i++)
            {
                Deployments[i] = new AppDeployment(Deployments[i].Id, Deployments[i].ReleaseId, environment[i], Deployments[i].DeployedAt);
            }

            return this;
        }

        public AppRelease Build()
        {
            return new AppRelease(Id, ProjectId, Version, Created, Deployments);
        }
    }
}