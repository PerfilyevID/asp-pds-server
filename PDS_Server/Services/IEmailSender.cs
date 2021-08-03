using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using PDS_Server.Local;
using RevitTeams_Server.Humanity;
using System.IO;
using System.Threading.Tasks;

namespace PDS_Server.Services
{
    public interface IEmailSender
    {
        public Task SendMessageAsync(Person person, string subject, EmailTemplate template, string format, string to, string from);
    }

    public class YandexSender : IEmailSender
    {
        public async Task SendMessageAsync(Person person, string subject, EmailTemplate template, string format, string to, string from="noreply")
        {
            var html = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), string.Format(@"Local\Html\{0}.html", template.ToString("G"))));
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress(string.Format($"{person.FirstName} from RevitTeams"), $"{from}@revit-teams.ru"));
            emailMessage.To.Add(new MailboxAddress("", to));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = string.Format(html, format, $"{person.FirstName} {person.LastName[0]}.", person.Email) };
            
            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.yandex.ru", 465, true);
                await client.AuthenticateAsync($"{from}@revit-teams.ru", ConfigurationManager.AppSetting.GetValue<string>("yandexMailPwd"));
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
        }
    }
}
