using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PAMiW_291118.Services
{
    public interface INotificationsService
    {
        Task SendNotificationAsync(string notification, bool alert, string href, string id);
    }
}

