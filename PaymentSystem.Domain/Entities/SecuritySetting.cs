using PaymentSystem.Domain.Entities.Base;

namespace PaymentSystem.Domain.Entities
{
    public class SecuritySetting:BaseEntity
    {
        public string Type { get; set; }
        public string Value { get; set; }
    }
}
