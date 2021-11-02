using System.Collections.Generic;
using System.Threading.Tasks;
using Release.Retention.Domain.Model.Objects;
using Release.Retention.Domain.Model.Service.Contracts;

namespace Release.Retention.Data
{
    public class AppReleaseJsonDataLoader:IAppReleaseDataLoader
    {
        public Task<IEnumerable<AppRelease>> LoadAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}