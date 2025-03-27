namespace MedicalAppointments.Api.Domain.Models;

public class Contact
{
    public int Id { get; set; }
    public required string PhoneNumber { get; set; }
    public string? Email { get; set; }
    public string? Fax { get; set; }
}
