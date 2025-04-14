using Moq;
using MedicalAppointments.Api.Application.Services;
using MedicalAppointments.Shared.Models;
using MedicalAppointments.Api.Infrastructure.Interfaces;
using MedicalAppointments.Api.Application.Interfaces;

namespace MedicalAppointments.Tests
{
    [TestFixture]
    public class PatientTests
    {
        private Patient _patient;
        private List<Patient> _patients;

        private Mock<IRepository<Patient>> _patientRepositoryMock;
        private IPatient _patientService;

        [SetUp]
        public void Setup()
        {
            _patientRepositoryMock = new Mock<IRepository<Patient>>();
            _patientService = new PatientService(_patientRepositoryMock.Object);

            _patient = new Patient
            {
                Id = "1",
                Title = "Mr",
                FirstName = "Innocent",
                LastName = "Ngobese",
                IsActive = true,
                Hospital = new Hospital
                {
                    Id = 1,
                    Name = "City Hospital"
                }
            };

            _patients = new List<Patient>
            {
                new() { Id = "1", IsActive = true, Hospital = _patient.Hospital },
                new() { Id = "2", IsActive = true, Hospital = _patient.Hospital }
            };
        }

        [Test]
        public async Task GetAllPatientsAsync_ShouldReturnPatients()
        {
            // Arrange
            _patientRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(_patients);

            // Act
            var result = await _patientService.GetAllPatientsAsync(_patient.Hospital!);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Exactly(2).Items);
        }

        [Test]
        public async Task GetPatientByIdAsync_ShouldReturnPatient_WhenExists()
        {
            // Arrange
            _patientRepositoryMock.Setup(repo => repo.GetByIdAsync("1")).ReturnsAsync(_patient);

            // Act
            var result = await _patientService.GetPatientByIdAsync("1");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(result?.Id, Is.EqualTo("1"));
                Assert.That(result?.FullName, Is.EqualTo("Mr. I Ngobese"));
            });
        }

        [Test]
        public async Task GetPatientByIdAsync_ShouldReturnNull_WhenNotExists()
        {
            // Arrange
            _patientRepositoryMock.Setup(repo => repo.GetByIdAsync("1")).ReturnsAsync((Patient?)null);

            // Act
            var result = await _patientService.GetPatientByIdAsync("1");

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task AddPatientAsync_ShouldCallAddAsyncOnce()
        {
            // Act
            await _patientService.AddPatientAsync(_patient);

            // Assert
            _patientRepositoryMock.Verify(repo => repo.AddAsync(_patient), Times.Once);
        }

        [Test]
        public async Task UpdatePatientAsync_ShouldCallUpdateAsyncOnce()
        {
            // Act
            await _patientService.UpdatePatientAsync(_patient);

            // Assert
            _patientRepositoryMock.Verify(repo => repo.UpdateAsync(_patient), Times.Once);
        }

        [Test]
        public async Task RemovePatientAsync_ShouldCallDeleteAsyncOnce()
        {
            // Act
            await _patientService.RemovePatientAsync(_patient);

            // Assert
            _patientRepositoryMock.Verify(repo => repo.DeleteAsync(_patient), Times.Once);
        }
    }
}