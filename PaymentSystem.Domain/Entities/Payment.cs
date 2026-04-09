using System.Text.Json.Serialization;
using PaymentSystem.Domain.Entities.Base;

namespace PaymentSystem.Domain.Entities
{
    public class Payment:BaseEntity
    {      
        public decimal Amount { get; set; }       
        public string IdempotencyKey { get; set; }
        public string? Description { get; set; }
        public string? MaskedCardNumber { get; set; }

        public string UserId { get; set; }
        public int MerchantId { get; set; }
        public int CurrencyId { get; set; }
        public int PaymentStatusId { get; set; }

        [JsonIgnore]
        public virtual AppUser User { get; set; } = null!;

        [JsonIgnore]
        public virtual Merchant Merchant { get; set; } = null!;

        [JsonIgnore]
        public virtual Currency Currency { get; set; } = null!;

        [JsonIgnore]
        public virtual PaymentStatus PaymentStatus { get; set; } = null!;

        [JsonIgnore]
        public virtual ICollection<Transaction> Transactions { get; set; } = new HashSet<Transaction>();
    }
}
