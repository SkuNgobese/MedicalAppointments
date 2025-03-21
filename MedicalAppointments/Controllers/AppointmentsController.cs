using MedicalAppointments.Application.Models;
using Microsoft.AspNetCore.Mvc;
using MedicalAppointments.Application.Interfaces;

namespace MedicalAppointments.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly IAppointment _service;

        public AppointmentsController(IAppointment service) => _service = service;

        public async Task<IActionResult> Index()
        {
            var appointments = await _service.GetAllAppointmentsAsync();
            return View(appointments);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Appointment appointment)
        {
            if (!ModelState.IsValid)
                return View(appointment);

            await _service.BookAppointmentAsync(appointment);
            return RedirectToAction(nameof(Index));
        }
    }
}
