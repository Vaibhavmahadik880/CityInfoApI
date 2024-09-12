namespace CityInfo.API.Services
{
    public class LocalEmailNotificationService : IEmailNotificationService
    {
        private string _mailTo = string.Empty;
        private string _mailFrom = string.Empty;

        public LocalEmailNotificationService(IConfiguration configuration)
        {
            _mailTo = configuration["emailNotificationSettings:mailToAddress"];
            _mailFrom = configuration["emailNotificationSettings:mailFromAddress"];
        }

        public void Notify(string subject, string message)
        {
            // Simulate sending email by outputting to the console
            Console.WriteLine($"Notification from  {nameof(LocalEmailNotificationService)}");
            Console.WriteLine($"Subject: {subject}");
            Console.WriteLine($"Message: {message}");
        }
    }
}
