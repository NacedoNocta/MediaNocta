using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace YARPConfigLibrairy
{
    [Table("yarp_routes")]
    [Index(nameof(route_id), IsUnique = true)]
    public sealed class YarpRoute
    {
        [JsonPropertyName("id")]
        [Key]
        public Guid Id { get; init; } = Guid.NewGuid();

        [JsonPropertyName("routeId")]
        [Required]
        public required string route_id { get; set; }

        [JsonPropertyName("path")]
        [Required]
        public required string path { get; set; }

        [JsonPropertyName("clusterId")]
        [Required]
        public required string cluster_id { get; set; }

    }
}
