using System;
using System.Text.Json.Serialization;

namespace Kafka.Mysql.Example.ViewModels
{
    public class CarCacheViewModel
    {
        [JsonPropertyName("id")] public int Id { get; set; }

        [JsonPropertyName("name")] public string Name { get; set; }

        [JsonPropertyName("color")] public string Color { get; set; }

        [JsonPropertyName("price")] public int Price { get; set; }
    }
}
