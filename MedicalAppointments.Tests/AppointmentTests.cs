using MedicalAppointments.Infrastructure.Interfaces;
using MedicalAppointments.Domain.Models;
using Moq;
using MedicalAppointments.Application.Services;

namespace MedicalAppointments.Tests
{
    [TestFixture]
    public class AppointmentTests
    {
        private Appointment _appointment;
        private List<Appointment> _appointments;

        private Mock<IRepository<Appointment>> _appointmentRepositoryMock;
        private Mock<AppointmentService> _appointmentServiceMock;

        [SetUp]
        public void Setup()
        {
            _appointmentRepositoryMock = new Mock<IRepository<Appointment>>();
            _appointmentServiceMock = new Mock<AppointmentService>();

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
                Doctor = doctor,  // Use real object
                Patient = patient // Use real object
            };

            _appointments =
            [
                new() {
                    Id = 1,
                    Date = DateTime.Now.AddDays(1),
                    Doctor = doctor,
                    Patient = patient
                },
                new() {
                Id = 2,
                Date = DateTime.Now.AddDays(2),
                    Doctor = doctor,
                    Patient = patient
                },
                _appointment,
            ];
        }

        [Test]
        public async Task GetAllAppointmentsAsync_ShouldReturnAppointments()
        {
            // Arrange
            _appointmentRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(_appointments);
            _appointmentServiceMock.Setup(service => service.GetAllAppointmentsAsync()).ReturnsAsync(_appointments);

            // Act
            var result = await _appointmentServiceMock.Object.GetAllAppointmentsAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task GetAppointmentByIdAsync_ShouldReturnAppointment_WhenExists()
        {
            // Arrange
            _appointmentRepositoryMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(_appointment);
            _appointmentServiceMock.Setup(service => service.GetAppointmentByIdAsync(1)).ReturnsAsync(_appointment);

            // Act
            var result = await _appointmentServiceMock.Object.GetAppointmentByIdAsync(1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(1));
        }

        [Test]
        public async Task BookAppointmentAsync_ShouldCallRepositoryOnce()
        {
            // Arrange
            _appointmentServiceMock.Setup(service => service.BookAppointmentAsync(_appointment)).Returns(Task.CompletedTask);

            // Act
            await _appointmentServiceMock.Object.BookAppointmentAsync(_appointment);

            // Assert
            _appointmentRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Appointment>()), Times.Once);
        }
    }
}
