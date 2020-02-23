using Lib.AspNetCore.ServerSentEvents;

namespace PAMiW_291118.Services
{
    internal class NotificationsServerSentEventsService : ServerSentEventsService, INotificationsServerSentEventsService
    {
        public NotificationsServerSentEventsService()
        {
            ChangeReconnectIntervalAsync(5000);
        }
    }
}
