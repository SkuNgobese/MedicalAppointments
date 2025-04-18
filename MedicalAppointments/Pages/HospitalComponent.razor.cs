using MedicalAppointments.Interfaces;
using MedicalAppointments.Shared.Models;
using Microsoft.AspNetCore.Components;
using MedicalAppointments.Shared.ViewModels;
using BlazorBootstrap;

namespace MedicalAppointments.Pages
{
    public partial class HospitalComponent
    {
        [Inject]
        private IHospital? _hospital { get; set; }

        protected IEnumerable<HospitalViewModel>? hospitals;

        private bool isEditing = false;
        private int? editingHospitalId = null;

        private Modal modalForm = default!;
        private Modal modalDelete = default!;

        private List<ToastMessage> messages = new();

        private Hospital? hospitalToDelete;

        private bool isSubmitting = false;

        private ErrorViewModel? errorModel;

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

        protected override async Task OnInitializedAsync() => 
            await LoadHospitalsAsync();

        private async Task LoadHospitalsAsync()
        {
            if (_hospital != null)
                hospitals = await _hospital.GetAllHospitalsAsync();
            else
                hospitals = [];
        }

        private async Task ShowModalFormAsync()
        {
            errorModel = null;
            ResetForm();
            await modalForm.ShowAsync();
        }

        private async Task HideModalFormAsync()
        {
            errorModel = null;
            ResetForm();
            await modalForm.HideAsync();
        }

        private async Task HandleValidSubmit()
        {
            errorModel = null;
            isSubmitting = true;

            if (isEditing && editingHospitalId.HasValue)
            {
                hospitalVM.Id = editingHospitalId;
                errorModel = await _hospital!.UpdateHospitalAsync(hospitalVM);

                if (errorModel.Errors != null && errorModel.Errors.Count > 0)
                {
                    isSubmitting = false;
                    return;
                }
            }
            else
            {
                errorModel = await _hospital!.AddHospitalAsync(hospitalVM);

                if (errorModel.Errors != null && errorModel.Errors.Count > 0)
                {
                    isSubmitting = false;
                    return;
                }
            }

            SuccessMessage(isEditing ? "Hospital updated successfully." : "Hospital added successfully.");

            isSubmitting = false;
            ResetForm();
            await modalForm.HideAsync();
            await LoadHospitalsAsync();
        }

        private async Task EditHospital(HospitalViewModel hospital)
        {
            hospitalVM = hospital;
            editingHospitalId = hospital.Id;
            isEditing = true;
            isSubmitting = false;
            await ShowModalFormAsync();
        }

        private async Task ConfirmDelete(HospitalViewModel hospitalVM)
        {
            hospitalToDelete = new Hospital
            {
                Id = (int)hospitalVM.Id!,
                Name = hospitalVM.HospitalName
            };

            await modalDelete.ShowAsync();
        }

        private async Task DeleteConfirmed()
        {
            if (hospitalToDelete is not null)
            {
                errorModel = await _hospital!.RemoveHospitalAsync(hospitalToDelete.Id);

                if (errorModel.Errors != null && errorModel.Errors.Count > 0)
                    return;

                SuccessMessage($"Hospital {hospitalToDelete.Name} deleted successfully.");

                hospitalToDelete = null;
                await modalDelete.HideAsync();
                await LoadHospitalsAsync();
            }
        }

        private void CancelDelete()
        {
            hospitalToDelete = null;
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