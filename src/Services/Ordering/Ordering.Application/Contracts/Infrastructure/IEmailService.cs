namespace Ordering.Application.Contracts.Infrastructure
{

    using Ordering.Application.Model;

    public interface IEmailService
    {
        Task<bool> SendEmail(Email email);
    }
}
