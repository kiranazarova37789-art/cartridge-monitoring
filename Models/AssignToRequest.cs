using System.Text.Json.Serialization;

namespace Project.Models;

public class AssignToRequest
{
    [JsonPropertyName("assign")]
    public string Assign { get; set; }
    [JsonPropertyName("requests")]
    public List<int> Requests { get; set; }
}