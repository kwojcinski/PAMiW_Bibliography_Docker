using System.Threading.Tasks;

namespace PAMiW_291118.Services
{
    internal class LocalNotificationsService : NotificationsServiceBase, INotificationsService
    {
        public LocalNotificationsService(INotificationsServerSentEventsService notificationsServerSentEventsService)
            : base(notificationsServerSentEventsService)
        { }

        public Task SendNotificationAsync(string notification, bool alert, string href, string id)
        {
            return SendSseEventAsync(notification, alert, href, id);
        }
    }
}
