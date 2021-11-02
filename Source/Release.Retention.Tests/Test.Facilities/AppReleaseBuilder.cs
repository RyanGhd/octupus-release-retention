﻿using System;
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
        AppDeployment Deployment { get; set; }

        public AppReleaseBuilder()
        {

        }

        public AppReleaseBuilder Start()
        {
            Id = "r-1";
            ProjectId = "proj-1";
            Version = "v1";
            Created = DateTime.Now.AddDays(-10);

            Deployment = new AppDeployment("d1", "r-1", new ReleaseEnvironment("env-1", "env one"),
                DateTime.Now.AddDays(-9));

            return this;
        }

        public AppReleaseBuilder WithReleaseCreated(DateTime created)
        {
            Created = created;
            return this;
        }

        public AppReleaseBuilder WithDeploymentCreated(DateTime created)
        {
            Deployment = new AppDeployment(Deployment.Id, Deployment.ReleaseId, Deployment.Environment, created);

            return this;
        }
      
        public AppRelease Build()
        {
            return new AppRelease(Id, ProjectId, Version, Created, Deployment);
        }
    }
}