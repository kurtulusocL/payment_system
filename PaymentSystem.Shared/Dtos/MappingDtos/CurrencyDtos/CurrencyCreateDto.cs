
namespace PaymentSystem.Shared.Dtos.MappingDtos.CurrencyDtos
{
    public class CurrencyCreateDto
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string? Symbol { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
