using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Yarp.ReverseProxy.Configuration;

namespace Yarp.Gateway.Entities
{
    public class YarpMatch
    {
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Only match requests that use these optional HTTP methods. E.g. GET, POST.
        /// </summary>
        public string? Methods { get; set; }

        /// <summary>
        /// Only match requests with the given Host header.
        /// </summary>
        public string? Hosts { get; set; }

        /// <summary>
        /// Only match requests with the given Path pattern.
        /// </summary>
        public string? Path { get; set; }

        /// <summary>
        /// Only match requests that contain all of these query parameters.
        /// </summary>
        public virtual List<YarpRouteQueryParameter> QueryParameters { get; set; }

        public virtual List<YarpRouteHeader> Headers { get; set; }

        public Guid? RouteId { get; set; }

        [ForeignKey("RouteId")]
        public virtual YarpRoute Route { get; set; }
    }
}
