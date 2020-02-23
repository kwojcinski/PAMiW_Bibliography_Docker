using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PAMiW_291118.Models;
using PAMiW_291118.Services;
using SSETutorial.Models;
using StackExchange.Redis;

namespace PAMiW_291118.Controllers
{
    public class NotificationController : Controller
    {
        private INotificationsService _notificationsService;

        public NotificationController(INotificationsService notificationsService)
        {
            _notificationsService = notificationsService;
        }
        [AcceptVerbs("POST")]
        public async Task<IActionResult> Sender(NotificationsSenderViewModel viewModel)
        {
            if (!String.IsNullOrEmpty(viewModel.Notification))
            {
                var redis = ConnectionMultiplexer.Connect("redis_image:6379");
                var db = redis.GetDatabase();
                var y = db.StringGet("user_" + viewModel.UserId);
                var z = JsonConvert.DeserializeObject<User>(y.ToString());
                viewModel.Name = z.Login;
                string notification = z.Login + " dodał nową publikację. Naciśnij to powiadomienie by do niej przejść.";
                string href = "http://localhost:8080/Bibliography/ShowPosition?link=" + viewModel.Href.Replace("localhost:8081","web2");
                await _notificationsService.SendNotificationAsync(notification, viewModel.Alert, href, z.Id);
            }

            ModelState.Clear();

            return RedirectToAction("Index", "Home");
        }
        public IActionResult Received()
        {
            return View();
        }
    }
}