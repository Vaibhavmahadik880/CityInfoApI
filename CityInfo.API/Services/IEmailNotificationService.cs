namespace CityInfo.API.Services
{
    public interface IEmailNotificationService
    {
        void Notify(string subject, string message);
    }
}
