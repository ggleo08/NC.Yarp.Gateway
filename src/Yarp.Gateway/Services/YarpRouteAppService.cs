using Microsoft.EntityFrameworkCore;
using Yarp.Gateway.Entities;
using Yarp.Gateway.EntityFrameworkCore;

namespace Yarp.Gateway.Services
{
    public class YarpRouteAppService : IYarpRouteAppService
    {
        private readonly ILogger<YarpRouteAppService> _logger;
        private YarpDbContext _yarpDbContext;
        private readonly IYarpConfigurationStore _yarpStore;

        public YarpRouteAppService(YarpDbContext dbContext,
                                   IYarpConfigurationStore yarpStore, 
                                   ILogger<YarpRouteAppService> logger)
        {
            _yarpDbContext = dbContext;
            _yarpStore = yarpStore;
            _logger = logger;
        }

        public async Task<bool> Create(YarpRoute route)
        {
            await _yarpDbContext.YarpRoutes.AddAsync(route);
            var res = await _yarpDbContext.SaveChangesAsync();
            if (res > 0)
            {
                _logger.LogInformation("Create Cluster Success.");
                ReloadYarpConfig();
                return true;
            }
            return false;
        }

        public async Task<YarpRoute> Find(Guid id)
        {
            return await _yarpDbContext.YarpRoutes.FirstOrDefaultAsync(c => c.Id == id);
        }

        public IQueryable<YarpRoute> GetAll()
        {
            return _yarpDbContext.YarpRoutes.AsNoTracking();
        }

        public async Task<bool> Update(YarpRoute route)
        {
            var dbRoute = await _yarpDbContext.YarpRoutes
                                              .Include(r => r.Match).ThenInclude(m => m.Headers)
                                              .Include(r => r.Metadata)
                                              .Include(r => r.Transforms)
                                              .FirstAsync(r => r.Id == route.Id);

            using (var tran = _yarpDbContext.Database.BeginTransaction())
            {
                try
                {
                    if (dbRoute.Match != null)
                        _yarpDbContext.Remove(dbRoute.Match);
                    if (dbRoute.Transforms != null)
                        _yarpDbContext.RemoveRange(dbRoute.Transforms);
                    if (dbRoute.Metadata != null)
                        _yarpDbContext.RemoveRange(dbRoute.Metadata);

                    await _yarpDbContext.SaveChangesAsync();

                    if (route.Match != null)
                    {
                        route.Match.Id = Guid.NewGuid();
                        if (route.Match.Headers != null)
                            route.Match.Headers.ForEach(d => d.Id = Guid.NewGuid());
                        dbRoute.Match = route.Match;
                    }

                    if (route.Transforms != null)
                    {
                        route.Transforms.ForEach(d => d.Id = Guid.NewGuid());
                        dbRoute.Transforms = route.Transforms;
                    }

                    if (route.Metadata != null)
                    {
                        route.Metadata.ForEach(d => d.Id = Guid.NewGuid());
                        dbRoute.Metadata = route.Metadata;
                    }
                    dbRoute.RouteId = route.RouteId;
                    dbRoute.ClusterId = route.ClusterId;
                    dbRoute.Order = route.Order;
                    dbRoute.AuthorizationPolicy = route.AuthorizationPolicy;
                    dbRoute.CorsPolicy = route.CorsPolicy;
                    _yarpDbContext.Update(dbRoute);
                    await _yarpDbContext.SaveChangesAsync();
                    await tran.CommitAsync();
                    _logger.LogInformation($"Update Route: {route.RouteId} Success.");
                    ReloadYarpConfig();
                    return true;
                }
                catch (Exception ex)
                {
                    await tran.RollbackAsync();
                    _logger.LogError(ex, ex.Message);
                    return false;
                }
            }
        }
        public async Task<bool> Delete(Guid id)
        {
            var route = await _yarpDbContext.YarpRoutes.FirstOrDefaultAsync(c => c.Id == id);

            if (route is null)
            {
                _logger.LogError("Route Not Exist");
                return false;
            }
            _yarpDbContext.Remove(route);
            await _yarpDbContext.SaveChangesAsync();
            ReloadYarpConfig();
            _logger.LogInformation($"Delete Route: {route.RouteId} Success.");
            return true;
        }

        private void ReloadYarpConfig()
        {
            Task.Factory.StartNew(() => _yarpStore.Reload());
        }

        public async Task<bool> UpdateRouteName(Guid id, string routeId)
        {
            var route = await _yarpDbContext.YarpRoutes.FirstOrDefaultAsync(c => c.Id == id);

            if (route is null)
            {
                _logger.LogError("Route Not Exist");
                return false;
            }
            route.RouteId = routeId;
            await _yarpDbContext.SaveChangesAsync();
            ReloadYarpConfig();
            _logger.LogInformation($"Delete Route: {route.RouteId} Success.");
            return true;
        }
    }
}
