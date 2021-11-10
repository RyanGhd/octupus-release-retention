using System.Collections.Generic;
using System.Threading.Tasks;
using Release.Retention.Domain.Model.Objects;

namespace Release.Retention.Domain.Model.Service.Contracts
{
    public interface IDataSourceSettings{

    }
    public interface IAppReleaseDataLoader
    {
        Task<IEnumerable<AppRelease>> LoadAsync();
    }
}