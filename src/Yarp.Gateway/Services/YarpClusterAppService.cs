using Microsoft.EntityFrameworkCore;
using Yarp.Gateway.Entities;
using Yarp.Gateway.EntityFrameworkCore;

namespace Yarp.Gateway.Services
{
    public class YarpClusterAppService : IYarpClusterAppService
    {
        private readonly ILogger<YarpClusterAppService> _logger;
        private YarpDbContext _yarpDbContext;
        private readonly IYarpConfigurationStore _yarpStore;

        public YarpClusterAppService(YarpDbContext dbContext,
                                     IYarpConfigurationStore yarpStore,
                                     ILogger<YarpClusterAppService> logger)
        {
            _yarpDbContext = dbContext;
            _yarpStore = yarpStore;
            _logger = logger;
        }

        public async Task<bool> Create(YarpCluster cluster)
        {
            await _yarpDbContext.AddAsync(cluster);
            var res = await _yarpDbContext.SaveChangesAsync();
            if (res > 0)
            {
                _logger.LogInformation("Create Cluster Success.");
                ReloadYarpConfig();
                return true;
            }
            return false;
        }

        public async Task<YarpCluster> Find(Guid id)
        {
            return await _yarpDbContext.YarpClusters.FirstOrDefaultAsync(c => c.Id == id);
        }

        public IQueryable<YarpCluster> GetAll()
        {
            return _yarpDbContext.YarpClusters.AsNoTracking();
        }

        public async Task<bool> Update(YarpCluster cluster)
        {
            var dbCluster = await _yarpDbContext.YarpClusters
                                                .Include(c => c.Metadata)
                                                .Include(c => c.Destinations)
                                                .Include(c => c.SessionAffinity).ThenInclude(s => s.Cookie)
                                                .Include(c => c.HttpRequest)
                                                .Include(c => c.HttpClient)
                                                .Include(c => c.HealthCheck).ThenInclude(h => h.Active)
                                                .Include(c => c.HealthCheck).ThenInclude(h => h.Passive)
                                                .FirstAsync(c => c.Id == cluster.Id);

            using (var tran = _yarpDbContext.Database.BeginTransaction())
            {
                try
                {

                    if (dbCluster.HealthCheck != null)
                        _yarpDbContext.Remove(dbCluster.HealthCheck);
                    if (dbCluster.Destinations != null)
                        _yarpDbContext.RemoveRange(dbCluster.Destinations);
                    if (dbCluster.HttpClient != null)
                        _yarpDbContext.Remove(dbCluster.HttpClient);
                    if (dbCluster.HttpRequest != null)
                        _yarpDbContext.Remove(dbCluster.HttpRequest);
                    if (dbCluster.SessionAffinity != null)
                        _yarpDbContext.Remove(dbCluster.SessionAffinity);
                    if (dbCluster.Metadata != null)
                        _yarpDbContext.RemoveRange(dbCluster.Metadata);

                    await _yarpDbContext.SaveChangesAsync();

                    if (cluster.HealthCheck != null)
                    {
                        cluster.HealthCheck.Id = Guid.NewGuid();
                        dbCluster.HealthCheck = cluster.HealthCheck;
                    }
                    if (cluster.Destinations != null)
                    {
                        cluster.Destinations.ForEach(d => d.Id = Guid.NewGuid());
                        dbCluster.Destinations = cluster.Destinations;
                    }
                    if (cluster.HttpClient != null)
                    {
                        cluster.HttpClient.Id = Guid.NewGuid();
                        dbCluster.HttpClient = cluster.HttpClient;
                    }
                    if (cluster.HttpRequest != null)
                    {
                        cluster.HttpRequest.Id = Guid.NewGuid();
                        dbCluster.HttpRequest = cluster.HttpRequest;
                    }
                    if (cluster.SessionAffinity != null)
                    {
                        cluster.SessionAffinity.Id = Guid.NewGuid();
                        dbCluster.SessionAffinity = cluster.SessionAffinity;
                    }
                    if (cluster.Metadata != null)
                    {
                        cluster.Metadata.ForEach(d => d.Id = Guid.NewGuid());
                        dbCluster.Metadata = cluster.Metadata;
                    }
                    dbCluster.LoadBalancingPolicy = cluster.LoadBalancingPolicy;
                    _yarpDbContext.Update(dbCluster);
                    await _yarpDbContext.SaveChangesAsync();
                    await tran.CommitAsync();
                    _logger.LogInformation($"Update Cluster: {cluster.ClusterId} Success.");
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
            var cluster = await _yarpDbContext.YarpClusters.FirstOrDefaultAsync(c => c.Id == id);
            if (cluster is null)
            {
                _logger.LogError("Cluster Not Exist");
                return false;
            }
            var useCount = await _yarpDbContext.YarpRoutes.CountAsync(p => p.ClusterId == id);
            if (useCount > 0)
            {
                _logger.LogError($"Cluster: {cluster.ClusterId} have been use.");
                return false;
            }
            var des = await _yarpDbContext.YarpDestinations.Include(d => d.Metadata).Where(d => d.ClusterId == cluster.Id).ToListAsync();
            foreach (var d in des)
            {
                _yarpDbContext.YarpMetadatas.RemoveRange(d.Metadata);
            }
            _yarpDbContext.YarpClusters.Remove(cluster);
            var res = await _yarpDbContext.SaveChangesAsync();
            if (res > 0)
            {
                _logger.LogInformation($"Delete Cluster: {id} Success.");
                ReloadYarpConfig();
                return true;
            }
            return false;
        }

        private void ReloadYarpConfig()
        {
            Task.Factory.StartNew(() => _yarpStore.Reload());
        }
    }
}
