using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PaymentSystem.Api.Controllers;
using PaymentSystem.Application.Services.Abstract;
using PaymentSystem.Shared.Dtos.MappingDtos.MerchantDtos;
using PaymentSystem.Shared.Results;

namespace PaymentSystem.Tests.MoqTests
{
    public class MerchantsControllerMoqTests
    {
        private readonly Mock<IMerchantService> _m;
        private readonly MerchantsController _c;

        public MerchantsControllerMoqTests()
        {
            _m = new Mock<IMerchantService>();
            _c = new MerchantsController(_m.Object);
        }

        [Fact]
        public void GetAll_ReturnsOk()
        {
            _m.Setup(x => x.GetAllIncluding()).Returns(new List<MerchantGetDto>().AsQueryable());
            _c.GetAllMerchants().Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void GetByStatus_ReturnsOk()
        {
            _m.Setup(x => x.GetAllIncludingByStatusId(1)).Returns(new List<MerchantGetDto>().AsQueryable());
            _c.GetAllMerchantsByStatusId(1).Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void GetNullTaskNumbers_ReturnsOk()
        {
            _m.Setup(x => x.GetAllIncludingByNullTaskNumber()).Returns(new List<MerchantGetDto>().AsQueryable());
            _c.GetAllMerchantsByNullTaskNumber().Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void GetByPayment_ReturnsOk()
        {
            _m.Setup(x => x.GetAllIncludingByPayment()).Returns(new List<MerchantGetDto>().AsQueryable());
            _c.GetAllMerchantsByPayment().Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void GetAllAdmin_ReturnsOk()
        {
            _m.Setup(x => x.GetAllIncludingForAdmin()).Returns(new List<MerchantGetDto>().AsQueryable());
            _c.GetAllMerchantsForAdmin().Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetById_Found_ReturnsOk()
        {
            _m.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(new List<MerchantGetDto> { new() });
            (await _c.GetMerchantById(1)).Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetById_NotFound_ReturnsNotFound()
        {
            _m.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((IEnumerable<MerchantGetDto>?)null);
            (await _c.GetMerchantById(1)).Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetForEdit_Found_ReturnsOk()
        {
            _m.Setup(x => x.GetByIdForUpdateAsync(1)).ReturnsAsync(new MerchantUpdateDto());
            (await _c.GetMerchantForEdit(1)).Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetForEdit_NotFound_ReturnsNotFound()
        {
            _m.Setup(x => x.GetByIdForUpdateAsync(1)).ReturnsAsync((MerchantUpdateDto?)null);
            (await _c.GetMerchantForEdit(1)).Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Create_Success_ReturnsOk()
        {
            _m.Setup(x => x.CreateAsync(It.IsAny<MerchantCreateDto>())).ReturnsAsync(Result<bool>.Success(true));
            (await _c.CreateMerchant(new MerchantCreateDto())).Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Create_Failure_ReturnsBadRequest()
        {
            _m.Setup(x => x.CreateAsync(It.IsAny<MerchantCreateDto>())).ReturnsAsync(Result<bool>.Failure("Error"));
            (await _c.CreateMerchant(new MerchantCreateDto())).Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Update_Success_ReturnsOk()
        {
            _m.Setup(x => x.UpdateAsync(It.IsAny<MerchantUpdateDto>())).ReturnsAsync(Result<bool>.Success(true));
            (await _c.UpdateMerchant(new MerchantUpdateDto())).Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Update_Failure_ReturnsBadRequest()
        {
            _m.Setup(x => x.UpdateAsync(It.IsAny<MerchantUpdateDto>())).ReturnsAsync(Result<bool>.Failure("Error"));
            (await _c.UpdateMerchant(new MerchantUpdateDto())).Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Delete_Success_ReturnsOk()
        {
            _m.Setup(x => x.DeleteAsync(1)).ReturnsAsync(Result<bool>.Success(true));
            (await _c.DeleteMerchant(1)).Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task DeleteMultiple_Success_ReturnsOk()
        {
            _m.Setup(x => x.DeleteByIdAsync(It.IsAny<List<int>>())).ReturnsAsync(Result<bool>.Success(true));
            (await _c.DeleteMerchantsById(new List<int> { 1, 2 })).Should().BeOfType<OkObjectResult>();
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
