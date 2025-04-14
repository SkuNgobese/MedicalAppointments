using System.ComponentModel.DataAnnotations;

namespace MedicalAppointments
{
    public class FutureDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            if (value is not DateTime date)
                return new ValidationResult("Invalid date format.");

            // Check if date is in the past
            if (date < DateTime.Now)
                return new ValidationResult("Appointment date must be in the future.");

            // Ensure the time is within working hours (optional)
            if (date.Hour < 8 || date.Hour > 16)
                return new ValidationResult("Appointments must be scheduled between 8 AM and 5 PM.");

            return ValidationResult.Success!;
        }
    }
}