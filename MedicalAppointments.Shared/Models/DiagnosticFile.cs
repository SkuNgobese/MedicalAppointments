using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MedicalAppointments.Api.Models
{
    public class DiagnosticFile
    {
        public DiagnosticFile()
        {
            Id = Guid.NewGuid();
        }

        [Key]
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("diagnosis")]
        public required string Diagnosis { get; set; }

        [JsonPropertyName("treatment")]
        public string? Treatment { get; set; }

        [JsonPropertyName("attachmentfilename")]
        public string? AttachmentFileName { get; set; }

        public required Patient Patient { get; set; }
    }
}