using System.Text.Json.Serialization;
using PaymentSystem.Domain.Entities.Base;

namespace PaymentSystem.Domain.Entities
{
    public class Audit : BaseEntity
    {
        public string AreaAccessed { get; set; }
        public string? AppUserId { get; set; }

        [JsonIgnore]
        public virtual AppUser AppUser { get; set; }
    }
}
