namespace ASP_P26.Services.Email
{
    public interface IEmailService
    {
        void Send(String to, String subject, String content);
    }
}
