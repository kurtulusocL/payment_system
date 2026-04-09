
namespace PaymentSystem.Shared.Dtos.MappingDtos.SecuritySettingDtos
{
    public class SecuritySettingGetDto
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsDeleted { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public DateTime? UpdatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public DateTime? SuspendedDate { get; set; }
    }
}
