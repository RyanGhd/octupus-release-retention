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
        private KeepLatestRetentionRuleTestFixture fixture = new KeepLatestRetentionRuleTestFixture();

        [Fact]
        public async Task Service_returns_empty_list_if_the_input_is_null_or_empty()
        {
            var sut = new KeepLatestRetentionRule();

            var result = await sut.SelectReleasesToKeepAsync(null);

            Assert.Empty(result);
        }

        [Theory]
        [MemberData(nameof(KeepLatestRetentionRuleTestFixture.ReleaseCreatedDates), MemberType = typeof(KeepLatestRetentionRuleTestFixture))]
        public async Task
            Service_chooses_the_latest_deployed_release_if_releases_belong_to_the_same_project_same_environment_regardlress_of_release_created_date(DateTime firstCreatedDate, DateTime secondCreatedDate)
        {
            // arrange 
            var first = fixture.DataBuilder.Start()
                                                    .WithReleaseCreated(firstCreatedDate)
                                                    .WithDeploymentCreated(DateTime.Now.AddDays(-4))
                                                    .Build();

            var second = fixture.DataBuilder.Start()
                                                     .WithReleaseCreated(secondCreatedDate)
                                                     .WithDeploymentCreated(DateTime.Now.AddDays(-5))
                                                     .Build();

            var releases = new List<AppRelease> { first, second };

            var sut = new KeepLatestRetentionRule();

            // act
            var result = await sut.SelectReleasesToKeepAsync(releases);

            var resultList = result.ToList();

            // assert
            Assert.Contains(resultList, r => r.Id.Equals(first.Id));
            Assert.True(resultList.Count() == 1);
        }
    }
}
