﻿using System.ComponentModel;

namespace MedicalAppointments.Application.ViewModels
{
    public partial class UserViewModel
    {
        [DisplayName("Title")]
        public required string Title { get; set; }

        [DisplayName("First Name")]
        public required string FirstName { get; set; }

        [DisplayName("Last Name")]
        public required string LastName { get; set; }

        [DisplayName("ID Number")]
        public required string IDNumber { get; set; }

        public required AddressViewModel AddressDetails { get; set; }
        public required ContactViewModel ContactDetails { get; set; }
    }
}
