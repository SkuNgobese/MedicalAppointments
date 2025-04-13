using System.Text.Json.Serialization;

namespace MedicalAppointments.Api.Models;

public class Contact
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("contactnumber")]
    public required string ContactNumber { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("fax")]
    public string? Fax { get; set; }
}