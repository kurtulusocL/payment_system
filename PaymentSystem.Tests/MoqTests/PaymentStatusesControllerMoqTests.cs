using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PaymentSystem.Api.Controllers;
using PaymentSystem.Application.Services.Abstract;
using PaymentSystem.Shared.Dtos.MappingDtos.PaymentStatusDtos;
using PaymentSystem.Shared.Results;

namespace PaymentSystem.Tests.MoqTests
{
    public class PaymentStatusesControllerMoqTests
    {
        private readonly Mock<IPaymentStatusService> _m;
        private readonly PaymentStatusesController _c;

        public PaymentStatusesControllerMoqTests()
        {
            _m = new Mock<IPaymentStatusService>();
            _c = new PaymentStatusesController(_m.Object);
        }

        [Fact]
        public void GetAll_ReturnsOk()
        {
            _m.Setup(x => x.GetAllIncluding()).Returns(new List<PaymentStatusGetDto>().AsQueryable());
            _c.GetAllPaymentStatuses().Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void GetByPayment_ReturnsOk()
        {
            _m.Setup(x => x.GetAllIncludingOrderByPayments()).Returns(new List<PaymentStatusGetDto>().AsQueryable());
            _c.GetAllPaymentStatusesByPayments().Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void GetAllAdmin_ReturnsOk()
        {
            _m.Setup(x => x.GetAllIncludingForAdmin()).Returns(new List<PaymentStatusGetDto>().AsQueryable());
            _c.GetAllPaymentStatusesForAdmin().Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetById_Found_ReturnsOk()
        {
            _m.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(new PaymentStatusGetDto());
            (await _c.GetPaymentStatusById(1)).Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetById_NotFound_ReturnsNotFound()
        {
            _m.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((PaymentStatusGetDto?)null);
            (await _c.GetPaymentStatusById(1)).Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetForEdit_Found_ReturnsOk()
        {
            _m.Setup(x => x.GetByIdForUpdateAsync(1)).ReturnsAsync(new PaymentStatusGetDto());
            (await _c.GetPaymentStatusForEdit(1)).Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetForEdit_NotFound_ReturnsNotFound()
        {
            _m.Setup(x => x.GetByIdForUpdateAsync(1)).ReturnsAsync((PaymentStatusGetDto?)null);
            (await _c.GetPaymentStatusForEdit(1)).Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Create_Success_ReturnsOk()
        {
            _m.Setup(x => x.CreateAsync(It.IsAny<PaymentStatusCreateDto>())).ReturnsAsync(Result<bool>.Success(true));
            (await _c.CreatePaymentStatus(new PaymentStatusCreateDto())).Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Create_Failure_ReturnsBadRequest()
        {
            _m.Setup(x => x.CreateAsync(It.IsAny<PaymentStatusCreateDto>())).ReturnsAsync(Result<bool>.Failure("Error"));
            (await _c.CreatePaymentStatus(new PaymentStatusCreateDto())).Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Update_Success_ReturnsOk()
        {
            _m.Setup(x => x.UpdateAsync(It.IsAny<PaymentStatusUpdateDto>())).ReturnsAsync(Result<bool>.Success(true));
            (await _c.UpdatePaymentStatus(new PaymentStatusUpdateDto())).Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Update_Failure_ReturnsBadRequest()
        {
            _m.Setup(x => x.UpdateAsync(It.IsAny<PaymentStatusUpdateDto>())).ReturnsAsync(Result<bool>.Failure("Error"));
            (await _c.UpdatePaymentStatus(new PaymentStatusUpdateDto())).Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Delete_Success_ReturnsOk()
        {
            _m.Setup(x => x.DeleteAsync(1)).ReturnsAsync(Result<bool>.Success(true));
            (await _c.DeletePaymentStatus(1)).Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task DeleteMultiple_Success_ReturnsOk()
        {
            _m.Setup(x => x.DeleteByIdAsync(It.IsAny<List<int>>())).ReturnsAsync(Result<bool>.Success(true));
            (await _c.DeletePaymentStatusesById(new List<int> { 1, 2 })).Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task SetActive_Success_ReturnsOk()
        {
            _m.Setup(x => x.SetActiveAsync(1)).ReturnsAsync(Result<bool>.Success(true));
            (await _c.SetActive(1)).Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task SetInactive_Success_ReturnsOk()
        {
            _m.Setup(x => x.SetInActiveAsync(1)).ReturnsAsync(Result<bool>.Success(true));
            (await _c.SetInactive(1)).Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task SoftDelete_Success_ReturnsOk()
        {
            _m.Setup(x => x.SetDeletedAsync(1)).ReturnsAsync(Result<bool>.Success(true));
            (await _c.SoftDelete(1)).Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Restore_Success_ReturnsOk()
        {
            _m.Setup(x => x.SetNotDeletedAsync(1)).ReturnsAsync(Result<bool>.Success(true));
            (await _c.Restore(1)).Should().BeOfType<OkObjectResult>();
        }
    }
}
