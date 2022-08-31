using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Yarp.Gateway.Entities
{
    public class YarpDestination
    {
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// eg."cluster1/destination1"
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Address of this destination. E.g. <c>https://127.0.0.1:123/abcd1234/</c>.
        /// </summary>
        public string? Address { get; set; }

        /// <summary>
        /// Endpoint accepting active health check probes. E.g. <c>http://127.0.0.1:1234/</c>.
        /// </summary>
        public string? Health { get; set; }
        public Guid? ClusterId { get; set; }

        [ForeignKey("ClusterId")]
        public virtual YarpCluster Cluster { get; set; }

        /// <summary>
        /// Arbitrary key-value pairs that further describe this destination.
        /// </summary>
        public virtual List<YarpMetadata> Metadata { get; set; }
    }
}
