
namespace PaymentSystem.Shared.Dtos.MappingDtos.AppUserDtos
{
    public class AppUserGetDto
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string NameSurname { get; set; }
        public string Title { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime Birthdate { get; set; }
        public int? ConfirmCode { get; set; }
        public int AuditCount { get; set; }  
        public int PaymentCount { get; set; }
        public int UserSessionCount { get; set; }
        public bool IsConfirmCodeActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsDeleted { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public DateTime? UpdatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public DateTime? SuspendedDate { get; set; }
    }
}
