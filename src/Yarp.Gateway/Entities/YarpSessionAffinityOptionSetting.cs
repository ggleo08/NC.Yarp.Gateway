using System.ComponentModel.DataAnnotations;

namespace Yarp.Gateway.Entities
{
    public class YarpSessionAffinityOptionSetting : KeyValueEntity
    {
        [Key]
        public Guid Id { get; set; }
    }
}
