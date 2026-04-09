using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PaymentSystem.Api.Controllers;
using PaymentSystem.Application.Services.Abstract;
using PaymentSystem.Shared.Dtos.MappingDtos.WalletDtos;
using PaymentSystem.Shared.Results;

namespace PaymentSystem.Tests.MoqTests
{
    public class WalletsControllerMoqTests
    {
        private readonly Mock<IWalletService> _m;
        private readonly WalletsController _c;

        public WalletsControllerMoqTests()
        {
            _m = new Mock<IWalletService>();
            _c = new WalletsController(_m.Object);
        }

        [Fact]
        public void GetAll_ReturnsOk()
        {
            _m.Setup(x => x.GetAllIncluding()).Returns(new List<WalletGetDto>().AsQueryable());
            _c.GetAllWallets().Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void GetByUser_ReturnsOk()
        {
            _m.Setup(x => x.GetAllIncludingByUserId("user1")).Returns(new List<WalletGetDto>().AsQueryable());
            _c.GetAllWalletsByUserId("user1").Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void GetByCurrency_ReturnsOk()
        {
            _m.Setup(x => x.GetAllIncludingByCurrencyId(1)).Returns(new List<WalletGetDto>().AsQueryable());
            _c.GetAllWalletsByCurrencyId(1).Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void GetAllAdmin_ReturnsOk()
        {
            _m.Setup(x => x.GetAllIncludingForAdmin()).Returns(new List<WalletGetDto>().AsQueryable());
            _c.GetAllWalletsForAdmin().Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetById_Found_ReturnsOk()
        {
            _m.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(new WalletGetDto());
            (await _c.GetWalletById(1)).Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetById_NotFound_ReturnsNotFound()
        {
            _m.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((WalletGetDto?)null);
            (await _c.GetWalletById(1)).Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetForEdit_Found_ReturnsOk()
        {
            _m.Setup(x => x.GetByIdForUpdateAsync(1)).ReturnsAsync(new WalletGetDto());
            (await _c.GetWalletForEdit(1)).Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetForEdit_NotFound_ReturnsNotFound()
        {
            _m.Setup(x => x.GetByIdForUpdateAsync(1)).ReturnsAsync((WalletGetDto?)null);
            (await _c.GetWalletForEdit(1)).Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Create_Success_ReturnsOk()
        {
            _m.Setup(x => x.CreateAsync(It.IsAny<WalletCreateDto>())).ReturnsAsync(Result<bool>.Success(true));
            var dto = new WalletCreateDto
            {
                Balance = 100,
                UserId = "u1",
                CurrencyId = 1
            };
            (await _c.CreateWallet(dto)).Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Create_Failure_ReturnsBadRequest()
        {
            _m.Setup(x => x.CreateAsync(It.IsAny<WalletCreateDto>())).ReturnsAsync(Result<bool>.Failure("Error"));
            var dto = new WalletCreateDto
            {
                Balance = 100,
                UserId = "u1",
                CurrencyId = 1
            };
            (await _c.CreateWallet(dto)).Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Update_Success_ReturnsOk()
        {
            _m.Setup(x => x.UpdateAsync(It.IsAny<WalletUpdateDto>())).ReturnsAsync(Result<bool>.Success(true));
            var dto = new WalletUpdateDto
            {
                Id = 1,
                Balance = 100,
                UserId = "u1",
                CurrencyId = 1
            };
            (await _c.UpdateWallet(dto)).Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Update_Failure_ReturnsBadRequest()
        {
            _m.Setup(x => x.UpdateAsync(It.IsAny<WalletUpdateDto>())).ReturnsAsync(Result<bool>.Failure("Error"));
            var dto = new WalletUpdateDto
            {
                Id = 1,
                Balance = 100,
                UserId = "u1",
                CurrencyId = 1
            };
            (await _c.UpdateWallet(dto)).Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Delete_Success_ReturnsOk()
        {
            _m.Setup(x => x.DeleteAsync(1)).ReturnsAsync(Result<bool>.Success(true));
            (await _c.DeleteWallet(1)).Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task DeleteMultiple_Success_ReturnsOk()
        {
            _m.Setup(x => x.DeleteByIdAsync(It.IsAny<List<int>>())).ReturnsAsync(Result<bool>.Success(true));
            (await _c.DeleteWalletsById(new List<int> { 1, 2 })).Should().BeOfType<OkObjectResult>();
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
