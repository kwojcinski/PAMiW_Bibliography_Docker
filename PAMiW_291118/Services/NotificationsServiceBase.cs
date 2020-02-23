using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lib.AspNetCore.ServerSentEvents;

namespace PAMiW_291118.Services
{
    internal abstract class NotificationsServiceBase
    {
        private INotificationsServerSentEventsService _notificationsServerSentEventsService;

        protected NotificationsServiceBase(INotificationsServerSentEventsService notificationsServerSentEventsService)
        {
            _notificationsServerSentEventsService = notificationsServerSentEventsService;
        }

        protected Task SendSseEventAsync(string notification, bool alert, string href, string id)
        {
            var data = new List<string>();
            data.Add(id + "~");
            data.Add(notification);
            data.Add(href);
            return _notificationsServerSentEventsService.SendEventAsync(new ServerSentEvent
            {
                Type = alert ? "alert" : null,
                Data = data,
            });
        }
    }
}
