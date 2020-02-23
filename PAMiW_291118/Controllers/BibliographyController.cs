using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PAMiW_291118.Models;
using StackExchange.Redis;

namespace PAMiW_291118.Controllers
{
    public class BibliographyController : Controller
    {
        [Authorize]
        public IActionResult Index()
        {
            string id = HttpContext.User.Claims.FirstOrDefault(b => b.Type == ClaimTypes.NameIdentifier).Value;
            if (!CheckIfUserInRedis(id))
                return RedirectToAction("Denied", "Home");
            return View();
        }
        [Authorize]
        public IActionResult AddPosition(string userGuid)
        {
            string id = HttpContext.User.Claims.FirstOrDefault(b => b.Type == ClaimTypes.NameIdentifier).Value;
            if (!CheckIfUserInRedis(id))
                return RedirectToAction("Denied", "Home");
            UserViewModel model = new UserViewModel
            {
                Id = id
            };
            return View(model);
        }
        [Authorize]
        public IActionResult AddFile(string userGuid)
        {
            string id = HttpContext.User.Claims.FirstOrDefault(b => b.Type == ClaimTypes.NameIdentifier).Value;
            if (!CheckIfUserInRedis(id))
                return RedirectToAction("Denied", "Home");
            UserViewModel model = new UserViewModel
            {
                Id = userGuid
            };
            return View(model);
        }
        [Authorize]
        public async Task<ActionResult> ShowPositions(string userGuid)
        {
            string id = HttpContext.User.Claims.FirstOrDefault(b => b.Type == ClaimTypes.NameIdentifier).Value;
            if (!CheckIfUserInRedis(id))
                return RedirectToAction("Denied", "Home");
            HttpClient client = new HttpClient();
            var response = await client.GetAsync("http://web2:80/rest/GetPositions/" + userGuid);
            //var response = await client.GetAsync("https://localhost:44358/rest/GetPositions/" + userGuid);
            var data = await response.Content.ReadAsStringAsync();
            List<BibliographicPosition> positions = JsonConvert.DeserializeObject<List<BibliographicPosition>>(data);
            return View(positions);
        }
        [Authorize]
        public async Task<ActionResult> ShowFiles(string userGuid)
        {
            string id = HttpContext.User.Claims.FirstOrDefault(b => b.Type == ClaimTypes.NameIdentifier).Value;
            if (!CheckIfUserInRedis(id))
                return RedirectToAction("Denied", "Home");
            HttpClient client = new HttpClient();
            var response = await client.GetAsync("http://web2:80/rest/GetFiles/" + userGuid);
            //var response = await client.GetAsync("https://localhost:44358/rest/GetFiles/" + userGuid);
            var data = await response.Content.ReadAsStringAsync();
            List<File> files = JsonConvert.DeserializeObject<List<File>>(data);
            return View(files);
        }
        [Authorize]
        public async Task<ActionResult> ShowPosition(string link)
        {
            string id = HttpContext.User.Claims.FirstOrDefault(b => b.Type == ClaimTypes.NameIdentifier).Value;
            if (!CheckIfUserInRedis(id))
                return RedirectToAction("Denied", "Home");
            HttpClient client = new HttpClient();
            var response = await client.GetAsync(link);
            var data = await response.Content.ReadAsStringAsync();
            BibliographicPosition position = JsonConvert.DeserializeObject<BibliographicPosition>(data);
            return View(position);
        }
        [Authorize]
        public async Task<ActionResult> ConnectFile(string link1, string link2)
        {
            string id = HttpContext.User.Claims.FirstOrDefault(b => b.Type == ClaimTypes.NameIdentifier).Value;
            if (!CheckIfUserInRedis(id))
                return RedirectToAction("Denied", "Home");
            HttpClient client = new HttpClient();
            var response = await client.GetAsync(link1);
            var data = await response.Content.ReadAsStringAsync();
            BibliographicPosition position = JsonConvert.DeserializeObject<BibliographicPosition>(data);
            var response2 = await client.GetAsync(link2);
            var data2 = await response2.Content.ReadAsStringAsync();
            List<File> files = JsonConvert.DeserializeObject<List<File>>(data2);
            ConnectFileViewModel model = new ConnectFileViewModel
            {
                Position = position,
                Files = files
            };
            return View(model);
        }
        public static bool CheckIfUserInRedis(string id)
        {
            var redis = ConnectionMultiplexer.Connect("redis_image:6379");
            var db = redis.GetDatabase();
            var x = db.StringGet("user_" + id);
            var u = JsonConvert.DeserializeObject<User>(x.ToString());
            if (u.Logged)
                return true;
            else
                return false;
            //var redis = ConnectionMultiplexer.Connect("localhost:6379");
            //var db = redis.GetDatabase();
            //var keys = redis.GetServer("localhost", 6379).Keys();
            //foreach (var key in keys)
            //{
            //    if (key.ToString().StartsWith("user_"))
            //    {
            //        var x = db.StringGet(key.ToString());
            //        var u = JsonConvert.DeserializeObject<User>(x.ToString());
            //        if (u.Login.Equals(login))
            //        {
            //            if (u.Logged)
            //                return true;
            //            else
            //                return false;
            //        }
            //    }
            //}
            //return false;
        }
        }
}