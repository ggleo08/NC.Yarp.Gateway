using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Yarp.Gateway.Entities
{
    /// <summary>
    /// Config used to construct <seealso cref="System.Net.WebProxy"/> instance.
    /// </summary>
    public class YarpWebProxyConfig : IEquatable<YarpWebProxyConfig>
    {
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// The URI of the proxy server.
        /// </summary>
        public string? Address { get; init; }

        /// <summary>
        /// true to bypass the proxy for local addresses; otherwise, false.
        /// If null, default value will be used: false
        /// </summary>
        public bool? BypassOnLocal { get; init; }

        /// <summary>
        /// Controls whether the <seealso cref="System.Net.CredentialCache.DefaultCredentials"/> are sent with requests.
        /// If null, default value will be used: false
        /// </summary>
        public bool? UseDefaultCredentials { get; init; }
        
        public Guid? HttpClientOptionId { get; init; }

        [ForeignKey("HttpClientOptionId")]
        public virtual YarpHttpClientOption HttpClientOption { get; init; }



        public bool Equals(YarpWebProxyConfig? other)
        {
            if (other == null)
            {
                return false;
            }

            return Address == other.Address
                && BypassOnLocal == other.BypassOnLocal
                && UseDefaultCredentials == other.UseDefaultCredentials;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                Address,
                BypassOnLocal,
                UseDefaultCredentials
            );
        }
    }
}
