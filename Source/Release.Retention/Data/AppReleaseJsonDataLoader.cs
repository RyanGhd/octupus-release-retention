using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Release.Retention.Data.Entities;
using Release.Retention.Domain.Model.Objects;
using Release.Retention.Domain.Model.Service.Contracts;

namespace Release.Retention.Data
{
    public class AppReleaseJsonDataLoaderSettings
    {
        public string BaseFolder { get; }

        public AppReleaseJsonDataLoaderSettings(string baseFolder)
        {
            BaseFolder = baseFolder;
        }
    }

    public class AppReleaseJsonDataLoader : IAppReleaseDataLoader
    {
        private readonly IFileReader _fileReader;
        private readonly AppReleaseJsonDataLoaderSettings _settings;


        public AppReleaseJsonDataLoader(IFileReader fileReader, AppReleaseJsonDataLoaderSettings settings)
        {
            _fileReader = fileReader;
            _settings = settings;
        }

        public async Task<IEnumerable<AppRelease>> LoadAsync()
        {
            var result = new List<AppRelease>();

            // read the files
            // release entities
            var releasesContent = await _fileReader.ReadAsync(Path.Combine(_settings.BaseFolder, "Releases.json"));
            if (string.IsNullOrWhiteSpace(releasesContent))
                return result;

            var releaseEntities = JsonConvert.DeserializeObject<List<AppReleaseEntity>>(releasesContent);

            // deployment entities
            var deploymentContent = await _fileReader.ReadAsync(Path.Combine(_settings.BaseFolder, "Deployments.json"));

            var deploymentEntities = string.IsNullOrWhiteSpace(deploymentContent)
                ? new List<AppDeploymentEntity>() :
                JsonConvert.DeserializeObject<List<AppDeploymentEntity>>(deploymentContent);

            // environment entities
            var environmentContent = await _fileReader.ReadAsync(Path.Combine(_settings.BaseFolder, "Environments.json"));

            var environmentEntities = string.IsNullOrWhiteSpace(environmentContent)
            ? new List<EnvironmentEntity>()
            : JsonConvert.DeserializeObject<List<EnvironmentEntity>>(environmentContent);

            var environments = environmentEntities.Select(e => new AppEnvironment(e.Id, e.Name))
                                                  .ToDictionary(e => e.Id);

            // find the entities and build the object graph
            var releases = new List<AppRelease>();

            foreach (var releaseEntity in releaseEntities)
            {
                var deployments = deploymentEntities.Where(d => d.ReleaseId.Equals(releaseEntity.Id, StringComparison.InvariantCultureIgnoreCase))
                                                         .Select(d => new AppDeployment(d.Id, d.ReleaseId, environments[d.EnvironmentId], d.DeployedAt));

                releases.Add(new AppRelease(releaseEntity.Id, releaseEntity.ProjectId, releaseEntity.Version, releaseEntity.Created, deployments));
            }

            return releases;
        }


    }
}