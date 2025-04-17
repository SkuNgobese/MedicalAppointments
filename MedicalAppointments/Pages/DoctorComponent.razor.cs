using MedicalAppointments.Interfaces;
using MedicalAppointments.Shared.Models;
using Microsoft.AspNetCore.Components;
using MedicalAppointments.Shared.ViewModels;

namespace MedicalAppointments.Pages
{
    public partial class DoctorComponent
    {
        [Inject]
        public IDoctor? _doctor { get; set; }

        protected IEnumerable<DoctorViewModel>? doctors;
        private ErrorViewModel? errorModel;

        private DoctorViewModel doctorVM = new()
        {
            Title = string.Empty,
            FirstName = string.Empty,
            LastName = string.Empty,
            IDNumber = string.Empty,
            Specialization = string.Empty,
            HireDate = DateTime.Now,

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

        private List<string> titles = ["Dr", "Mr", "Mrs", "Miss", "Ms", "Prof"];

        private bool isEditing = false;
        private string? editingDoctorId = null;
        private int? editingAddressId = null;
        private int? editingContactId = null;
        private bool showDeleteModal = false;

        private Doctor? doctorToDelete;

        protected override async Task OnInitializedAsync()
        {
            await LoadDoctors();
        }

        private async Task HandleValidSubmit()
        {
            var doctor = new Doctor
            {
                Title = doctorVM.Title,
                FirstName = doctorVM.FirstName,
                LastName = doctorVM.LastName,
                IDNumber = doctorVM.IDNumber,
                Specialization = doctorVM.Specialization,
                HireDate = doctorVM.HireDate,
                Address = new Address
                {
                    Street = doctorVM.AddressDetails!.Street,
                    Suburb = doctorVM.AddressDetails.Suburb,
                    City = doctorVM.AddressDetails.City,
                    PostalCode = doctorVM.AddressDetails.PostalCode,
                    Country = doctorVM.AddressDetails.Country
                },
                Contact = new Contact
                {
                    ContactNumber = doctorVM.ContactDetails!.ContactNumber,
                    Fax = doctorVM.ContactDetails.Fax,
                    Email = doctorVM.ContactDetails.Email
                }
            };


            if (isEditing && !string.IsNullOrEmpty(editingDoctorId))
            {
                doctor.Id = editingDoctorId;

                if (editingAddressId.HasValue)
                    doctor.Address.Id = editingAddressId.Value;

                if (editingContactId.HasValue)
                    doctor.Contact.Id = editingContactId.Value;

                errorModel = await _doctor!.UpdateDoctorAsync(doctor);
            }
            else
                errorModel = await _doctor!.EnrollDoctorAsync(doctor);

            ResetForm();
            await LoadDoctors();
        }

        private async Task LoadDoctors()
        {
            if (_doctor != null)
                doctors = await _doctor.GetAllDoctorsAsync();
            else
                doctors = [];

            errorModel = _doctor!.Error;
        }

        private void EditDoctor(DoctorViewModel model)
        {
            doctorVM = model;

            isEditing = true;
        }

        private void ConfirmDelete(DoctorViewModel model)
        {
            doctorToDelete = new Doctor
            {
                Id = model.Id!,
                Title = model.Title,
                FirstName = model.FirstName,
                LastName = model.LastName,
                IDNumber = model.IDNumber,
                Specialization = model.Specialization,
                HireDate = model.HireDate
            };

            showDeleteModal = true;
        }

        private async Task DeleteDoctor()
        {
            if (doctorToDelete is not null)
            {
                errorModel = await _doctor!.RemoveDoctorAsync(doctorToDelete);
                
                await LoadDoctors();
            }

            showDeleteModal = false;
        }

        private void ResetForm()
        {
            doctorVM = new DoctorViewModel
            {
                Title = string.Empty,
                FirstName = string.Empty,
                LastName = string.Empty,
                IDNumber = string.Empty,
                Specialization = string.Empty,
                HireDate = DateTime.Now,

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

            editingDoctorId = null;
            editingAddressId = null;
            editingContactId = null;

            isEditing = false;
        }
    }
}