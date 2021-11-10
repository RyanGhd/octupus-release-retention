using System;
using System.Threading.Tasks;
using Moq;
using Release.Retention.Domain.Model.Service.Contracts;

namespace Release.Retention.Data
{
    public class AppReleaseJsonDataLoaderTestFixture
    {
        private Mock<IFileReader> FileReaderMock { get; set; }
        private AppReleaseJsonDataLoaderSettings Settings { get; set; }

        public AppReleaseJsonDataLoaderTestFixture Start()
        {
            FileReaderMock = new Mock<IFileReader>();
            Settings = new AppReleaseJsonDataLoaderSettings("TestData");

            return this;
        }

        public AppReleaseJsonDataLoaderTestFixture WithFileReaderSetupForReleases(string output)
        {
            return WithFileReaderSetup("releases.json", output);
        } 
        
        public AppReleaseJsonDataLoaderTestFixture WithFileReaderSetupForDeployments(string output)
        {
            return WithFileReaderSetup("deployments.json", output);
        } 

        public AppReleaseJsonDataLoaderTestFixture WithFileReaderSetupForEnvironments(string output)
        {
            return WithFileReaderSetup("environments.json", output);
        }

        private AppReleaseJsonDataLoaderTestFixture WithFileReaderSetup(string input, string output)
        {
            FileReaderMock.Setup(m => m.ReadAsync(It.Is<string>(val => val.Contains(input, StringComparison.InvariantCultureIgnoreCase)))).Returns(Task.FromResult(output));
            return this;
        }

        public AppReleaseJsonDataLoader Build()
        {
            return new AppReleaseJsonDataLoader(FileReaderMock.Object, Settings); ;
        }

        
        public string GetReleasesString()
        {
            return @"[
                         {
                             'Id': 'Release-1',
                             'ProjectId': 'Project-1',
                             'Version': '1.0.0',
                             'Created': '2000-01-01T09:00:00'
                         },
                         {
                             'Id': 'Release-2',
                             'ProjectId': 'Project-1',
                             'Version': '1.0.1',
                             'Created': '2000-01-02T09:00:00'
                         }
                     ]";
        }

        public string GetDeploymentsString()
        {
            return @"[
                       {
                           'Id': 'Deployment-1',
                           'ReleaseId': 'Release-1',
                           'EnvironmentId': 'Environment-1',
                           'DeployedAt': '2000-01-01T10:00:00'
                         },
                         {
                           'Id': 'Deployment-2',
                           'ReleaseId': 'Release-2',
                           'EnvironmentId': 'Environment-2',
                           'DeployedAt': '2000-01-02T10:00:00'
                         }
                      ]";
        }

        public string GetEnvironmentsString()
        {
            return @"[
                         {
                           'Id': 'Environment-1',
                           'Name': 'Staging'
                         },
                         {
                           'Id': 'Environment-2',
                           'Name': 'Production'
                         }
                    ]";
        }
    }
}