
namespace PaymentSystem.Application.Authorization.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class SkipOwnershipCheckAttribute : Attribute
    {
    }
}
