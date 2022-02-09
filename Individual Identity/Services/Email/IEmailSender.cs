namespace Individual_Identity.Services.Email
{
    public interface IEmailSender
    {
        bool SendCode(string to, string code);
    }
}
