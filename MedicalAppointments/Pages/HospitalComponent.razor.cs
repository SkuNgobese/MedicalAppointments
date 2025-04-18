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

        private Modal modalMainForm = default!;
        private Modal modalDelete = default!;

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

        protected override async Task OnInitializedAsync()
        {
            ResetForm();
            await LoadHospitalsAsync();
        }

        private async Task LoadHospitalsAsync()
        {
            errorModel = null;

            if (_hospital != null)
                hospitals = await _hospital.GetAllHospitalsAsync();
            else
                hospitals = [];

            errorModel = _hospital!.Error;
        }

        private async Task OnShowModalFormClickAsync()
        {
            await modalMainForm.ShowAsync();
        }

        private async Task OnHideModalFormClickAsync()
        {
            ResetForm();
            await modalMainForm.HideAsync();
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
                
                errorModel = await _hospital!.AddHospitalAsync(hospital);

                if (errorModel.Errors != null && errorModel.Errors.Count > 0)
                {
                    isSubmitting = false;
                    return;
                }
            }

            await LoadHospitalsAsync();
            ResetForm();
            await OnHideModalFormClickAsync();

            isSubmitting = false;
        }

        private async Task EditHospital(HospitalViewModel hospital)
        {
            hospitalVM = hospital;

            editingHospitalId = hospital.Id;

            isEditing = true;
            await OnShowModalFormClickAsync();
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

                await LoadHospitalsAsync();
                hospitalToDelete = null;

                await modalDelete.HideAsync();
            }
        }

        private void CancelDelete()
        {
            hospitalToDelete = null;
            modalDelete.HideAsync();
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