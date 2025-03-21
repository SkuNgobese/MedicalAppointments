﻿namespace MedicalAppointments.Domain.Models
{
    public class Patient : User
    {
        public string? IDNumber { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? RemoveDate { get; set; }
        public bool IsActive { get; set; }

        public List<Appointment>? Appointments { get; set; }
        public List<DiagnosticFile>? DiagnosticFiles { get; set; }
    }
}
