using MedicalAppointments.Domain.Interfaces;
using MedicalAppointments.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace MedicalAppointments.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly IAppointmentService _service;

        public AppointmentsController(IAppointmentService service) => _service = service;

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
