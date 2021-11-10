using System;
using System.Linq;
using Release.Retention.Test.Facilities;
using Xunit;

namespace Release.Retention.Domain.Model.Objects
{
    public class AppReleaseBucket_Tests
    {
        private readonly AppReleaseBuilder _builder = new AppReleaseBuilder();

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void Object_returns_an_empty_result_if_no_release_is_added(int numberOfReleasesToKeep)
        {
            var sut = new AppReleaseBucket("p-1", new AppEnvironment("e-1", "e-1"), numberOfReleasesToKeep);

            var result = sut.GetReleases();

            Assert.True(result.Count() == 0);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void
            Object_does_not_keep_the_release_if_it_does_not_belong_to_the_same_project(int numberOfReleaseToKeep)
        {
            // arrange
            var e1 = new AppEnvironment("1", "1");

            var r1 = _builder.Start().WithProjectId("p-1").WithEnvironment(e1).Build();

            var sut = new AppReleaseBucket("p-2", e1, numberOfReleaseToKeep);

            // act
            sut.Add(r1);

            var result = sut.GetReleases();

            // assert
            Assert.Empty(result);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void
            Object_does_not_throw_error_if_null_release_is_added(int numberOfReleaseToKeep)
        {
            var sut = new AppReleaseBucket("p-1", new AppEnvironment("1", "1"), numberOfReleaseToKeep);

            sut.Add(null);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void
            Object_does_not_throw_error_if_release_is_not_deployed_at_all(int numberOfReleaseToKeep)
        {
            var r = _builder.Start().WithNoDeployment().Build();

            var sut = new AppReleaseBucket("p-1", new AppEnvironment("1", "1"), numberOfReleaseToKeep);

            sut.Add(r);

            var result = sut.GetReleases();

            Assert.Empty(result);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void
            Object_does_not_keep_the_release_if_it_is_not_deployed_to_the_environment_that_the_bucket_is_created_for(int numberOfReleaseToKeep)
        {
            // arrange
            var e1 = new AppEnvironment("1", "1");
            var e2 = new AppEnvironment("2", "2");

            var r1 = _builder.Start().WithEnvironment(e1).Build();

            var sut = new AppReleaseBucket(r1.ProjectId, e2, numberOfReleaseToKeep);

            // act
            sut.Add(r1);

            var result = sut.GetReleases();

            // assert
            Assert.Empty(result);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void Object_keeps_the_newest_app_releases_for_the_same_projects_and_same_environments(int numberOfReleasesToKeep)
        {
            // arrange 
            var dt = DateTime.Now;

            var r1 = _builder.Start().WithDeployedAt(dt.AddDays(-5), dt.AddDays(-5), dt.AddDays(-5)).Build();
            var r2 = _builder.Start().WithDeployedAt(dt.AddDays(-4), dt.AddDays(-4), dt.AddDays(-4)).Build();
            var r3 = _builder.Start().WithDeployedAt(dt.AddDays(-3), dt.AddDays(-3), dt.AddDays(-3)).Build();
            var r4 = _builder.Start().WithDeployedAt(dt.AddDays(-6), dt.AddDays(-6), dt.AddDays(-6)).Build();

            var sut = new AppReleaseBucket(r1.ProjectId, r1.LatestDeploymentsPerEnvironment.First().Environment, numberOfReleasesToKeep);

            // act
            sut = sut.Add(r1).Add(r2).Add(r3).Add(r4);

            var result = sut.GetReleases().ToList();

            // assert
            Assert.True(result.Count == numberOfReleasesToKeep);

            var r3Result = result.FirstOrDefault(r => r.Release.Id.Equals(r3.Id));
            Assert.NotNull(r3Result.Release);
            Assert.True(r3Result.Reason.Equals(string.Format(ReasonMessage.MessageFormat, r3.LatestDeploymentsPerEnvironment.First().Environment.Id)));

            if (numberOfReleasesToKeep > 1)
            {
                var r2Result = result.FirstOrDefault(r => r.Release.Id.Equals(r2.Id));
                Assert.NotNull(r2Result.Release);
                Assert.True(r2Result.Reason.Equals(string.Format(ReasonMessage.MessageFormat, r2.LatestDeploymentsPerEnvironment.First().Environment.Id)));
            }

            if (numberOfReleasesToKeep > 2)
            {
                var r1Result = result.FirstOrDefault(r => r.Release.Id.Equals(r1.Id));
                Assert.NotNull(r1Result.Release);
                Assert.True(r1Result.Reason.Equals(string.Format(ReasonMessage.MessageFormat, r1.LatestDeploymentsPerEnvironment.First().Environment.Id)));
            }
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void Object_keeps_the_newest_app_releases_for_the_same_projects_and_same_environments_if_the_releases_are_deployed_to_multiple_environments(int numberOfReleasesToKeep)
        {
            // arrange 
            var dt = DateTime.Now;

            var e1 = new AppEnvironment("1", "1");
            var e2 = new AppEnvironment("2", "2");
            var e3 = new AppEnvironment("3", "3");

            var r1 = _builder.Start().WithEnvironment(e1, e2, e3).WithDeployedAt(dt.AddDays(-5), dt.AddDays(-5), dt.AddDays(-5)).Build();
            var r2 = _builder.Start().WithEnvironment(e1, e2, e3).WithDeployedAt(dt.AddDays(-4), dt.AddDays(-4), dt.AddDays(-4)).Build();
            var r3 = _builder.Start().WithEnvironment(e1, e2, e3).WithDeployedAt(dt.AddDays(-3), dt.AddDays(-3), dt.AddDays(-3)).Build();
            var r4 = _builder.Start().WithEnvironment(e1, e2, e3).WithDeployedAt(dt.AddDays(-6), dt.AddDays(-6), dt.AddDays(-6)).Build();

            var sut = new AppReleaseBucket(r1.ProjectId, e1, numberOfReleasesToKeep);
            var sut2 = new AppReleaseBucket(r1.ProjectId, e2, numberOfReleasesToKeep);
            // act
            sut = sut.Add(r1).Add(r2).Add(r3).Add(r4);
            sut2 = sut2.Add(r1).Add(r2).Add(r3).Add(r4);

            var result = sut.GetReleases().ToList();

            // assert
            Assert.True(result.Count == numberOfReleasesToKeep);

            var r3Result = result.FirstOrDefault(r => r.Release.Id.Equals(r3.Id));
            Assert.NotNull(r3Result.Release);
            Assert.True(r3Result.Reason.Equals(string.Format(ReasonMessage.MessageFormat, e1.Id)));

            if (numberOfReleasesToKeep > 1)
            {
                var r2Result = result.FirstOrDefault(r => r.Release.Id.Equals(r2.Id));
                Assert.NotNull(r2Result.Release);
                Assert.True(r2Result.Reason.Equals(string.Format(ReasonMessage.MessageFormat, e1.Id)));
            }

            if (numberOfReleasesToKeep > 2)
            {
                var r1Result = result.FirstOrDefault(r => r.Release.Id.Equals(r1.Id));
                Assert.NotNull(r1Result.Release);
                Assert.True(r1Result.Reason.Equals(string.Format(ReasonMessage.MessageFormat, e1.Id)));
            }
        }
    }
}