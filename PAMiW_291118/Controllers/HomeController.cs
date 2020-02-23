using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PAMiW_291118.Models;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using StackExchange.Redis;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using SSETutorial.Models;

namespace PAWB_L1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IHostingEnvironment _env;
        private static readonly HttpClient client = new HttpClient();

        public HomeController(ILogger<HomeController> logger, IHostingEnvironment env)
        {
            _env = env;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Register()
        {
            UserViewModel model = new UserViewModel();
            return View(model);
        }
        [HttpPost]
        public async Task<ActionResult> Register(UserViewModel model) 
        {
            if (!ModelState.IsValid)
                return View(model);
            if (String.IsNullOrEmpty(model.Email))
            {
                ModelState.AddModelError("Email", "Podaj login.");
                return View(model);
            }
            if (String.IsNullOrEmpty(model.Login))
            {
                ModelState.AddModelError("Login", "Podaj email.");
                return View(model);
            }
            if (String.IsNullOrEmpty(model.Passwd))
            {
                ModelState.AddModelError("Passwd", "Podaj hasło.");
                return View(model);
            }
            var redis = ConnectionMultiplexer.Connect("redis_image:6379");
            var db = redis.GetDatabase();
            var keys = redis.GetServer("redis_image", 6379).Keys();
            foreach (var key in keys)
            {
                if (key.ToString().StartsWith("user_"))
                {
                    var x = db.StringGet(key.ToString());
                    var u = JsonConvert.DeserializeObject<User>(x.ToString());
                    if(u.Login.Equals(model.Login))
                    {
                        ModelState.AddModelError("Login", "Istnieje już użytkownik o takim loginie.");
                        return View(model);
                    }
                    if (u.Email.Equals(model.Email))
                    {
                        ModelState.AddModelError("Email", "Istnieje już użytkownik o takim emailu.");
                        return View(model);
                    }
                }
            }
            //string line;
            //var webrootFolder = _env.WebRootPath;
            //try
            //{
            //    using (StreamReader file = new StreamReader(webrootFolder + @"\users.txt"))
            //    {
            //        while ((line = file.ReadLine()) != null)
            //        {
            //            int first = line.IndexOf(";");
            //            int second = line.IndexOf(';', line.IndexOf(';') + 1);
            //            string email = Slice(line, 0, first);
            //            string login = Slice(line, first + 1, second);
            //            if (login.Equals(model.Login))
            //            {
            //                ModelState.AddModelError("Login", "Istnieje już użytkownik o takim loginie.");
            //                return View(model);
            //            }
            //            if (email.Equals(model.Email))
            //            {
            //                ModelState.AddModelError("Email", "Istnieje już użytkownik o takim emailu.");
            //                return View(model);
            //            }
            //        }
            //    }
            //}
            //catch(Exception)
            //{
            //    using (FileStream fs = System.IO.File.Create(webrootFolder + @"\users.txt"))
            //    {
            //    }
            //}
            User user = new User
            {
                Id = Guid.NewGuid().ToString(),
                Login = model.Login,
                Email = model.Email,
                Passwd = model.Passwd,
                Positions = new List<string>(),
                Files = new List<string>(),
                Logged = true
            };
            var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, model.Email),
                        new Claim(ClaimTypes.Name, model.Login),
                        new Claim(ClaimTypes.Email, model.Email)
                    };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(principal);

            var serialized = JsonConvert.SerializeObject(user);
            db.StringSet("user_" + user.Id, serialized);

            return RedirectToAction("Index");
        }
        //public IActionResult Login()
        //{
        //    return View();
        //}
        //[HttpPost]
        //public async Task<ActionResult> Login(UserViewModel model)
        //{
        //    if (!ModelState.IsValid)
        //        return View(model);
        //    if (String.IsNullOrEmpty(model.Login))
        //    {
        //        ModelState.AddModelError("Login", "Podaj email.");
        //        return View(model);W
        //    }
        //    if (String.IsNullOrEmpty(model.Passwd))
        //    {
        //        ModelState.AddModelError("Passwd", "Podaj hasło.");
        //        return View(model);
        //    }
        //    var redis = ConnectionMultiplexer.Connect("redis_image:6379");
        //    var db = redis.GetDatabase();
        //    var keys = redis.GetServer("redis_image", 6379).Keys();
        //    bool logged = false;
        //    foreach (var key in keys)
        //    {
        //        if (key.ToString().StartsWith("user_"))
        //        {
        //            var x = db.StringGet(key.ToString());
        //            var u = JsonConvert.DeserializeObject<User>(x.ToString());
        //            if (u.Login.Equals(model.Login))
        //            {
        //                if(u.Passwd.Equals(model.Passwd))
        //                {
        //                    logged = true;
        //                    var claims = new List<Claim>
        //                    {
        //                        new Claim(ClaimTypes.NameIdentifier, u.Email),
        //                        new Claim(ClaimTypes.Name, u.Login),
        //                        new Claim(ClaimTypes.Email, u.Email)
        //                    };
        //                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        //                    var principal = new ClaimsPrincipal(identity);

        //                    await HttpContext.SignInAsync(principal);

        //                    u.Logged = true;

        //                    var serialized = JsonConvert.SerializeObject(u);
        //                    db.StringSet("user_" + u.Id, serialized);
        //                }
        //                else
        //                {
        //                    ModelState.AddModelError("Passwd", "Niepoprawne hasło.");
        //                    return View(model);
        //                }

        //            }
        //        }
        //    }
        //    if (!logged)
        //    {
        //        ModelState.AddModelError("Login", "Nie istnieje użytkownik o takim loginie.");
        //        return View(model);
        //    }
        //    //try
        //    //{
        //    //    using (StreamReader file = new StreamReader(webrootFolder + @"\users.txt"))
        //    //    {
        //    //        while ((line = file.ReadLine()) != null)
        //    //        {
        //    //            int first = line.IndexOf(";");
        //    //            int second = line.IndexOf(';', line.IndexOf(';') + 1);
        //    //            var x = Slice(line, first + 1, second);
        //    //            if (x.Equals(model.Login))
        //    //            {
        //    //                found = line;
        //    //            }
        //    //        }
        //    //    }
        //    //    if (!String.IsNullOrEmpty(found))
        //    //    {
        //    //        int fir = found.IndexOf(";");
        //    //        int sec = found.IndexOf(';', found.IndexOf(';') + 1);
        //    //        int thi = found.IndexOf(';', found.IndexOf(';', found.IndexOf(';') + 1) + 1);
        //    //        string email = Slice(found, 0, fir);
        //    //        string login = Slice(found, fir + 1, sec);
        //    //        string pass = Slice(found, sec + 1, thi);
        //    //        string sid = Slice(found, thi + 1, found.Length);
        //    //        if (login == model.Login && pass == model.Passwd)
        //    //        {
        //    //            var claims = new List<Claim>
        //    //        {
        //    //            new Claim(ClaimTypes.NameIdentifier, email),
        //    //            new Claim(ClaimTypes.Name, login),
        //    //            new Claim(ClaimTypes.Email, email)
        //    //        };
        //    //            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        //    //            var principal = new ClaimsPrincipal(identity);

        //    //            await HttpContext.SignInAsync(principal);

        //    //            var redis = ConnectionMultiplexer.Connect("redis_image:6379");
        //    //            var db = redis.GetDatabase();

        //    //            db.StringSet(sid, "true");
        //    //        }
        //    //        else
        //    //        {
        //    //            ModelState.AddModelError("Passwd", "Niepoprawne hasło");
        //    //            return View(model);
        //    //        }
        //    //    }
        //    //    else
        //    //    {
        //    //        ModelState.AddModelError("Login", "Nie ma takiego użytkownika");
        //    //        return View(model);
        //    //    }
        //    //}
        //    //catch(Exception)
        //    //{
        //    //    ModelState.AddModelError("Login", "Nie ma takiego użytkownika");
        //    //    return View(model);
        //    //}

        //    return RedirectToAction("Index");
        //}
        [Authorize]
        public IActionResult Profile()
        {
            string id = HttpContext.User.Claims.FirstOrDefault(b => b.Type == ClaimTypes.NameIdentifier).Value;
            if (!CheckIfUserInRedis(id))
                return RedirectToAction("Denied");
            UserViewModel model = new UserViewModel
            {
                Id = id
            };
            return View(model);
            //string found = "";
            //string line;
            //try
            //{
            //    using (StreamReader file = new StreamReader(webrootFolder + @"\users.txt"))
            //    {
            //        while ((line = file.ReadLine()) != null)
            //        {
            //            int first = line.IndexOf(";");
            //            int second = line.IndexOf(';', line.IndexOf(';') + 1);
            //            var x = Slice(line, first + 1, second);
            //            if (x.Equals(login))
            //            {
            //                found = line;
            //            }
            //        }
            //    }
            //    if (!String.IsNullOrEmpty(found))
            //    {
            //        string line2;
            //        UserViewModel model = new UserViewModel();
            //        model.PDFs = new List<PDF>();
            //        try
            //        {
            //            using (StreamReader file = new StreamReader(webrootFolder + @"\pdfs.txt"))
            //            {
            //                while ((line2 = file.ReadLine()) != null)
            //                {
            //                    int first = line2.IndexOf(";");
            //                    int second = line2.IndexOf(';', line2.IndexOf(';') + 1);
            //                    string foundLogin = Slice(line2, 0, first);
            //                    if (foundLogin.Equals(login))
            //                    {
            //                        string guid = Slice(line2, first + 1, second);
            //                        string fileName = Slice(line2, second + 1, line2.Length);
            //                        model.Login = login;
            //                        PDF pdf = new PDF
            //                        {
            //                            Guid = guid,
            //                            FileName = fileName
            //                        };
            //                        model.PDFs.Add(pdf);
            //                    }
            //                }
            //            }
            //            return View(model);
            //        }
            //        catch (Exception)
            //        {
            //            using (FileStream fs = System.IO.File.Create(webrootFolder + @"\pdfs.txt"))
            //            {
            //            }

            //        }
            //        UserViewModel model2 = new UserViewModel
            //        {
            //            Login = login,
            //            PDFs = new List<PDF>()
            //        };
            //        return View(model2);
            //    }
            //    else
            //    {
            //        return RedirectToAction("Index");
            //    }
            //}
            //catch (Exception)
            //{
            //    return RedirectToAction("Index");
            //}
        }
//        [Authorize]
//        public IActionResult AddPDF()
//        {
//            string login = HttpContext.User.Identity.Name;
//            if (!CheckIfUserInRedis(login))
//                return RedirectToAction("Denied");
//            PDFViewModel model = new PDFViewModel();
//            model.Login = HttpContext.User.Identity.Name;
//            return View(model);
//        }
//        [HttpPost]
//        [Authorize]
//        public async Task<ActionResult> AddPDF(PDFViewModel model)
//        {
//            string login = HttpContext.User.Identity.Name;
//            var webrootFolder = _env.WebRootPath;
//            if(!CheckIfUserInRedis(login))
//                return RedirectToAction("Denied");
//            if (!ModelState.IsValid)
//                return View(model);

//            using (var ms = new MemoryStream())
//            {
//                model.File.CopyTo(ms);
//                var fileBytes = ms.ToArray();
//                string guid = Guid.NewGuid().ToString();
//                var values = new
//                {
//                    guid = guid,
//                    file = fileBytes
//                };

//                var stringPayload = JsonConvert.SerializeObject(values);
//                var content = new StringContent(stringPayload, Encoding.UTF8, "application/json");
//                var x = GetToken();

//                client.DefaultRequestHeaders.Authorization
//                         = new AuthenticationHeaderValue("Bearer", x);
//                string subscriptionUrl = new UriBuilder("http", "web2", 80, "/api/Api/post").Uri.ToString();
//                var response = await client.PostAsync(subscriptionUrl, content);

//                var responseString = await response.Content.ReadAsStringAsync();

//                string text = model.Login + ";" + guid + ";" + model.Name;
//                try
//                {
//                    using (System.IO.StreamWriter file =
//new System.IO.StreamWriter(webrootFolder + @"\pdfs.txt", true))
//                    {
//                        file.WriteLine(text);
//                    }
//                }
//                catch (Exception)
//                {
//                    using (FileStream fs = System.IO.File.Create(webrootFolder + @"\pdfs.txt"))
//                    {
//                    }
//                    using (System.IO.StreamWriter file =
//new System.IO.StreamWriter(webrootFolder + @"\pdfs.txt", true))
//                    {
//                        file.WriteLine(text);
//                    }
//                }
//                // act on the Base64 data
//            }

//            return RedirectToAction("Profile", new { login = model.Login });
//        }
        //public static string Slice(string source, int start, int end)
        //{
        //    if (end < 0) // Keep this for negative end support
        //    {
        //        end = source.Length + end;
        //    }
        //    int len = end - start;               // Calculate length
        //    return source.Substring(start, len); // Return Substring of length
        //}
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
            //string line;
            //try
            //{
            //    using (StreamReader file = new StreamReader(webrootFolder + @"\users.txt"))
            //    {
            //        while ((line = file.ReadLine()) != null)
            //        {
            //            int first = line.IndexOf(";");
            //            int second = line.IndexOf(';', line.IndexOf(';') + 1);
            //            var x = Slice(line, first + 1, second);
            //            if (x.Equals(login))
            //            {
            //                int third = line.IndexOf(';', line.IndexOf(';', line.IndexOf(';') + 1) + 1);
            //                string sid = Slice(line, third + 1, line.Length);

            //                var redis = ConnectionMultiplexer.Connect("redis_image:6379");
            //                var db = redis.GetDatabase();

            //                var z = db.StringGet(sid);

            //                if (!z.HasValue)
            //                    return false;
            //                else
            //                    return true;
            //            }
            //        }
            //        return false;
            //    }
            //}
            //catch (Exception)
            //{
            //    return false;
            //}
        }
        public IActionResult Denied()
        {
            return View();
        }
        public async Task Login(string returnUrl = "/Home/Logged")
        {
            await HttpContext.ChallengeAsync("Auth0", new AuthenticationProperties() { RedirectUri = returnUrl });
        }
        public async Task<ActionResult> Logged()
        {
            var id = HttpContext.User.Claims.FirstOrDefault(b => b.Type == ClaimTypes.NameIdentifier).Value;
            var nickname = HttpContext.User.Claims.FirstOrDefault(b => b.Type == "nickname").Value;
            var email = HttpContext.User.Claims.FirstOrDefault(b => b.Type == "name").Value;
            var redis = ConnectionMultiplexer.Connect("redis_image:6379");
            var db = redis.GetDatabase();
            var keys = redis.GetServer("redis_image", 6379).Keys();
            foreach (var key in keys)
            {
                if(key.Equals("user_" + id))
                {
                    var x = db.StringGet(key.ToString());
                    var u = JsonConvert.DeserializeObject<User>(x.ToString());
                    u.Logged = true;
                    var serializedU = JsonConvert.SerializeObject(u);
                    db.StringSet("user_" + u.Id, serializedU);
                    return RedirectToAction("Index");
                }
            }
            User user = new User
            {
                Id = id,
                Login = nickname,
                Email = email,
                Positions = new List<string>(),
                Files = new List<string>(),
                Logged = true
            };
            var serialized = JsonConvert.SerializeObject(user);
            db.StringSet("user_" + user.Id, serialized);
            return RedirectToAction("Index");
        }

        [Authorize]
        public async Task Logout()
        {
            await HttpContext.SignOutAsync("Auth0", new AuthenticationProperties
            {
                // Indicate here where Auth0 should redirect the user after a logout.
                // Note that the resulting absolute Uri must be whitelisted in the
                // **Allowed Logout URLs** settings for the app.
                RedirectUri = Url.Action("Index", "Home")
            });
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
        //[Authorize]
        //[HttpPost]
        //public async Task<ActionResult> LogOut()
        //{
        //    await HttpContext.SignOutAsync();
        //    string login = HttpContext.User.Identity.Name;
        //    var redis = ConnectionMultiplexer.Connect("redis_image:6379");
        //    var db = redis.GetDatabase();
        //    var keys = redis.GetServer("redis_image", 6379).Keys();
        //    foreach (var key in keys)
        //    {
        //        if (key.ToString().StartsWith("user_"))
        //        {
        //            var x = db.StringGet(key.ToString());
        //            var u = JsonConvert.DeserializeObject<User>(x.ToString());
        //            if (u.Login.Equals(login))
        //            {
        //                u.Logged = false;
        //                var serialized = JsonConvert.SerializeObject(u);
        //                db.StringSet("user_" + u.Id, serialized);
        //            }
        //        }
        //    }
        //    //string line;
        //    //var webrootFolder = _env.WebRootPath;
        //    //using (StreamReader file = new StreamReader(webrootFolder + @"\users.txt"))
        //    //{
        //    //    while ((line = file.ReadLine()) != null)
        //    //    {
        //    //        int first = line.IndexOf(";");
        //    //        int second = line.IndexOf(';', line.IndexOf(';') + 1);
        //    //        var x = Slice(line, first + 1, second);
        //    //        if (x.Equals(login))
        //    //        {
        //    //            int third = line.IndexOf(';', line.IndexOf(';', line.IndexOf(';') + 1) + 1);
        //    //            string sid = Slice(line, third + 1, line.Length);

        //    //            var redis = ConnectionMultiplexer.Connect("redis_image:6379");
        //    //            var db = redis.GetDatabase();

        //    //            db.KeyDelete(sid);
        //    //        }
        //    //    }
        //    //}
        //    return RedirectToAction("Index");
        //}
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult Callback()
        {
            return View();
        }
        public string GetToken()
        {
            string securityKey = "supper_long_security_key_for_token_validation_pamiw_291118";

            var symetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));

            var signingCredentials = new SigningCredentials(symetricSecurityKey, SecurityAlgorithms.HmacSha256Signature);

            var token = new JwtSecurityToken(
                issuer: "http://localhost:8080",
                audience: "http://localhost:8081",
                expires: DateTime.Now.AddMinutes(2),
                signingCredentials: signingCredentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
