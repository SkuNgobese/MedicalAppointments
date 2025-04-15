using MedicalAppointments.Interfaces;
using MedicalAppointments.Shared.Models;
using Microsoft.AspNetCore.Components;
using MedicalAppointments.Shared.ViewModels;

namespace MedicalAppointments.Pages
{
    public partial class HospitalComponent
    {
        [Inject]
        private IHospital? _hospital { get; set; }

        protected IEnumerable<HospitalViewModel>? hospitals;

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
                    Street = hospitalVM.AddressDetails!.Street,
                    Suburb = hospitalVM.AddressDetails.Suburb,
                    City = hospitalVM.AddressDetails.City,
                    PostalCode = hospitalVM.AddressDetails.PostalCode,
                    Country = hospitalVM.AddressDetails.Country
                },
                Contact = new Contact
                {
                    ContactNumber = hospitalVM.ContactDetails!.ContactNumber,
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

        private void EditHospital(HospitalViewModel hospital)
        {
            hospitalVM = hospital;

            editingHospitalId = hospital.Id;
            editingAddressId = hospital.AddressDetails!.Id;
            editingContactId = hospital.ContactDetails?.Id;

            isEditing = true;
        }

        private void ConfirmDelete(HospitalViewModel hospitalVM)
        {
            hospitalToDelete = new Hospital
            {
                Name = hospitalVM.HospitalName,
                Address = new Address
                {
                    Street = hospitalVM.AddressDetails!.Street,
                    Suburb = hospitalVM.AddressDetails.Suburb,
                    City = hospitalVM.AddressDetails.City,
                    PostalCode = hospitalVM.AddressDetails.PostalCode,
                    Country = hospitalVM.AddressDetails.Country
                },
                Contact = new Contact
                {
                    ContactNumber = hospitalVM.ContactDetails!.ContactNumber,
                    Fax = hospitalVM.ContactDetails.Fax,
                    Email = hospitalVM.ContactDetails.Email
                }
            };

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