// ReSharper disable InconsistentNaming

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Release.Retention.Data.Entities;
using Release.Retention.Domain.Model.Objects;
using Release.Retention.Services;
using Xunit;

namespace Release.Retention.Data
{
    public class AppReleaseJsonDataLoader_Tests
    {
        private readonly AppReleaseJsonDataLoaderTestFixture _fixture = new AppReleaseJsonDataLoaderTestFixture();

        [Fact]
        public async Task Service_can_load_app_releases_from_files()
        {
            // arrange 
            var sut = 
                _fixture.Start()
                        .WithFileReaderSetupForReleases(_fixture.GetReleasesString())
                        .WithFileReaderSetupForDeployments(_fixture.GetDeploymentsString())
                        .WithFileReaderSetupForEnvironments(_fixture.GetEnvironmentsString())
                        .Build();

            // act
            var result = (IDictionary<string, AppRelease>) (await sut.LoadAsync()).ToDictionary(r=>r.Id);

            // assert
            Assert.True(result.Count==2);
            Assert.Contains("Release-1", result);
            Assert.Contains("Release-2", result);

            foreach (var (releaseId,release) in result)
            {
                Assert.True(release.LatestDeploymentsPerEnvironment.Count()==1);
            }
        }

        [Fact]
        public async Task Service_returns_empty_result_if_it_can_not_read_releases_from_the_file()
        {
            // arrange 
            var sut =
                _fixture.Start()
                    .WithFileReaderSetupForReleases(null)
                    .WithFileReaderSetupForDeployments(_fixture.GetDeploymentsString())
                    .WithFileReaderSetupForEnvironments(_fixture.GetEnvironmentsString())
                    .Build();

            // act
            var result = (IDictionary<string, AppRelease>)(await sut.LoadAsync()).ToDictionary(r => r.Id);

            // assert
            Assert.True(result.Count == 0);
        }

        [Fact]
        public async Task Service_returns_releases_if_the_releases_do_not_have_any_deployments()
        {
            // arrange 
            var sut =
                _fixture.Start()
                    .WithFileReaderSetupForReleases(_fixture.GetReleasesString())
                    .WithFileReaderSetupForDeployments(null)
                    .WithFileReaderSetupForEnvironments(_fixture.GetEnvironmentsString())
                    .Build();

            // act
            var result = (IDictionary<string, AppRelease>)(await sut.LoadAsync()).ToDictionary(r => r.Id);

            // assert
            Assert.True(result.Count == 2);
            Assert.Contains("Release-1", result);
            Assert.Contains("Release-2", result);

            foreach (var (releaseId, release) in result)
            {
                Assert.True(!release.LatestDeploymentsPerEnvironment.Any());
            }
        }

        [Fact]
        public async Task Service_returns_empty_result_if_it_can_not_read_deployments_and_environments_from_the_file()
        {
            // arrange 
            var sut =
                _fixture.Start()
                    .WithFileReaderSetupForReleases(_fixture.GetReleasesString())
                    .WithFileReaderSetupForDeployments(null)
                    .WithFileReaderSetupForEnvironments(null)
                    .Build();

            // act
            var result = (IDictionary<string, AppRelease>)(await sut.LoadAsync()).ToDictionary(r => r.Id);

            // assert
            Assert.True(result.Count == 2);
            Assert.Contains("Release-1", result);
            Assert.Contains("Release-2", result);

            foreach (var (releaseId, release) in result)
            {
                Assert.True(!release.LatestDeploymentsPerEnvironment.Any());
            }
        }
    }
}