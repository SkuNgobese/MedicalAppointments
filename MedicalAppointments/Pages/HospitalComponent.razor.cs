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
        private int? editingAddressId = null;
        private int? editingContactId = null;

        private Modal modal = default!;

        private bool showDeleteModal = false;
        private Hospital? hospitalToDelete;

        private bool isSubmitting = false;

        private ErrorViewModel? errorModel;

        protected override async Task OnInitializedAsync()
        {
            await LoadHospitalsAsync();
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
    
        private async Task LoadHospitalsAsync()
        {
            if (_hospital != null)
                hospitals = await _hospital.GetAllHospitalsAsync();
            else
                hospitals = [];

            errorModel = _hospital!.Error;
        }

        private async Task OnShowModalClickAsync()
        {
            await modal.ShowAsync();
        }

        private async Task OnHideModalClickAsync()
        {
            ResetForm();
            await modal.HideAsync();
        }

        private async Task HandleValidSubmit()
        {
            errorModel = null;
            isSubmitting = true;

            if (isEditing && editingHospitalId.HasValue)
            {
                hospitalVM.Id = editingHospitalId;
                errorModel = await _hospital!.UpdateHospitalAsync(hospitalVM);
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
            }

            await LoadHospitalsAsync();
            ResetForm();
            await OnHideModalClickAsync();

            isSubmitting = false;
        }

        private async Task EditHospital(HospitalViewModel hospital)
        {
            hospitalVM = hospital;

            editingHospitalId = hospital.Id;
            editingAddressId = hospital.AddressDetails!.Id;
            editingContactId = hospital.ContactDetails?.Id;

            isEditing = true;
            await OnShowModalClickAsync();
        }

        private void ConfirmDelete(HospitalViewModel hospitalVM)
        {
            hospitalToDelete = new Hospital
            {
                Id = (int)hospitalVM.Id!,
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
                errorModel = await _hospital!.RemoveHospitalAsync(hospitalToDelete.Id);
                
                await LoadHospitalsAsync();
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