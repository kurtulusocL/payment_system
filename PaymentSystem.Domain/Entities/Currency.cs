using System.Text.Json.Serialization;
using PaymentSystem.Domain.Entities.Base;

namespace PaymentSystem.Domain.Entities
{
    public class Currency:BaseEntity
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string? Symbol { get; set; } 
        public string? Description { get; set; }


        [JsonIgnore]
        public virtual ICollection<Payment> Payments { get; set; } = new HashSet<Payment>();

        [JsonIgnore]
        public virtual ICollection<Wallet> Wallets { get; set; } = new HashSet<Wallet>();

        [JsonIgnore]
        public virtual ICollection<Transaction> Transactions { get; set; } = new HashSet<Transaction>();
    }
}
