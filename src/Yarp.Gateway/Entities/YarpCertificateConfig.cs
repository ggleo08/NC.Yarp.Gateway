using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Yarp.Gateway.Entities
{
    public class YarpCertificateConfig
    {
        [Key]
        public Guid Id { get; set; }

        public string? Path { get; set; }

        public string? KeyPath { get; set; }

        public string? Password { get; set; }

        public string? Subject { get; set; }

        public string? Store { get; set; }

        public string? Location { get; set; }

        public bool? AllowInvalid { get; set; }

        internal bool IsFileCert => !string.IsNullOrEmpty(Path);

        internal bool IsStoreCert => !string.IsNullOrEmpty(Subject);

        public Guid? HttpClientOptionId { get; set; }
        
        [ForeignKey("HttpClientOptionId")]
        public virtual YarpHttpClientOption HttpClientOption { get; set; }
    }
}
