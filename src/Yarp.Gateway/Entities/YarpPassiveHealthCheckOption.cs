using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Yarp.Gateway.Entities
{
    public class YarpPassiveHealthCheckOption
    {
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Whether passive health checks are enabled.
        /// </summary>
        public bool? Enabled { get; set; }

        /// <summary>
        /// Passive health check policy.
        /// </summary>
        public string? Policy { get; set; }

        /// <summary>
        /// Destination reactivation period after which an unhealthy destination is considered healthy again.
        /// </summary>
        public string? ReactivationPeriod { get; set; }

        public Guid? HealthCheckOptionId { get; set; }

        [ForeignKey("HealthCheckOptionId")]

        public virtual YarpHealthCheckOption HealthCheckOption { get; set; }
    }
}
