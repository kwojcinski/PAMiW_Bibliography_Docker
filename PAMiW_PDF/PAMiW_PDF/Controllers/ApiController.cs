using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace PAMiW_PDF.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiController : ControllerBase
    {

        [EnableCors]
        [Authorize]
        [HttpPost("post")]
        public Dictionary<string, string> PostFile(Dictionary<string, string> post)
        {
            var redis = ConnectionMultiplexer.Connect("redis_image:6379");
            var db = redis.GetDatabase();

            db.StringSet(post["guid"], post["file"]);

            return post;

        }
        [EnableCors]
        [Authorize]
        [HttpGet("get")]
        public IActionResult GetFile(string userGuid, string guid)
        {
            if (String.IsNullOrEmpty(guid) || String.IsNullOrEmpty(userGuid))
            {
                return BadRequest();
            }
            var redis = ConnectionMultiplexer.Connect("redis_image:6379");
            var db = redis.GetDatabase();

            var fileJson = db.StringGet("file_" + guid);
            if (fileJson.IsNullOrEmpty)
            {
                return NotFound($"Plik z id {guid} nie został znaleziony");
            }
            var y = db.StringGet("user_" + userGuid);
            if (y.IsNullOrEmpty)
            {
                return this.NotFound($"Użytkownik z id {userGuid} nie został znaleziony");
            }
            var file = JsonConvert.DeserializeObject<File>(fileJson.ToString());

            return Ok(file);


        }
    }
}