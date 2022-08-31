using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Yarp.Gateway.Entities
{
    public class YarpActiveHealthCheckOption
    {
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Whether active health checks are enabled.
        /// </summary>
        public bool? Enabled { get; set; }

        /// <summary>
        /// Health probe interval.
        /// </summary>
        public string? Interval { get; set; }

        /// <summary>
        /// Health probe timeout, after which a destination is considered unhealthy.
        /// </summary>
        public string? Timeout { get; set; }

        /// <summary>
        /// Active health check policy.
        /// </summary>
        public string? Policy { get; set; }

        /// <summary>
        /// HTTP health check endpoint path.
        /// </summary>
        public string? Path { get; set; }

        public Guid? HealthCheckOptionId { get; set; }

        [ForeignKey("HealthCheckOptionId")]
        public virtual YarpHealthCheckOption HealthCheckOption { get; set; }
    }
}
