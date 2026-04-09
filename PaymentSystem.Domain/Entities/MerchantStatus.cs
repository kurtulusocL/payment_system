using System.Text.Json.Serialization;
using PaymentSystem.Domain.Entities.Base;

namespace PaymentSystem.Domain.Entities
{
    public class MerchantStatus:BaseEntity
    {
        public string Name { get; set; }
        public string? Description { get; set; }


        [JsonIgnore]
        public virtual ICollection<Merchant> Merchants { get; set; } = new HashSet<Merchant>();
    }
}
