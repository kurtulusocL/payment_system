using System.Text.Json.Serialization;
using PaymentSystem.Domain.Entities.Base;

namespace PaymentSystem.Domain.Entities
{
    public class TransactionType:BaseEntity
    {
        public string Name { get; set; }
        public string? Description { get; set; }

        [JsonIgnore]
        public virtual ICollection<Transaction> Transactions { get; set; } = new HashSet<Transaction>();
    }
}
