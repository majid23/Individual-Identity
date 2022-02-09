using System.Diagnostics;
using System.Net.Mail;
using System.Net.Mime;

namespace Individual_Identity.Services.Email
{
    public class EmailSender : IEmailSender
    {
        private string userName;
        private string password;
        private readonly ILogger<EmailSender> _logger;
        private readonly IConfiguration Configuration;

        public EmailSender(IConfiguration configuration,
            ILogger<EmailSender> logger)
        {
            Configuration = configuration;
            this.userName = Configuration["EmailSender:UserName"];
            this.password = Configuration["EmailSender:Password"];
            _logger = logger;
        }

        public bool SendCode(string to, string code)
        {
            try
            {
                _logger.LogInformation(code);
#if DEBUG
                Debug.WriteLine($"The code was sent to email is ({code})");
#endif
                return true;

                // If you want send code via email, uncomment below code
                // Befor use set email configuration in appsettings
                /*
                #region formatter
                string text = $"This code was sent to confirm email\nCode : {code}\n\nIgnore this request if you did not submit it";
                string html = $"<div>This code was sent to confirm email</div><h3>Code : {code}</h3><br/><div>Ignore this request if you did not submit it</div>";

                #endregion

                MailMessage msg = new MailMessage();
                msg.From = new MailAddress(userName);
                msg.To.Add(new MailAddress(to));
                msg.Subject = "Change Email";
                msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(text, null, MediaTypeNames.Text.Plain));
                msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(html, null, MediaTypeNames.Text.Html));

                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", Convert.ToInt32(587));
                System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(userName, password);
                smtpClient.Credentials = credentials;
                smtpClient.EnableSsl = true;
                smtpClient.Send(msg);

                return true;
                */
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
