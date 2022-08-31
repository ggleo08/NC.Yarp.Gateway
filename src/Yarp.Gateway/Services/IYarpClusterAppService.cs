using Yarp.Gateway.Entities;

namespace Yarp.Gateway.Services
{
    public interface IYarpClusterAppService
    {
        IQueryable<YarpCluster> GetAll();
        Task<YarpCluster> Find(Guid id);
        Task<bool> Create(YarpCluster cluster);
        Task<bool> Update(YarpCluster cluster);
        Task<bool> Delete(Guid id);
    }
}
