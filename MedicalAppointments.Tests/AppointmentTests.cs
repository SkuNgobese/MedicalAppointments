using MedicalAppointments.Infrastructure.Interfaces;
using MedicalAppointments.Domain.Models;
using Moq;
using MedicalAppointments.Application.Services;
using MedicalAppointments.Domain.Interfaces;

namespace MedicalAppointments.Tests
{
    [TestFixture]
    public class AppointmentTests
    {
        private Appointment _appointment;
        private List<Appointment> _appointments;

        private Mock<IRepository<Appointment>> _appointmentRepositoryMock;
        private AppointmentService _appointmentService;

        [SetUp]
        public void Setup()
        {
            _appointmentRepositoryMock = new Mock<IRepository<Appointment>>();
            _appointmentService = new AppointmentService(_appointmentRepositoryMock.Object);
            // Create mock objects for Doctor and Patient
            var doctor = new Doctor
            {
                Id = 1,
                Specialization = "Cardiology"
            };

            var patient = new Patient
            {
                Id = 2
            };

            // Initialize Appointment with real objects
            _appointment = new Appointment
            {
                Id = 1,
                Date = DateTime.Now.AddDays(1),
                Doctor = doctor,
                Patient = patient,
                IsCancelled = false
            };

            _appointments =
            [
                new() {
                    Id = 1,
                    Date = DateTime.Now.AddDays(1),
                    Doctor = doctor,
                    Patient = patient,
                    IsCancelled = false
                },
                new() {
                    Id = 2,
                    Date = DateTime.Now.AddDays(2),
                    Doctor = doctor,
                    Patient = patient,
                    IsCancelled = false
                }
            ];
        }

        [Test]
        public async Task GetAllAppointmentsAsync_ShouldReturnAppointments()
        {
            // Arrange
            _appointmentRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(_appointments);

            // Act
            var result = await _appointmentService.GetAllAppointmentsAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Exactly(2).Items);
        }

        [Test]
        public async Task GetAppointmentByIdAsync_ShouldReturnAppointment_WhenExists()
        {
            // Arrange
            _appointment.Description = "Dental Checkup";
            _appointmentRepositoryMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(_appointment);

            // Act
            var result = await _appointmentService.GetAppointmentByIdAsync(1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result?.Id, Is.EqualTo(1));
                Assert.That(result?.Description, Is.EqualTo("Dental Checkup"));
            });
        }

        [Test]
        public async Task GetAppointmentByIdAsync_ShouldReturnNull_WhenNotExists()
        {
            // Arrange
            _appointmentRepositoryMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync((Appointment?)null);

            // Act
            var result = await _appointmentService.GetAppointmentByIdAsync(1);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task BookAppointmentAsync_ShouldCallAddAsyncOnce()
        {
            // Act
            await _appointmentService.BookAppointmentAsync(_appointment);

            // Assert
            _appointmentRepositoryMock.Verify(repo => repo.AddAsync(_appointment), Times.Once);
        }

        [Test]
        public async Task CancelAppointmentAsync_ShouldCallUpdateAsyncOnce()
        {
            // Arrange
            _appointment.IsCancelled = true;

            // Act
            await _appointmentService.CancelAppointmentAsync(_appointment);

            // Assert
            _appointmentRepositoryMock.Verify(repo => repo.UpdateAsync(_appointment), Times.Once);
        }
    }
}
