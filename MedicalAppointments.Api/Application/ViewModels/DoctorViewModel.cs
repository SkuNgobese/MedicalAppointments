﻿using System.ComponentModel;

namespace MedicalAppointments.Api.Application.ViewModels
{
    public partial class DoctorViewModel : UserViewModel
    {
        [DisplayName("Specialization")]
        public required string Specialization { get; set; }

        [DisplayName("Hire Date")]
        public required DateTime HireDate { get; set; }
    }
}
