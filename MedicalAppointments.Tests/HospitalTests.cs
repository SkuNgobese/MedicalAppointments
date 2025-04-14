using MedicalAppointments.Shared.Infrastructure.Interfaces;
using Moq;
using MedicalAppointments.Shared.Application.Services;
using MedicalAppointments.Shared.Models;
using System.Linq.Expressions;
using MedicalAppointments.Shared.Application.Interfaces;
using MedicalAppointments.Shared.Application.Interfaces;
using MedicalAppointments.Shared.Models;

namespace MedicalAppointments.Tests
{
    [TestFixture]
    public class HospitalTests
    {
        private Hospital _hospital;
        private List<Hospital> _hospitals;

        private Mock<IRepository<Hospital>> _hospitalRepositoryMock;
        private Mock<IDoctor> _doctorServiceMock;
        private Mock<IPatient> _patientServiceMock;
        private Mock<IAppointment> _appointmentServiceMock;
        private Mock<IAdmin> _sysAdminMock;
        private Mock<ICurrentUserHelper>? _currentUserHelperMock;
        private IHospital _hospitalService;

        [SetUp]
        public void Setup()
        {
            _hospitalRepositoryMock = new Mock<IRepository<Hospital>>();
            _doctorServiceMock = new Mock<IDoctor>();
            _patientServiceMock = new Mock<IPatient>();
            _appointmentServiceMock = new Mock<IAppointment>();
            _sysAdminMock = new Mock<IAdmin>();
            _currentUserHelperMock = new Mock<ICurrentUserHelper>();

            _hospitalService = new HospitalService(
                _hospitalRepositoryMock.Object,
                _doctorServiceMock.Object,
                _patientServiceMock.Object,
                _appointmentServiceMock.Object,
                _sysAdminMock.Object,
                _currentUserHelperMock!.Object);

            Address address = new()
            {
                Id = 1,
                Street = "123 Main St",
                City = "Pretoria",
                Suburb = "Heuweloord",
                PostalCode = "0001"
            };

            Contact contact = new()
            {
                Id = 2,
                ContactNumber = "string",
                Email = "string"
            };

            _hospital = new Hospital
            {
                Id = 1,
                Name = "City Hospital",
                Address = address
            };


            _hospitals =
            [
                new() { Id = 1, Name = "Raslouw", Address = address, Contact = contact },
                new() { Id = 2, Name = "St. Joseph", Address = address, Contact = contact }
            ];
        }

        [Test]
        public async Task GetAllHospitalsAsync_ShouldReturnHospitals()
        {
            // Arrange
            _hospitalRepositoryMock
                .Setup(repo => repo.GetAllAsync(It.IsAny<Expression<Func<Hospital, object>>[]>()))
                .ReturnsAsync(_hospitals);

            // Act
            var result = await _hospitalService.GetAllHospitalsAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Exactly(2).Items);
        }

        [Test]
        public async Task GetHospitalByIdAsync_ShouldReturnHospital_WhenExists()
        {
            // Arrange
            _hospitalRepositoryMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(_hospital);

            // Act
            var result = await _hospitalService.GetHospitalByIdAsync(1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result?.Id, Is.EqualTo(1));
                Assert.That(result?.Name, Is.EqualTo("City Hospital"));
            });
        }

        [Test]
        public async Task GetHospitalByIdAsync_ShouldReturnNull_WhenNotExists()
        {
            // Arrange
            _hospitalRepositoryMock.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync((Hospital?)null);

            // Act
            var result = await _hospitalService.GetHospitalByIdAsync(1);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task AddHospitalAsync_ShouldCallAddAsyncOnce()
        {
            // Act
            await _hospitalService.AddHospitalAsync(_hospital);

            // Assert
            _hospitalRepositoryMock.Verify(repo => repo.AddAsync(_hospital), Times.Once);
        }

        [Test]
        public async Task UpdateHospitalAsync_ShouldCallUpdateAsyncOnce()
        {
            // Act
            await _hospitalService.UpdateHospitalAsync(_hospital);

            // Assert
            _hospitalRepositoryMock.Verify(repo => repo.UpdateAsync(_hospital), Times.Once);
        }

        [Test]
        public async Task RemoveHospitalAsync_ShouldCallDeleteAsyncOnce()
        {
            // Act
            await _hospitalService.RemoveHospitalAsync(_hospital);

            // Assert
            _hospitalRepositoryMock.Verify(repo => repo.DeleteAsync(_hospital), Times.Once);
        }
    }
}
