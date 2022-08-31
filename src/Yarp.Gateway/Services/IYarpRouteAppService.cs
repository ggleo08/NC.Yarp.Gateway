using Yarp.Gateway.Entities;

namespace Yarp.Gateway.Services
{
    public interface IYarpRouteAppService
    {
        IQueryable<YarpRoute> GetAll();
        Task<YarpRoute> Find(Guid id);
        Task<bool> Create(YarpRoute route);
        Task<bool> Update(YarpRoute route);
        Task<bool> Delete(Guid id);
        Task<bool> UpdateRouteName(Guid id, string routeId);
    }
}
