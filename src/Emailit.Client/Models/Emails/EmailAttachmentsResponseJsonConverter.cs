using System.Text.Json;
using System.Text.Json.Serialization;

namespace Emailit.Client.Models.Emails;

internal sealed class EmailAttachmentsResponseJsonConverter : JsonConverter<EmailAttachmentsResponse>
{
    public override EmailAttachmentsResponse Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var document = JsonDocument.ParseValue(ref reader);
        var root = document.RootElement;

        var objectValue = root.TryGetProperty("object", out var objectProperty)
            ? objectProperty.GetString() ?? "email"
            : "email";

        var id = root.TryGetProperty("id", out var idProperty)
            ? idProperty.ToString()
            : null;

        List<EmailAttachment> attachments;
        if (root.TryGetProperty("attachments", out var attachmentsProperty))
        {
            attachments = JsonSerializer.Deserialize<List<EmailAttachment>>(attachmentsProperty.GetRawText(), options) ?? [];
        }
        else if (root.TryGetProperty("data", out var dataProperty))
        {
            attachments = JsonSerializer.Deserialize<List<EmailAttachment>>(dataProperty.GetRawText(), options) ?? [];
        }
        else
        {
            attachments = [];
        }

        return new EmailAttachmentsResponse
        {
            Object = objectValue,
            Id = id,
            Attachments = attachments
        };
    }

    public override void Write(Utf8JsonWriter writer, EmailAttachmentsResponse value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteString("object", value.Object);

        if (!string.IsNullOrWhiteSpace(value.Id))
        {
            writer.WriteString("id", value.Id);
        }

        writer.WritePropertyName("attachments");
        JsonSerializer.Serialize(writer, value.Attachments, options);
        writer.WriteEndObject();
    }
}
