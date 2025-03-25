using MedicalAppointments.Domain.Interfaces;
using MedicalAppointments.Domain.Services;
using MedicalAppointments.Domain.Models;
using MedicalAppointments.Infrastructure.Interfaces;
using Moq;

namespace MedicalAppointments.Tests
{
    [TestFixture]
    public class DoctorTests
    {
        private Doctor _doctor;
        private List<Doctor> _doctors;

        private Mock<IRepository<Doctor>> _doctorRepositoryMock;
        private IDoctor _doctorService;

        [SetUp]
        public void Setup()
        {
            _doctorRepositoryMock = new Mock<IRepository<Doctor>>();
            _doctorService = new DoctorService(_doctorRepositoryMock.Object);

            _doctor = new Doctor
            {
                Id = "1",
                Title = "Dr",
                FirstName = "Innocent",
                LastName = "Ngobese",
                Specialization = "Cardiology",
                IsRetired = false
            };

            _doctors =
            [
                new() { Specialization = "Cardiology", IsRetired = false },
                new() { Specialization = "Neurology", IsRetired = false }
            ];
        }

        [Test]
        public async Task GetAllDoctorsAsync_ShouldReturnDoctors()
        {
            // Arrange
            _doctorRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(_doctors);

            // Act
            var result = await _doctorService.GetAllDoctorsAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Exactly(2).Items);
        }

        [Test]
        public async Task GetDoctorByIdAsync_ShouldReturnDoctor_WhenExists()
        {
            // Arrange
            _doctorRepositoryMock.Setup(repo => repo.GetByIdAsync("1")).ReturnsAsync(_doctor);

            // Act
            var result = await _doctorService.GetDoctorByIdAsync("1");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result?.Id, Is.EqualTo("1"));
                Assert.That(result?.FullName, Is.EqualTo("Dr. I Ngobese"));
            });
        }

        [Test]
        public async Task GetDoctorByIdAsync_ShouldReturnNull_WhenNotExists()
        {
            // Arrange
            _doctorRepositoryMock.Setup(repo => repo.GetByIdAsync("1")).ReturnsAsync((Doctor?)null);

            // Act
            var result = await _doctorService.GetDoctorByIdAsync("1");

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task EnrollDoctorAsync_ShouldCallAddAsyncOnce()
        {
            // Act
            await _doctorService.EnrollDoctorAsync(_doctor);

            // Assert
            _doctorRepositoryMock.Verify(repo => repo.AddAsync(_doctor), Times.Once);
        }

        [Test]
        public async Task UpdateDoctorAsync_ShouldCallUpdateAsyncOnce()
        {
            // Act
            await _doctorService.UpdateDoctorAsync(_doctor);

            // Assert
            _doctorRepositoryMock.Verify(repo => repo.UpdateAsync(_doctor), Times.Once);
        }

        [Test]
        public async Task RemoveDoctorAsync_ShouldCallDeleteAsyncOnce()
        {
            // Act
            await _doctorService.RemoveDoctorAsync(_doctor);

            // Assert
            _doctorRepositoryMock.Verify(repo => repo.DeleteAsync(_doctor), Times.Once);
        }
    }
}
