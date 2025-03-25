using Microsoft.AspNetCore.Mvc;
using MedicalAppointments.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using MedicalAppointments.Application.ViewModels;
using MedicalAppointments.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using MedicalAppointments.Application.Interfaces;
using MedicalAppointments.Domain.Interfaces.Shared;

namespace MedicalAppointments.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class HospitalsController : Controller
    {
        private readonly IHospital _hospital;
        private readonly IHospitalValidation _hospitalValidation;
        private readonly IAddress _address;
        private readonly IContact _contact;

        private readonly UserManager<User> _userManager;
        private readonly IUserStore<User> _userStore;
        private readonly IUserEmailStore<User> _emailStore;

        private readonly IUserService _userService;

        public HospitalsController(
            IHospital hospital, 
            IHospitalValidation hospitalValidation, 
            UserManager<User> userManager,
            IUserStore<User> userStore,
            IUserEmailStore<User> userEmailStore,
            IAddress address,
            IContact contact,
            IUserService userService)
        {
            _hospital = hospital;
            _hospitalValidation = hospitalValidation;
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _userStore = userStore;
            _address = address;
            _contact = contact;
            _userService = userService;
            _emailStore = _userService!.GetEmailStore();
        }

        // GET: Hospitals
        //public async Task<IActionResult> Index()
        //{
        //    return View(await _context.Hospitals.ToListAsync());
        //}

        //// GET: Hospitals/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var hospital = await _context.Hospitals
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (hospital == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(hospital);
        //}

        // GET: Hospitals/AddNew
        public IActionResult AddNew()
        {
            return View();
        }

        // POST: Hospitals/AddNew
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddNew(HospitalViewModel model)
        {
            if (ModelState.IsValid)
            {
                Address address = new()
                {
                    Street = model.AddressDetails.Street,
                    City = model.AddressDetails.City,
                    Suburb = model.AddressDetails.Suburb,
                    PostalCode = model.AddressDetails.PostalCode
                };
                await _address.AddAddress(address);

                Contact contact = new()
                {
                    PhoneNumber = model.ContactDetails.PhoneNumber,
                    Email = model.ContactDetails.Email,
                    Fax = model.ContactDetails.Fax
                };
                await _contact.AddContact(contact);

                Hospital hospital = new()
                {
                    Name = model.HospitalName,
                    Address = address,
                    Contact = contact
                };

                var hospitals = await _hospital.GetAllHospitalsAsync();

                if (_hospitalValidation.CanAddHospital(hospital, [.. hospitals]))
                {
                    await _hospital.AddHospitalAsync(hospital);
                    //Register the Hospital Admin as a user
                    await RegisterHospitalAdminAsync(hospital);
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(model);
        }

        // GET: Hospitals/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var hospital = await _context.Hospitals.FindAsync(id);
        //    if (hospital == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(hospital);
        //}

        //// POST: Hospitals/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Hospital hospital)
        //{
        //    if (id != hospital.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(hospital);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!HospitalExists(hospital.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(hospital);
        //}

        //// GET: Hospitals/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var hospital = await _context.Hospitals
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (hospital == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(hospital);
        //}

        //// POST: Hospitals/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var hospital = await _context.Hospitals.FindAsync(id);
        //    if (hospital != null)
        //    {
        //        _context.Hospitals.Remove(hospital);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool HospitalExists(int id)
        //{
        //    return _context.Hospitals.Any(e => e.Id == id);
        //}

        private async Task RegisterHospitalAdminAsync(Hospital hospital)
        {
            if (hospital.Contact?.Email == null)
                return;

            var existingUser = await _userManager.FindByEmailAsync(hospital.Contact.Email.ToUpper());

            if (existingUser != null)
                return;

            User user = _userService.CreateUser();

            user.UserName = hospital.Contact.Email;
            user.Title = "Mr/Mrs";
            user.FirstName = $"{hospital.Name}";
            user.LastName = "Admin";

            await _userStore.SetUserNameAsync(user, hospital.Contact.Email, CancellationToken.None);
            await _emailStore.SetEmailAsync(user, hospital.Contact.Email, CancellationToken.None);

            string generatedPassword = _userService.GenerateRandomPassword(12);
            var createUserResult = await _userManager.CreateAsync(user, generatedPassword);

            if (createUserResult.Succeeded)
                await _userManager.AddToRoleAsync(user, "Admin");
        }
    }
}
