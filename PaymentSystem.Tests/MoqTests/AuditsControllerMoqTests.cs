using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PaymentSystem.Api.Controllers;
using PaymentSystem.Application.Services.Abstract;
using PaymentSystem.Shared.Dtos.MappingDtos.AuditDtos;
using PaymentSystem.Shared.Results;

namespace PaymentSystem.Tests.MoqTests
{
    public class AuditsControllerMoqTests
    {
        private readonly Mock<IAuditService> _mock;
        private readonly AuditsController _ctrl;
        public AuditsControllerMoqTests()
        {
            _mock = new Mock<IAuditService>();
            _ctrl = new AuditsController(_mock.Object);
        }

        [Fact]
        public void GetAll_ReturnsOk()
        {
            _mock.Setup(x => x.GetAllIncluding()).Returns(new List<AuditGetDto>().AsQueryable());
            _ctrl.GetAllAudits().Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void GetByUserId_ReturnsOk()
        {
            _mock.Setup(x => x.GetAllIncludingByUserId(It.IsAny<string>())).Returns(new List<AuditGetDto>().AsQueryable());
            _ctrl.GetAllAuditsByUserId("1").Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void GetAllAdmin_ReturnsOk()
        {
            _mock.Setup(x => x.GetAllIncludingForAdmin()).Returns(new List<AuditGetDto>().AsQueryable());
            _ctrl.GetAllAuditsForAdmin().Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetById_Found_ReturnsOk()
        {
            _mock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(Result<AuditGetDto>.Success(new AuditGetDto()));
            (await _ctrl.GetAuditById(1)).Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetById_NotFound_ReturnsNotFound()
        {
            _mock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(Result<AuditGetDto>.Failure(""));
            (await _ctrl.GetAuditById(1)).Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task Delete_Success_ReturnsOk()
        {
            _mock.Setup(x => x.DeleteAsync(1)).ReturnsAsync(Result<bool>.Success(true));
            (await _ctrl.Delete(1)).Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Delete_Fail_ReturnsBadRequest()
        {
            _mock.Setup(x => x.DeleteAsync(1)).ReturnsAsync(Result<bool>.Failure(""));
            (await _ctrl.Delete(1)).Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task DeleteMultiple_ReturnsOk()
        {
            _mock.Setup(x => x.DeleteByIdAsync(It.IsAny<List<int>>())).ReturnsAsync(Result<bool>.Success(true));
            (await _ctrl.DeleteAuditsById(new() { 1 })).Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task SetActive_ReturnsOk()
        {
            _mock.Setup(x => x.SetActiveAsync(1)).ReturnsAsync(Result<bool>.Success(true));
            (await _ctrl.SetActive(1)).Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task SetInactive_ReturnsOk()
        {
            _mock.Setup(x => x.SetInActiveAsync(1)).ReturnsAsync(Result<bool>.Success(true));
            (await _ctrl.SetInactive(1)).Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task SoftDelete_ReturnsOk()
        {
            _mock.Setup(x => x.SetDeletedAsync(1)).ReturnsAsync(Result<bool>.Success(true));
            (await _ctrl.SoftDelete(1)).Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Restore_ReturnsOk()
        {
            _mock.Setup(x => x.SetNotDeletedAsync(1)).ReturnsAsync(Result<bool>.Success(true));
            (await _ctrl.Restore(1)).Should().BeOfType<OkObjectResult>();
        }
    }
}
