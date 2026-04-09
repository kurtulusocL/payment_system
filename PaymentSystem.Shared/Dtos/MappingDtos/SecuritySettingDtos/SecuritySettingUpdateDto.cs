
namespace PaymentSystem.Shared.Dtos.MappingDtos.SecuritySettingDtos
{
    public class SecuritySettingUpdateDto
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
