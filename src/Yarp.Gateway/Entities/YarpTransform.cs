using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Yarp.Gateway.Entities
{
    public class YarpTransform : KeyValueEntity
    {
        [Key]
        public Guid Id { get; set; }

        public EnumTransformType Type { get; set; }

        public Guid? RouteId { get; set; }

        [ForeignKey("RouteId")]
        public virtual YarpRoute Route { get; set; }
    }
}
