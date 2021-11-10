using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Release.Retention.Domain.Model.Objects;
using Xunit;

// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

namespace Release.Retention.Services
{
    public class KeepLatestRetentionRule_Tests
    {
        private readonly KeepLatestRetentionRuleTestFixture fixture = new KeepLatestRetentionRuleTestFixture();

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public async Task Service_returns_an_empty_list_if_the_input_is_null_or_empty(int numberOfReleasesToKeep)
        {
            var sut = new KeepLatestRetentionRule();

            var result1 = await sut.SelectReleasesToKeepAsync(null, numberOfReleasesToKeep);
            var result2 = await sut.SelectReleasesToKeepAsync(new List<AppRelease>(), numberOfReleasesToKeep);

            Assert.Empty(result1);
            Assert.Empty(result2);
        }

        [Fact]
        public async Task Service_returns_an_empty_list_if_number_of_releases_to_keep_is_zero()
        {
            // arrange 
            var data = new List<AppRelease>
            {
                fixture.DataBuilder.Start().Build(),
                fixture.DataBuilder.Start().Build(),
                fixture.DataBuilder.Start().Build(),
                fixture.DataBuilder.Start().Build()
            };

            var sut = new KeepLatestRetentionRule();

            // act
            var result = await sut.SelectReleasesToKeepAsync(data, 0);

            // assert
            Assert.Empty(result);
        }

        [Theory]
        [MemberData(nameof(KeepLatestRetentionRuleTestFixture.ReleaseCreatedDates), MemberType = typeof(KeepLatestRetentionRuleTestFixture))]
        public async Task
            Service_chooses_the_latest_deployed_releases_if_releases_belong_to_the_same_project_and_same_environment_regardless_of_release_created_date(DateTime firstCreatedDate, DateTime secondCreatedDate, int numberOfReleasesToKeep)
        {
            // arrange
            var dt = new DateTime(2000, 2, 1);

            var r1 = fixture.DataBuilder.Start().WithReleaseCreated(firstCreatedDate).WithDeployedAt(dt).Build();
            var r2 = fixture.DataBuilder.Start().WithReleaseCreated(secondCreatedDate).WithDeployedAt(dt.AddDays(1)).Build();
            var r3 = fixture.DataBuilder.Start().WithReleaseCreated(firstCreatedDate).WithDeployedAt(dt.AddDays(2)).Build();
            var r4 = fixture.DataBuilder.Start().WithReleaseCreated(secondCreatedDate).WithDeployedAt(dt.AddDays(3)).Build();

            var releases = new List<AppRelease> { r1, r2, r3, r4 };

            var sut = new KeepLatestRetentionRule();

            // act
            var result = await sut.SelectReleasesToKeepAsync(releases, numberOfReleasesToKeep);

            var resultList = result.ToList();

            // assert
            releases.Reverse();
            var expected = releases.Take(numberOfReleasesToKeep).ToList();

            expected.ForEach(e =>
            {
                var (release, reason) = resultList.FirstOrDefault(r => r.Release.Id.Equals(e.Id));
                Assert.NotNull(release);
                Assert.True(reason.Equals(string.Format(ReasonMessage.MessageFormat, e.LatestDeploymentsPerEnvironment.First().Environment.Id)));
            });

            Assert.True(resultList.Count() == numberOfReleasesToKeep);
        }

        [Theory]
        [MemberData(nameof(KeepLatestRetentionRuleTestFixture.ReleaseCreatedDates), MemberType = typeof(KeepLatestRetentionRuleTestFixture))]
        public async Task
            Service_chooses_the_latest_deployed_releases_for_each_project_if_deployments_belong_to_the_same_environment_regardless_of_release_created_date(DateTime firstCreatedDate, DateTime secondCreatedDate, int numberOfReleasesToKeep)
        {
            // arrange 
            var dt = new DateTime(2000, 2, 1);
            var e1 = new AppEnvironment("e-1", "e-1");

            var r1 = fixture.DataBuilder.Start().WithReleaseCreated(firstCreatedDate).WithProjectId("p1").WithEnvironment(e1,e1,e1).WithDeployedAt(dt.AddDays(1)).Build();
            var r2 = fixture.DataBuilder.Start().WithReleaseCreated(secondCreatedDate).WithProjectId("p1").WithEnvironment(e1, e1, e1).WithDeployedAt(dt.AddDays(2)).Build();
            var r3 = fixture.DataBuilder.Start().WithReleaseCreated(firstCreatedDate).WithProjectId("p1").WithEnvironment(e1, e1, e1).WithDeployedAt(dt.AddDays(3)).Build();

            var r4 = fixture.DataBuilder.Start().WithReleaseCreated(firstCreatedDate).WithProjectId("p2").WithEnvironment(e1, e1, e1).WithDeployedAt(dt.AddDays(1)).Build();
            var r5 = fixture.DataBuilder.Start().WithReleaseCreated(secondCreatedDate).WithProjectId("p2").WithEnvironment(e1, e1, e1).WithDeployedAt(dt.AddDays(2)).Build();
            var r6 = fixture.DataBuilder.Start().WithReleaseCreated(secondCreatedDate).WithProjectId("p2").WithEnvironment(e1, e1, e1).WithDeployedAt(dt.AddDays(3)).Build();

            var releases = new List<AppRelease> { r1, r2, r3, r4, r5, r6 };

            var sut = new KeepLatestRetentionRule();

            // act
            var result = await sut.SelectReleasesToKeepAsync(releases, numberOfReleasesToKeep);

            var resultList = result.ToList();

            // assert
            var expected = new List<AppRelease> { r3, r6 };
            if (numberOfReleasesToKeep == 2)
                expected.AddRange(new[] { r2, r5 });

            expected.ForEach(e =>
            {
                var (release, reason) = resultList.FirstOrDefault(r => r.Release.Id.Equals(e.Id));
                Assert.NotNull(release);
                Assert.True(reason.Equals(string.Format(ReasonMessage.MessageFormat, e.LatestDeploymentsPerEnvironment.First().Environment.Id)));
            });

            Assert.True(resultList.Count == expected.Count);
        }

        [Theory]
        [MemberData(nameof(KeepLatestRetentionRuleTestFixture.ReleaseCreatedDates), MemberType = typeof(KeepLatestRetentionRuleTestFixture))]
        public async Task
            Service_chooses_the_latest_deployed_releases_for_each_environment_if_deployments_belong_to_the_same_project_regardless_of_release_created_date(DateTime firstCreatedDate, DateTime secondCreatedDate, int numberOfDeploymentsToKeep)
        {
            // arrange 
            var dt = new DateTime(2000, 2, 1);
            var env1 = new AppEnvironment("1", "one");
            var env2 = new AppEnvironment("2", "two");
            var env3 = new AppEnvironment("3", "three");

            var r1 = fixture.DataBuilder.Start().WithReleaseCreated(firstCreatedDate).WithProjectId("p1").WithEnvironment(env1, env1, env1).WithDeployedAt(dt.AddDays(1)).Build();
            var r2 = fixture.DataBuilder.Start().WithReleaseCreated(secondCreatedDate).WithProjectId("p1").WithEnvironment(env2, env2, env2).WithDeployedAt(dt.AddDays(2)).Build();
            var r3 = fixture.DataBuilder.Start().WithReleaseCreated(firstCreatedDate).WithProjectId("p1").WithEnvironment(env3, env3, env3).WithDeployedAt(dt).Build();

            var r4 = fixture.DataBuilder.Start().WithReleaseCreated(firstCreatedDate).WithProjectId("p2").WithEnvironment(env1, env1, env1).WithDeployedAt(dt.AddDays(1)).Build();
            var r5 = fixture.DataBuilder.Start().WithReleaseCreated(secondCreatedDate).WithProjectId("p2").WithEnvironment(env2, env2, env2).WithDeployedAt(dt.AddDays(2)).Build();
            var r6 = fixture.DataBuilder.Start().WithReleaseCreated(firstCreatedDate).WithProjectId("p2").WithEnvironment(env3, env3, env3).WithDeployedAt(dt).Build();

            var releases = new List<AppRelease> { r1, r2, r3, r4, r5, r6 };

            var sut = new KeepLatestRetentionRule();

            // act
            var result = await sut.SelectReleasesToKeepAsync(releases, numberOfDeploymentsToKeep);

            var resultList = result.ToList();

            // assert
            releases.ForEach(e =>
            {
                var (release, reason) = resultList.FirstOrDefault(r => r.Release.Id.Equals(e.Id));
                Assert.NotNull(release);
                Assert.True(reason.Equals(string.Format(ReasonMessage.MessageFormat, e.LatestDeploymentsPerEnvironment.First().Environment.Id)));
            });

            Assert.True(resultList.Count == releases.Count);
        }

        [Fact]
        public async Task
            Service_merge_the_reasons_if_one_release_is_kept_due_to_being_the_lastest_in_multiple_environments()
        {
            // arrange 
            var dt = new DateTime(2000, 2, 1);
            var env1 = new AppEnvironment("1", "one");
            var env2 = new AppEnvironment("2", "two");
            var env3 = new AppEnvironment("3", "three");

            var r1 = fixture.DataBuilder.Start().WithEnvironment(env1, env2, env3).Build();

            var sut = new KeepLatestRetentionRule();

            var reason =  @$"kept because it was the most recently deployed to 1{Environment.NewLine}kept because it was the most recently deployed to 2{Environment.NewLine}kept because it was the most recently deployed to 3";

            // act
            var result = await sut.SelectReleasesToKeepAsync(new []{r1}, 1);

            var resultList = result.ToList();

            // assert 
            Assert.True(resultList.Count==1);
            Assert.Equal(resultList[0].Reason,reason);
        }
    }
}
