using System.Text.Json.Serialization;
using PaymentSystem.Domain.Entities.Base;

namespace PaymentSystem.Domain.Entities
{
    public class PaymentStatus:BaseEntity
    {
        public string Name { get; set; }
        public string? Description { get; set; }

        [JsonIgnore]
        public virtual ICollection<Payment> Payments { get; set; } = new HashSet<Payment>();
    }
}
