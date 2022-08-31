using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Yarp.Gateway.Entities
{
    public class YarpHealthCheckOption
    {
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Passive health check options.
        /// </summary>
        public virtual YarpPassiveHealthCheckOption Passive { get; init; }

        /// <summary>
        /// Active health check options.
        /// </summary>
        public virtual YarpActiveHealthCheckOption Active { get; init; }
        /// <summary>
        /// Available destinations policy.
        /// </summary>
        public string? AvailableDestinationsPolicy { get; init; }

        public Guid? ClusterId { get; set; }

        [ForeignKey("ClusterId")]
        public virtual YarpCluster Cluster { get; set; }
    }
}
