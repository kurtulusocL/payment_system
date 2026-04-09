
namespace PaymentSystem.Shared.Dtos.MappingDtos.CurrencyDtos
{
    public class CurrencyUpdateDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string? Symbol { get; set; }
        public string? Description { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
