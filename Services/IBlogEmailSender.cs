using Microsoft.AspNetCore.Identity.UI.Services;

namespace BlogProj_12_10_22.Services
{
    public interface IBlogEmailSender : IEmailSender
    {
        Task SendContactEmailAsync(string emailFrom, string name, string subject, string htmlMessage);
    }
}
