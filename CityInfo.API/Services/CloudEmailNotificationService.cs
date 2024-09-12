namespace CityInfo.API.Services
{
    public class CloudEmailNotificationService : IEmailNotificationService
    {
        private string _mailTo = string.Empty;
        private string _mailFrom = string.Empty;

        public CloudEmailNotificationService(IConfiguration configuration)
        {
            _mailTo = configuration["emailNotificationSettings:mailToAddress"];
            _mailFrom = configuration["emailNotificationSettings:mailFromAddress"];
        }

        public void Notify(string subject, string message)
        {
            // Simulate sending email by outputting to the console
            Console.WriteLine($"$\"Notification from {nameof(CloudEmailNotificationService)}");
            Console.WriteLine($"Subject: {subject}");
            Console.WriteLine($"Message: {message}");
        }
    }
}
