
using System.Text.Json.Serialization;

namespace CatFacts.src;

public class CatFact
{
    [JsonPropertyName("fact")]
    public string Fact { get; set; }
    [JsonPropertyName("length")]
    public int Length { get; set; }
}
