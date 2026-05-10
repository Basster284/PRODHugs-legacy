using System.Text.Json.Serialization;

namespace PRODHugs_frontend
{
    public record User(
        [property: JsonPropertyName("display_name")] string DisplayName,
        [property: JsonPropertyName("gender")] string Gender,
        [property: JsonPropertyName("role")] string Role,
        [property: JsonPropertyName("telegram_id")] int TelegramId,
        [property: JsonPropertyName("username")] string Username,
        [property: JsonPropertyName("id")] string Id
    );
}
