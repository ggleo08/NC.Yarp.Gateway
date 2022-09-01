using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Yarp.Gateway.Entities
{
    public class YarpCluster
    {
        /// <summary>
        /// Key
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// The Id for this cluster. This needs to be globally unique.
        /// </summary>
        public string? ClusterId { get; set; }

        /// <summary>
        /// Load balancing policy.
        /// </summary>
        public string? LoadBalancingPolicy { get; set; }

        /// <summary>
        /// Session affinity options.
        /// </summary>
        public virtual YarpSessionAffinityConfig SessionAffinity { get; set; }

        /// <summary>
        /// Health checking options.
        /// </summary>
        public virtual YarpHealthCheckOption HealthCheck { get; set; }

        /// <summary>
        /// Options of an HTTP client that is used to call this cluster.
        /// </summary>
        public virtual YarpHttpClientOption HttpClient { get; set; }

        /// <summary>
        /// Options of an outgoing HTTP request.
        /// </summary>
        public virtual YarpForwarderRequest HttpRequest { get; set; }

        /// <summary>
        /// The set of destinations associated with this cluster.
        /// </summary>
        public virtual List<YarpDestination> Destinations { get; set; }

        /// <summary>
        /// Arbitrary key-value pairs that further describe this cluster.
        /// </summary>
        public virtual List<YarpMetadata> Metadata { get; set; }

        public virtual List<YarpRoute> ProxyRoutes { get; set; }
    }
}
