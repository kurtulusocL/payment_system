using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PaymentSystem.Api.Controllers;
using PaymentSystem.Application.Services.Abstract;
using PaymentSystem.Shared.Dtos.MappingDtos.TransactionDtos;
using PaymentSystem.Shared.Results;

namespace PaymentSystem.Tests.MoqTests
{
    public class TransactionsControllerMoqTests
    {
        private readonly Mock<ITransactionService> _m;
        private readonly TransactionsController _c;

        public TransactionsControllerMoqTests()
        {
            _m = new Mock<ITransactionService>();
            _c = new TransactionsController(_m.Object);
        }

        [Fact]
        public void GetAll_ReturnsOk()
        {
            _m.Setup(x => x.GetAllIncluding()).Returns(new List<TransactionGetDto>().AsQueryable());
            _c.GetAllTransactions().Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void GetByWallet_ReturnsOk()
        {
            _m.Setup(x => x.GetAllIncludingByWalletId(1)).Returns(new List<TransactionGetDto>().AsQueryable());
            _c.GetAllTransactionsByWalletId(1).Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void GetByPayment_ReturnsOk()
        {
            _m.Setup(x => x.GetAllIncludingByPaymentId(1)).Returns(new List<TransactionGetDto>().AsQueryable());
            _c.GetAllTransactionsByPaymentId(1).Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void GetByCurrency_ReturnsOk()
        {
            _m.Setup(x => x.GetAllIncludingByCurrencyId(1)).Returns(new List<TransactionGetDto>().AsQueryable());
            _c.GetAllTransactionsByCurrencyId(1).Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void GetByType_ReturnsOk()
        {
            _m.Setup(x => x.GetAllIncludingByTransactionTypeId(1)).Returns(new List<TransactionGetDto>().AsQueryable());
            _c.GetAllTransactionsByTransactionTypeId(1).Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void GetAllAdmin_ReturnsOk()
        {
            _m.Setup(x => x.GetAllIncludingForAdmin()).Returns(new List<TransactionGetDto>().AsQueryable());
            _c.GetAllTransactionsForAdmin().Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetById_Found_ReturnsOk()
        {
            _m.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(new TransactionGetDto());
            (await _c.GetTransactionById(1)).Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetById_NotFound_ReturnsNotFound()
        {
            _m.Setup(x => x.GetByIdAsync(1)).ReturnsAsync((TransactionGetDto?)null);
            (await _c.GetTransactionById(1)).Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetForEdit_Found_ReturnsOk()
        {
            _m.Setup(x => x.GetByIdForUpdate(1)).ReturnsAsync(new TransactionGetDto());
            (await _c.GetTransactionForEdit(1)).Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetForEdit_NotFound_ReturnsNotFound()
        {
            _m.Setup(x => x.GetByIdForUpdate(1)).ReturnsAsync((TransactionGetDto?)null);
            (await _c.GetTransactionForEdit(1)).Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Create_Success_ReturnsOk()
        {
            _m.Setup(x => x.CreateAsync(It.IsAny<TransactionCreateDto>())).ReturnsAsync(Result<bool>.Success(true));
            var dto = new TransactionCreateDto
            {
                Amount = 100,
                WalletId = 1,
                CurrencyId = 1,
                TransactionTypeId = 1
            };
            (await _c.CreateTransaction(dto)).Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Create_Failure_ReturnsBadRequest()
        {
            _m.Setup(x => x.CreateAsync(It.IsAny<TransactionCreateDto>())).ReturnsAsync(Result<bool>.Failure("Error"));
            var dto = new TransactionCreateDto
            {
                Amount = 100,
                WalletId = 1,
                CurrencyId = 1,
                TransactionTypeId = 1
            };
            (await _c.CreateTransaction(dto)).Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Update_Success_ReturnsOk()
        {
            _m.Setup(x => x.UpdateAsync(It.IsAny<TransactionUpdateDto>())).ReturnsAsync(Result<bool>.Success(true));
            var dto = new TransactionUpdateDto
            {
                Id = 1,
                Amount = 100,
                WalletId = 1,
                CurrencyId = 1,
                TransactionTypeId = 1
            };
            (await _c.UpdateTransaction(dto)).Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Update_Failure_ReturnsBadRequest()
        {
            _m.Setup(x => x.UpdateAsync(It.IsAny<TransactionUpdateDto>())).ReturnsAsync(Result<bool>.Failure("Error"));
            var dto = new TransactionUpdateDto
            {
                Id = 1,
                Amount = 100,
                WalletId = 1,
                CurrencyId = 1,
                TransactionTypeId = 1
            };
            (await _c.UpdateTransaction(dto)).Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Delete_Success_ReturnsOk()
        {
            _m.Setup(x => x.DeleteAsync(1)).ReturnsAsync(Result<bool>.Success(true));
            (await _c.DeleteTransaction(1)).Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task DeleteMultiple_Success_ReturnsOk()
        {
            _m.Setup(x => x.DeleteByIdAsync(It.IsAny<List<int>>())).ReturnsAsync(Result<bool>.Success(true));
            (await _c.DeleteTransactionsById(new List<int> { 1, 2 })).Should().BeOfType<OkObjectResult>();
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
