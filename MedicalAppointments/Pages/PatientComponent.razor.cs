using BlazorBootstrap;
using MedicalAppointments.Interfaces;
using MedicalAppointments.Shared.ViewModels;
using Microsoft.AspNetCore.Components;

namespace MedicalAppointments.Pages
{
    public partial class PatientComponent
    {
        [Inject]
        private IPatient? _patient { get; set; }

        private IEnumerable<PatientViewModel>? patients;
        private PatientViewModel? patientVM;
        private PatientViewModel? patientToDelete;
        private ErrorViewModel? errorModel;

        private List<string> titles = ["Dr", "Mr", "Mrs", "Miss", "Ms", "Prof"];

        private Modal modalForm = default!;
        private Modal modalDelete = default!;

        List<ToastMessage> messages = [];

        private bool isEditing = false;
        private bool isSubmitting = false;

        protected override async Task OnInitializedAsync()
        {
            patientVM = new()
            {
                Title = string.Empty,
                FirstName = string.Empty,
                LastName = string.Empty,
                IDNumber = string.Empty,
                ContactDetails = new ContactViewModel
                {
                    ContactNumber = string.Empty,
                    Email = string.Empty,
                    Fax = string.Empty
                },
                AddressDetails = new AddressViewModel
                {
                    Street = string.Empty,
                    Suburb = string.Empty,
                    City = string.Empty,
                    PostalCode = string.Empty,
                    Country = string.Empty
                }
            };

            await LoadPatientsAsync();
        }

        private async Task LoadPatientsAsync()
        {
            patients = await _patient!.GetAllPatientsAsync();
        }

        private void ShowModalFormAsync()
        {
            isEditing = false;
            modalForm.ShowAsync();
        }

        private void EditPatient(PatientViewModel model)
        {
            patientVM = model;
            isEditing = true;
            modalForm.ShowAsync();
        }

        private void CloseFormModal()
        {
            modalForm.HideAsync();
            errorModel = null;
        }

        private async Task HandleValidSubmit()
        {
            isSubmitting = true;

            if (isEditing)
                errorModel = await _patient!.UpdatePatientAsync(patientVM!);
            else
                patientVM = await _patient!.AddPatientAsync(patientVM!);

            isSubmitting = false;
            if (errorModel?.Message?.StartsWith("Success:", StringComparison.OrdinalIgnoreCase) == true)
            {
                await modalForm.HideAsync();
                await LoadPatientsAsync();
            }
        }

        private void ConfirmDelete(PatientViewModel patient)
        {
            patientToDelete = patient;
            modalDelete.ShowAsync();
        }

        private void CancelDelete()
        {
            patientToDelete = null;
            modalDelete.HideAsync();
        }

        private async Task DeleteConfirmed()
        {
            if (patientToDelete != null)
            {
                errorModel = await _patient!.RemovePatientAsync(patientToDelete!.Id!);
                if (errorModel?.Message?.StartsWith("Success:", StringComparison.OrdinalIgnoreCase) == true)
                {
                    await modalDelete.HideAsync();
                    await LoadPatientsAsync();
                }
            }
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
            patientVM = new()
            {
                Title = string.Empty,
                FirstName = string.Empty,
                LastName = string.Empty,
                IDNumber = string.Empty,
                ContactDetails = new ContactViewModel
                {
                    ContactNumber = string.Empty,
                    Email = string.Empty,
                    Fax = string.Empty
                },
                AddressDetails = new AddressViewModel
                {
                    Street = string.Empty,
                    Suburb = string.Empty,
                    City = string.Empty,
                    PostalCode = string.Empty,
                    Country = string.Empty
                }
            };
        }
    }
}
