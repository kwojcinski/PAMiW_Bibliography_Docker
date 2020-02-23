using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using PAWB_L1.Controllers;
using StackExchange.Redis;

namespace PAMiW_291118.Controllers
{
    [Route("api/Api/[action]")]
    [ApiController]
    [Authorize]
    public class ApiController : ControllerBase
    {
        private IHostingEnvironment _env;
        private static readonly HttpClient client = new HttpClient();
        public ApiController(IHostingEnvironment env)
        {
            _env = env;
        }
        [HttpGet]
        [Authorize]
        [ActionName("DownloadPDF")]
        public async Task<IActionResult> DownloadPDF(string userGuid, string guid)
        {
            string id = HttpContext.User.Claims.FirstOrDefault(b => b.Type == ClaimTypes.NameIdentifier).Value;
            var webrootFolder = _env.WebRootPath;
            if (!HomeController.CheckIfUserInRedis(id))
                return NotFound();
            var x = GetToken();

            client.DefaultRequestHeaders.Authorization
                     = new AuthenticationHeaderValue("Bearer", x);

            string subscriptionUrl = new UriBuilder("http", "web3", 80, "/api/Api/get").Uri.ToString();
            subscriptionUrl += "?userGuid=" + userGuid + "&guid=" + guid;

            var response = await client.GetAsync(subscriptionUrl);
            var data = await response.Content.ReadAsStringAsync();
            Models.File file = JsonConvert.DeserializeObject<Models.File>(data);

            MemoryStream ms = new MemoryStream(file.FileBytes);
            return File(file.FileBytes, "application/pdf", file.FileName + ".pdf");
        }
        public string GetToken()
        {
            string securityKey = "supper_long_security_key_for_token_validation_pamiw_291118";

            var symetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));

            var signingCredentials = new SigningCredentials(symetricSecurityKey, SecurityAlgorithms.HmacSha256Signature);

            var token = new JwtSecurityToken(
                issuer: "http://localhost:8080",
                audience: "http://localhost:8082",
                expires: DateTime.Now.AddMinutes(2),
                signingCredentials: signingCredentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}