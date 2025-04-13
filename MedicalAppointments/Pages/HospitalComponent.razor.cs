using MedicalAppointments.Interfaces;
using MedicalAppointments.Api.Models;
using MedicalAppointments.Api.ViewModels;
using Microsoft.AspNetCore.Components;

namespace MedicalAppointments.Pages
{
    public partial class HospitalComponent
    {
        [Inject]
        private IHospital? _hospital { get; set; }

        protected IEnumerable<Hospital>? hospitals;

        private bool isEditing = false;
        private int? editingHospitalId = null;
        private int? editingAddressId = null;
        private int? editingContactId = null;

        private bool showDeleteModal = false;
        private Hospital? hospitalToDelete;

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
    
        private async Task LoadHospitals()
        {
            if (_hospital != null)
                hospitals = await _hospital.GetAllHospitalsAsync();
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

                if (editingAddressId.HasValue)
                    hospital.Address.Id = editingAddressId.Value;

                if (editingContactId.HasValue)
                    hospital.Contact.Id = editingContactId.Value;

                await _hospital!.UpdateHospitalAsync(hospital);
            }
            else
                await _hospital!.AddHospitalAsync(hospital);

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
            editingAddressId = hospital.Address!.Id;
            editingContactId = hospital.Contact?.Id;

            isEditing = true;
        }

        private void ConfirmDelete(Hospital hospital)
        {
            hospitalToDelete = hospital;
            showDeleteModal = true;
        }

        private async Task DeleteConfirmed()
        {
            if (hospitalToDelete is not null)
            {
                await _hospital!.RemoveHospitalAsync(hospitalToDelete);
                await LoadHospitals();
                hospitalToDelete = null;
            }

            showDeleteModal = false;
        }

        private void CancelDelete()
        {
            hospitalToDelete = null;
            showDeleteModal = false;
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
            editingAddressId = null;
            editingContactId = null;

            isEditing = false;
        }
    }
}