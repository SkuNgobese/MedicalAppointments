﻿using System.ComponentModel;

namespace MedicalAppointments.Shared.ViewModels
{
    public partial class HospitalViewModel
    {
        [DisplayName("Hospital Name")]
        public required string HospitalName { get; set; }

        public required AddressViewModel AddressDetails { get; set; }
        
        public required ContactViewModel ContactDetails { get; set; }
    }
}