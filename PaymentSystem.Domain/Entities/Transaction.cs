using System.Text.Json.Serialization;
using PaymentSystem.Domain.Entities.Base;

namespace PaymentSystem.Domain.Entities
{
    public class Transaction : BaseEntity
    {
        public decimal Amount { get; set; }
        public string? Reference { get; set; }

        public int WalletId { get; set; }
        public int? PaymentId { get; set; }
        public int CurrencyId { get; set; }
        public int TransactionTypeId { get; set; }


        [JsonIgnore]
        public virtual Wallet Wallet { get; set; } = null!;

        [JsonIgnore]
        public virtual Payment? Payment { get; set; }

        [JsonIgnore]
        public virtual Currency Currency { get; set; } = null!;

        [JsonIgnore]
        public virtual TransactionType TransactionType { get; set; } = null!;
    }
}
