using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Yarp.Gateway.Entities
{
    public class YarpMetadata : KeyValueEntity
    {
        [Key]
        public Guid Id { get; set; }

        public Guid? RouteId { get; set; }

        [ForeignKey("RouteId")]
        public virtual YarpRoute Route { get; set; }

        public Guid? ClusterId { get; set; }

        [ForeignKey("ClusterId")]
        public virtual YarpCluster Cluster { get; set; }


        public Guid? DestinationId { get; set; }

        [ForeignKey("DestinationId")]
        public virtual YarpDestination Destination { get; set; }
    }
}
