using System.Text.Json.Serialization;

namespace PRODHugs_frontend
{
    public record HugElement(
        [property: JsonPropertyName("giver_display_name")] string GiverDisplayName,
        [property: JsonPropertyName("receiver_display_name")] string ReceiverDisplayName
    );
}
