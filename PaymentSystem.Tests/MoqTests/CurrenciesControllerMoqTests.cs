using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PaymentSystem.Api.Controllers;
using PaymentSystem.Application.Services.Abstract;
using PaymentSystem.Shared.Dtos.MappingDtos.CurrencyDtos;
using PaymentSystem.Shared.Results;

namespace PaymentSystem.Tests.MoqTests
{
    public class CurrenciesControllerMoqTests
    {
        private readonly Mock<ICurrencyService> _m;
        private readonly CurrenciesController _c;

        public CurrenciesControllerMoqTests()
        {
            _m = new Mock<ICurrencyService>();
            _c = new CurrenciesController(_m.Object);
        }

        [Fact]
        public void GetAll_ReturnsOk()
        {
            _m.Setup(x => x.GetAllIncluding()).Returns(new List<CurrencyGetDto>().AsQueryable());
            _c.GetAllCurrencies().Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void GetByPayment_ReturnsOk()
        {
            _m.Setup(x => x.GetAllIncludingOrderByPayment()).Returns(new List<CurrencyGetDto>().AsQueryable());
            _c.GetAllCurrenciesByPayment().Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void GetByWallet_ReturnsOk()
        {
            _m.Setup(x => x.GetAllIncludingOrderByWallet()).Returns(new List<CurrencyGetDto>().AsQueryable());
            _c.GetAllCurrenciesByWallet().Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void GetByTransaction_ReturnsOk()
        {
            _m.Setup(x => x.GetAllIncludingOrderByTransaction()).Returns(new List<CurrencyGetDto>().AsQueryable());
            _c.GetAllCurrenciesByTransaction().Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void GetAllAdmin_ReturnsOk()
        {
            _m.Setup(x => x.GetAllIncludingForAdmin()).Returns(new List<CurrencyGetDto>().AsQueryable());
            _c.GetAllCurrenciesForAdmin().Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetById_Found_ReturnsOk()
        {
            _m.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(new CurrencyGetDto());
            (await _c.GetCurrencyById(1)).Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetById_NotFound_ReturnsNotFound()
        {
            _m.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((CurrencyGetDto?)null);
            (await _c.GetCurrencyById(1)).Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetForEdit_Found_ReturnsOk()
        {
            _m.Setup(x => x.GetByIdForUpdate(1)).ReturnsAsync(new CurrencyGetDto());
            (await _c.GetCurrencyForEdit(1)).Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetForEdit_NotFound_ReturnsNotFound()
        {
            _m.Setup(x => x.GetByIdForUpdate(1)).ReturnsAsync((CurrencyGetDto?)null);
            (await _c.GetCurrencyForEdit(1)).Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Create_Success_ReturnsOk()
        {
            _m.Setup(x => x.CreateAsync(It.IsAny<CurrencyCreateDto>())).ReturnsAsync(Result<bool>.Success(true));
            (await _c.CreateCurrency(new CurrencyCreateDto())).Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Create_Failure_ReturnsBadRequest()
        {
            _m.Setup(x => x.CreateAsync(It.IsAny<CurrencyCreateDto>())).ReturnsAsync(Result<bool>.Failure("Error"));
            (await _c.CreateCurrency(new CurrencyCreateDto())).Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Update_Success_ReturnsOk()
        {
            _m.Setup(x => x.UpdateAsync(It.IsAny<CurrencyUpdateDto>())).ReturnsAsync(Result<bool>.Success(true));
            (await _c.UpdateCurrency(new CurrencyUpdateDto())).Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Update_Failure_ReturnsBadRequest()
        {
            _m.Setup(x => x.UpdateAsync(It.IsAny<CurrencyUpdateDto>())).ReturnsAsync(Result<bool>.Failure("Error"));
            (await _c.UpdateCurrency(new CurrencyUpdateDto())).Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Delete_Success_ReturnsOk()
        {
            _m.Setup(x => x.DeleteAsync(1)).ReturnsAsync(Result<bool>.Success(true));
            (await _c.DeleteCurrency(1)).Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Delete_Failure_ReturnsBadRequest()
        {
            _m.Setup(x => x.DeleteAsync(1)).ReturnsAsync(Result<bool>.Failure("Error"));
            (await _c.DeleteCurrency(1)).Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task DeleteMultiple_Success_ReturnsOk()
        {
            _m.Setup(x => x.DeleteByIdAsync(It.IsAny<List<int>>())).ReturnsAsync(Result<bool>.Success(true));
            (await _c.DeleteCurrenciesById(new List<int> { 1, 2 })).Should().BeOfType<OkObjectResult>();
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
