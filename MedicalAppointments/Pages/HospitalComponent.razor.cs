using MedicalAppointments.Shared.Interfaces;
using MedicalAppointments.Shared.Models;
using MedicalAppointments.Shared.ViewModels;
using Microsoft.AspNetCore.Components;

namespace MedicalAppointments.Pages
{
    public partial class HospitalComponent
    {
        [Inject]
        public IHospital? Hospital { get; set; }

        protected IEnumerable<Hospital>? hospitals;

        private bool isEditing = false;
        private int? editingHospitalId = null;

        protected override async Task OnInitializedAsync()
        {
            await LoadHospitals();
        }

        private HospitalViewModel hospitalVM = new()
        {
            HospitalName = string.Empty,
            AddressDetails = new AddressViewModel()
            {
                Street = string.Empty,
                Suburb = string.Empty,
                City = string.Empty,
                PostalCode = string.Empty,
                Country = string.Empty
            },
            ContactDetails = new ContactViewModel()
            {
                ContactNumber = string.Empty,
                Fax = string.Empty,
                Email = string.Empty
            }
        };

        public HospitalComponent(List<Hospital> allHospitals) => this.hospitals = allHospitals;
                
        private async Task LoadHospitals()
        {
            if (Hospital != null)
                hospitals = await Hospital.GetAllHospitalsAsync();
            else
                hospitals = [];
        }

        private async Task HandleValidSubmit()
        {
            var hospital = new Hospital
            {
                Name = hospitalVM.HospitalName,
                Address = new Address
                {
                    Street = hospitalVM.AddressDetails.Street,
                    Suburb = hospitalVM.AddressDetails.Suburb,
                    City = hospitalVM.AddressDetails.City,
                    PostalCode = hospitalVM.AddressDetails.PostalCode,
                    Country = hospitalVM.AddressDetails.Country
                },
                Contact = new Contact
                {
                    ContactNumber = hospitalVM.ContactDetails.ContactNumber,
                    Fax = hospitalVM.ContactDetails.Fax,
                    Email = hospitalVM.ContactDetails.Email
                }
            };

            if (isEditing && editingHospitalId.HasValue)
            {
                hospital.Id = editingHospitalId.Value;
                await hospitalService.UpdateHospitalAsync(hospital);
            }
            else
                await hospitalService.AddHospitalAsync(hospital);

            await LoadHospitals();
            ResetForm();
        }

        private void EditHospital(Hospital hospital)
        {
            hospitalVM = new HospitalViewModel
            {
                HospitalName = hospital.Name,
                AddressDetails = new AddressViewModel
                {
                    Street = hospital.Address?.Street ?? string.Empty,
                    Suburb = hospital.Address?.Suburb ?? string.Empty,
                    City = hospital.Address?.City ?? string.Empty,
                    PostalCode = hospital.Address?.PostalCode ?? string.Empty,
                    Country = hospital.Address?.Country ?? string.Empty
                },
                ContactDetails = new ContactViewModel
                {
                    ContactNumber = hospital.Contact?.ContactNumber ?? string.Empty,
                    Fax = hospital.Contact?.Fax ?? string.Empty,
                    Email = hospital.Contact?.Email ?? string.Empty
                }
            };

            editingHospitalId = hospital.Id;
            isEditing = true;
        }

        private async Task DeleteHospital(Hospital hospital)
        {
            await hospitalService.RemoveHospitalAsync(hospital);
            await LoadHospitals();
        }

        private void ResetForm()
        {
            hospitalVM = new HospitalViewModel
            {
                HospitalName = string.Empty,
                AddressDetails = new AddressViewModel()
                {
                    Street = string.Empty,
                    Suburb = string.Empty,
                    City = string.Empty,
                    PostalCode = string.Empty,
                    Country = string.Empty
                },
                ContactDetails = new ContactViewModel()
                {
                    ContactNumber = string.Empty,
                    Fax = string.Empty,
                    Email = string.Empty
                }
            };
            editingHospitalId = null;
            isEditing = false;
        }
    }
}