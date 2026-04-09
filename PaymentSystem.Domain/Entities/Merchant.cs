using System.Text.Json.Serialization;
using PaymentSystem.Domain.Entities.Base;

namespace PaymentSystem.Domain.Entities
{
    public class Merchant:BaseEntity
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? TaxNumber { get; set; }
        public Guid ApiKey { get; set; } = Guid.NewGuid();
        
        public int MerchantStatusId { get; set; }

        [JsonIgnore]
        public virtual MerchantStatus MerchantStatus { get; set; } = null!;

        [JsonIgnore]
        public virtual ICollection<Payment> Payments { get; set; } = new HashSet<Payment>();
    }
}
