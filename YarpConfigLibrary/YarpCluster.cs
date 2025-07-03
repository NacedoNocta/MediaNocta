using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace YarpConfigLibrary
{
    [Table("yarp_clusters")]
    [Index(nameof(cluster_id), IsUnique = true)]
    public sealed class YarpCluster
    {
        [JsonPropertyName("id")]
        [Key]
        public Guid Id { get; init; } = Guid.NewGuid();

        [JsonPropertyName("clusterId")]
        [Required]
        public required string cluster_id { get; set; }

        [JsonPropertyName("destinationUrl")]
        [Required]
        public required string destination_url { get; set; }

        public YarpCluster()
        {
        }

        public YarpCluster(string clusterId, string destinationUrl)
        {
            cluster_id = clusterId;
            destination_url = destinationUrl;
        }
    }
}
