using MedicalAppointments.Interfaces;
using MedicalAppointments.Shared.Models;
using Microsoft.AspNetCore.Components;
using MedicalAppointments.Shared.ViewModels;
using BlazorBootstrap;

namespace MedicalAppointments.Pages
{
    public partial class DoctorComponent
    {
        [Inject]
        public IDoctor? _doctor { get; set; }

        protected IEnumerable<DoctorViewModel>? doctors;
        private ErrorViewModel? errorModel;

        private Modal modalForm = default!;
        private Modal modalDelete = default!;

        List<ToastMessage> messages = [];

        private bool isSubmitting = false;

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

        private Doctor? doctorToDelete;

        protected override async Task OnInitializedAsync()
        {
            await LoadDoctors();
        }

        private async Task LoadDoctors()
        {
            if (_doctor != null)
                doctors = await _doctor.GetAllDoctorsAsync();
            else
                doctors = [];
        }

        private async Task HandleValidSubmit()
        {
            errorModel = null;
            isSubmitting = true;

            if (isEditing && !string.IsNullOrEmpty(editingDoctorId))
            {
                doctorVM.Id = editingDoctorId;
                errorModel = await _doctor!.UpdateDoctorAsync(doctorVM);

                if (errorModel.Errors != null && errorModel.Errors.Count > 0)
                    return;
            }
            else
            {
                errorModel = await _doctor!.EnrollDoctorAsync(doctorVM);

                if (errorModel.Errors != null && errorModel.Errors.Count > 0)
                    return;
            }

            SuccessMessage("Doctor saved successfully!");

            isSubmitting = false;
            ResetForm();
            await modalForm.HideAsync();
            await LoadDoctors();
        }

        private async Task ShowModalFormAsync()
        {
            ResetForm();
            await modalForm.ShowAsync();
        }

        private void HideModalFormAsync()
        {
            ResetForm();
            modalForm.HideAsync();
        }

        private void EditDoctor(DoctorViewModel model)
        {
            doctorVM = model;
            isEditing = true;

            modalForm.ShowAsync();
        }

        private async Task ConfirmDelete(DoctorViewModel model)
        {
            doctorToDelete = new Doctor
            {
                Id = model.Id!,
                Title = model.Title,
                FirstName = model.FirstName,
                LastName = model.LastName,
            };

            await modalDelete.ShowAsync();
        }

        private async Task DeleteDoctor()
        {
            errorModel = null;

            if (doctorToDelete is not null)
            {
                errorModel = await _doctor!.RemoveDoctorAsync(doctorToDelete.Id);

                if (errorModel.Errors != null && errorModel.Errors.Count > 0)
                    return;

                await LoadDoctors();
            }

            doctorToDelete = null;
            SuccessMessage("Doctor deleted successfully!");

            await modalDelete.HideAsync();
        }

        private void CancelDelete()
        {
            doctorToDelete = null;
            modalDelete.HideAsync();
        }

        private void SuccessMessage(string message)
        {
            messages.Add(new ToastMessage
            {
                Type = ToastType.Success,
                Title = "Notification",
                Message = message
            });
        }

        private void ResetForm()
        {
            errorModel = null;
            isSubmitting = false;

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
            isEditing = false;
        }
    }
}