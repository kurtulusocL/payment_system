using System.Text.Json.Serialization;
using PaymentSystem.Domain.Entities.Base;

namespace PaymentSystem.Domain.Entities
{
    public class Wallet : BaseEntity
    {
        public decimal Balance { get; set; } = 0;
        public byte[]? RowVersion { get; set; }

        public string UserId { get; set; }
        public int CurrencyId { get; set; }


        [JsonIgnore]
        public virtual AppUser User { get; set; } = null!;

        [JsonIgnore]
        public virtual Currency Currency { get; set; } = null!;

        [JsonIgnore]
        public virtual ICollection<Transaction> Transactions { get; set; } = new HashSet<Transaction>();
    }
}
