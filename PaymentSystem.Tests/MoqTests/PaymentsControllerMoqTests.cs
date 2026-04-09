using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PaymentSystem.Api.Controllers;
using PaymentSystem.Application.Services.Abstract;
using PaymentSystem.Shared.Dtos.MappingDtos.PaymentDtos;
using PaymentSystem.Shared.Results;

namespace PaymentSystem.Tests.MoqTests
{
    public class PaymentsControllerMoqTests
    {
        private readonly Mock<IPaymentService> _m;
        private readonly PaymentsController _c;

        public PaymentsControllerMoqTests()
        {
            _m = new Mock<IPaymentService>();
            _c = new PaymentsController(_m.Object);
        }

        [Fact]
        public void GetAll_ReturnsOk()
        {
            _m.Setup(x => x.GetAllIncluding()).Returns(new List<PaymentGetDto>().AsQueryable());
            _c.GetAllPayments().Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void GetByUser_ReturnsOk()
        {
            _m.Setup(x => x.GetAllIncludingByUserId("user1")).Returns(new List<PaymentGetDto>().AsQueryable());
            _c.GetAllPaymentsByUserId("user1").Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void GetByStatus_ReturnsOk()
        {
            _m.Setup(x => x.GetAllIncludingByPaymentStatusId(1)).Returns(new List<PaymentGetDto>().AsQueryable());
            _c.GetAllPaymentsByStatusId(1).Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void GetByCurrency_ReturnsOk()
        {
            _m.Setup(x => x.GetAllIncludingByCurrencyId(1)).Returns(new List<PaymentGetDto>().AsQueryable());
            _c.GetAllPaymentsByCurrencyId(1).Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void GetByMerchant_ReturnsOk()
        {
            _m.Setup(x => x.GetAllIncludingByMerchantId(1)).Returns(new List<PaymentGetDto>().AsQueryable());
            _c.GetAllPaymentsByMerchantId(1).Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void GetAllAdmin_ReturnsOk()
        {
            _m.Setup(x => x.GetAllIncludingForAdmin()).Returns(new List<PaymentGetDto>().AsQueryable());
            _c.GetAllPaymentsForAdmin().Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetById_Found_ReturnsOk()
        {
            _m.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(new List<PaymentGetDto> { new() });
            (await _c.GetPaymentById(1)).Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetById_NotFound_ReturnsNotFound()
        {
            _m.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((IEnumerable<PaymentGetDto>?)null);
            (await _c.GetPaymentById(1)).Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetForEdit_Found_ReturnsOk()
        {
            _m.Setup(x => x.GetByIdForUpdateAsync(1)).ReturnsAsync(new PaymentGetDto());
            (await _c.GetPaymentForEdit(1)).Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetForEdit_NotFound_ReturnsNotFound()
        {
            _m.Setup(x => x.GetByIdForUpdateAsync(1)).ReturnsAsync((PaymentGetDto?)null);
            (await _c.GetPaymentForEdit(1)).Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Create_Success_ReturnsOk()
        {
            _m.Setup(x => x.CreateAsync(It.IsAny<PaymentCreateDto>())).ReturnsAsync(Result<bool>.Success(true));
            var dto = new PaymentCreateDto
            {
                Amount = 100,
                IdempotencyKey = "key1",
                UserId = "u1",
                MerchantId = 1,
                CurrencyId = 1,
                PaymentStatusId = 1
            };
            (await _c.CreatePayment(dto)).Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Create_Failure_ReturnsBadRequest()
        {
            _m.Setup(x => x.CreateAsync(It.IsAny<PaymentCreateDto>())).ReturnsAsync(Result<bool>.Failure("Error"));
            var dto = new PaymentCreateDto
            {
                Amount = 100,
                IdempotencyKey = "key1",
                UserId = "u1",
                MerchantId = 1,
                CurrencyId = 1,
                PaymentStatusId = 1
            };
            (await _c.CreatePayment(dto)).Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Update_Success_ReturnsOk()
        {
            _m.Setup(x => x.UpdateAsync(It.IsAny<PaymentUpdateDto>())).ReturnsAsync(Result<bool>.Success(true));
            var dto = new PaymentUpdateDto
            {
                Id = 1,
                Amount = 100,
                IdempotencyKey = "key1",
                UserId = "u1",
                MerchantId = 1,
                CurrencyId = 1,
                PaymentStatusId = 1
            };
            (await _c.UpdatePayment(dto)).Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Update_Failure_ReturnsBadRequest()
        {
            _m.Setup(x => x.UpdateAsync(It.IsAny<PaymentUpdateDto>())).ReturnsAsync(Result<bool>.Failure("Error"));
            var dto = new PaymentUpdateDto
            {
                Id = 1,
                Amount = 100,
                IdempotencyKey = "key1",
                UserId = "u1",
                MerchantId = 1,
                CurrencyId = 1,
                PaymentStatusId = 1
            };
            (await _c.UpdatePayment(dto)).Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Delete_Success_ReturnsOk()
        {
            _m.Setup(x => x.DeleteAsync(1)).ReturnsAsync(Result<bool>.Success(true));
            (await _c.DeletePayment(1)).Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task DeleteMultiple_Success_ReturnsOk()
        {
            _m.Setup(x => x.DeleteByIdAsync(It.IsAny<List<int>>())).ReturnsAsync(Result<bool>.Success(true));
            (await _c.DeletePaymentsById(new List<int> { 1, 2 })).Should().BeOfType<OkObjectResult>();
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
