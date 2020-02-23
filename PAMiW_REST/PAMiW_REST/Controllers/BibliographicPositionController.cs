using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StackExchange.Redis;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace PAMiW_REST.Controllers
{
    [ApiController]
    [Route("rest")]
    public class BibliographicPositionController : Controller
    {
        [EnableCors]
        [HttpGet("GetPositions/{userGuid}", Name = "GetPositions")]
        public IActionResult GetPositions(string userGuid)
        {
            if (String.IsNullOrEmpty(userGuid))
            {
                return this.BadRequest();
            }
            var redis = ConnectionMultiplexer.Connect("redis_image:6379");
            var db = redis.GetDatabase();
            List<BibliographicPosition> list = new List<BibliographicPosition>();
            var x = db.StringGet("user_" + userGuid);
            if (x.IsNullOrEmpty)
            {
                return this.NotFound($"Użytkownik z id {userGuid} nie został znaleziony");
            }
            var user = JsonConvert.DeserializeObject<User>(x.ToString());
            foreach (string key in user.Positions)
            {
                var y = db.StringGet("position_" + key);
                var position = JsonConvert.DeserializeObject<BibliographicPosition>(y.ToString());
                foreach (File f in position.Files)
                {
                    this.CreateLinksForFile(f, userGuid);
                }
                this.CreateLinksForPosition(position, userGuid);
                list.Add(position);
            }
            return Ok(list);
        }
        [EnableCors]
        [HttpGet("GetPosition/{userGuid}/{guid}", Name = "GetPosition")]
        public IActionResult GetPosition(string userGuid, string guid)
        {
            if (String.IsNullOrEmpty(guid) || String.IsNullOrEmpty(userGuid))
            {
                return this.BadRequest();
            }
            var redis = ConnectionMultiplexer.Connect("redis_image:6379");
            var db = redis.GetDatabase();
            var x = db.StringGet("position_" + guid);
            if (x.IsNullOrEmpty)
            {
                return this.NotFound($"Pozycja z id {guid} nie została znaleziona");
            }
            var y = db.StringGet("user_" + userGuid);
            if (y.IsNullOrEmpty)
            {
                return this.NotFound($"Użytkownik z id {userGuid} nie został znaleziony");
            }
            var position = JsonConvert.DeserializeObject<BibliographicPosition>(x);
            foreach (File f in position.Files)
            {
                this.CreateLinksForFile(f, userGuid);
            }
            return Ok(this.CreateLinksForPosition(position, userGuid));
        }
        [EnableCors]
        [HttpPost("PostPosition", Name = "PostPosition")]
        public IActionResult PostPosition([FromForm]BibliographicPositionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return this.BadRequest("Podano błędne dane!");
            }
            var redis = ConnectionMultiplexer.Connect("redis_image:6379");
            var db = redis.GetDatabase();
            var x = db.StringGet("user_" + model.UserGuid);
            if (x.IsNullOrEmpty)
            {
                return this.NotFound($"Użytkownik z id {model.UserGuid} nie został znaleziony");
            }
            BibliographicPosition position = new BibliographicPosition();
            position.Title = model.Title;
            position.Author = model.Author;
            position.PublicationDate = model.PublicationDate;
            position.Links = new List<Link>();
            position.Files = new List<File>();
            position.UserId = model.UserGuid;
            //this.CreateLinksForUser(position);
            position.Guid = Guid.NewGuid().ToString();
            var serialized = JsonConvert.SerializeObject(position);
            db.StringSet("position_" + position.Guid, serialized);
            var user = JsonConvert.DeserializeObject<User>(x.ToString());
            user.Positions.Add(position.Guid);
            var serializedU = JsonConvert.SerializeObject(user);
            db.StringSet("user_" + user.Id, serializedU);
            return Ok(this.CreateLinksForPosition(position, model.UserGuid));
        }
        [EnableCors]
        [HttpDelete("DeletePosition/{userGuid}/{guid}", Name = "DeletePosition")]
        public IActionResult DeletePosition(string userGuid, string guid)
        {
            if (String.IsNullOrEmpty(guid) || String.IsNullOrEmpty(userGuid))
            {
                return this.BadRequest();
            }
            var redis = ConnectionMultiplexer.Connect("redis_image:6379");
            var db = redis.GetDatabase();
            var x = db.StringGet("position_" + guid);
            if (x.IsNullOrEmpty)
            {
                return this.NotFound($"Pozycja z id {guid} nie została znaleziona");
            }
            var y = db.StringGet("user_" + userGuid);
            if (y.IsNullOrEmpty)
            {
                return this.NotFound($"Użytkownik z id {userGuid} nie został znaleziony");
            }
            var user = JsonConvert.DeserializeObject<User>(y.ToString());
            user.Positions.Remove(guid);
            db.KeyDelete("position_" + guid);
            var serializedU = JsonConvert.SerializeObject(user);
            db.StringSet("user_" + user.Id, serializedU);
            return Ok($"Position with id {guid} was deleted");
        }
        [EnableCors]
        [HttpPost("AddFile", Name = "AddFile")]
        public IActionResult AddFile([FromForm]FileViewModel file)
        {
            var redis = ConnectionMultiplexer.Connect("redis_image:6379");
            var db = redis.GetDatabase();
            var x = db.StringGet("user_" + file.UserGuid);
            if (x.IsNullOrEmpty)
            {
                return this.NotFound($"Użytkownik z id {file.UserGuid} nie został znaleziony");
            }
            byte[] fileBytes;
            if (file.PDF != null)
            {
                using (var ms = new MemoryStream())
                {
                    file.PDF.CopyTo(ms);
                    fileBytes = ms.ToArray();
                }
            }
            else if (!String.IsNullOrEmpty(file.PDFbytes))
            {
                fileBytes = Convert.FromBase64String(file.PDFbytes);
            }
            else
            {
                return this.BadRequest();
            }
            File newFile = new File
            {
                Guid = Guid.NewGuid().ToString(),
                FileName = file.FileName,
                FileBytes = fileBytes,
                Links = new List<Link>()
            };
            var serialized = JsonConvert.SerializeObject(newFile);
            db.StringSet("file_" + newFile.Guid, serialized);
            var user = JsonConvert.DeserializeObject<User>(x.ToString());
            user.Files.Add(newFile.Guid);
            var serializedU = JsonConvert.SerializeObject(user);
            db.StringSet("user_" + user.Id, serializedU);
            return Ok(this.CreateLinksForFile(newFile, file.UserGuid));
        }
        [EnableCors]
        [HttpGet("GetFile/{userGuid}/{guid}", Name = "GetFile")]
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

            MemoryStream ms = new MemoryStream(file.FileBytes);
            return File(file.FileBytes, "application/pdf", file.FileName + ".pdf");
        }
        [EnableCors]
        [HttpGet("GetFiles/{userGuid}", Name = "GetFiles")]
        public IActionResult GetFiles(string userGuid)
        {
            if (String.IsNullOrEmpty(userGuid))
            {
                return BadRequest();
            }
            var redis = ConnectionMultiplexer.Connect("redis_image:6379");
            var db = redis.GetDatabase();
            List<File> list = new List<File>();
            var x = db.StringGet("user_" + userGuid);
            if (x.IsNullOrEmpty)
            {
                return this.NotFound($"Użytkownik z id {userGuid} nie został znaleziony");
            }
            var user = JsonConvert.DeserializeObject<User>(x.ToString());
            foreach (string key in user.Files)
            {
                var y = db.StringGet("file_" + key);
                var file = JsonConvert.DeserializeObject<File>(y.ToString());
                file.FileBytes = null;
                this.CreateLinksForFile(file, userGuid);
                list.Add(file);
            }
            return Ok(list);
        }
        [EnableCors]
        [HttpDelete("DeleteFile/{userGuid}/{guid}", Name = "DeleteFile")]
        public IActionResult DeleteFile(string userGuid, string guid)
        {
            if (String.IsNullOrEmpty(guid) || String.IsNullOrEmpty(userGuid))
            {
                return this.BadRequest();
            }
            var redis = ConnectionMultiplexer.Connect("redis_image:6379");
            var db = redis.GetDatabase();
            var x = db.StringGet("file_" + guid);
            if (x.IsNullOrEmpty)
            {
                return this.NotFound($"Plik z id {guid} nie został znaleziony");
            }
            var z = db.StringGet("user_" + userGuid);
            if (z.IsNullOrEmpty)
            {
                return this.NotFound($"Użytkownik z id {userGuid} nie został znaleziony");
            }
            var user = JsonConvert.DeserializeObject<User>(z.ToString());
            foreach (var pos in user.Positions)
            {
                    bool isConnected = false;
                    var y = db.StringGet("position_" + pos);
                    var position = JsonConvert.DeserializeObject<BibliographicPosition>(y.ToString());
                    if (position.Files != null)
                    {
                        foreach (File f in position.Files)
                        {
                            if (guid.Equals(f.Guid))
                            {
                                isConnected = true;
                            }
                        }
                        if (isConnected)
                        {
                            position.Files.RemoveAll(f => f.Guid == guid);
                            var serialized = JsonConvert.SerializeObject(position);
                            db.StringSet("position_" + position.Guid, serialized);
                        }
                    }
            }
            user.Files.Remove(guid);
            db.KeyDelete("file_" + guid);
            var serializedU = JsonConvert.SerializeObject(user);
            db.StringSet("user_" + user.Id, serializedU);
            return Ok($"File with id {guid} was deleted");
        }
        [EnableCors]
        [HttpPatch("ConnectFile/{userGuid}/{positionGuid}", Name = "ConnectFile")]
        public IActionResult ConnectFile(string userGuid, string positionGuid, string fileGuid)
        {
            if (String.IsNullOrEmpty(positionGuid) || String.IsNullOrEmpty(fileGuid) || String.IsNullOrEmpty(userGuid))
            {
                return BadRequest();
            }
            var redis = ConnectionMultiplexer.Connect("redis_image:6379");
            var db = redis.GetDatabase();
            var x = db.StringGet("position_" + positionGuid);
            if (x.IsNullOrEmpty)
            {
                return this.NotFound($"Pozycja z id {positionGuid} nie została znaleziona");
            }
            var y = db.StringGet("file_" + fileGuid);
            if (y.IsNullOrEmpty)
            {
                return this.NotFound($"Plik z id {fileGuid} nie został znaleziony");
            }
            var z = db.StringGet("user_" + userGuid);
            if (z.IsNullOrEmpty)
            {
                return this.NotFound($"Użytkownik z id {userGuid} nie został znaleziony");
            }
            var position = JsonConvert.DeserializeObject<BibliographicPosition>(x.ToString());
            var file = JsonConvert.DeserializeObject<File>(y.ToString());
            file.FileBytes = null;
            int index = position.Files.FindIndex(item => item.Guid == fileGuid);
            if (index >= 0)
            {
                return this.NotFound($"Plik z id {fileGuid} jest już dodany");
            }
            position.Files.Add(file);
            db.StringSet("position_" + positionGuid, JsonConvert.SerializeObject(position));
            foreach(File f in position.Files)
            {
                this.CreateLinksForFile(f, userGuid);
            }
            return Ok(this.CreateLinksForPosition(position, userGuid));
        }
        [EnableCors]
        [HttpPatch("DisconnectFile/{userGuid}/{positionGuid}", Name = "DisconnectFile")]
        public IActionResult DisconnectFile(string userGuid, string positionGuid, string fileGuid)
        {
            if (String.IsNullOrEmpty(positionGuid) || String.IsNullOrEmpty(fileGuid) || String.IsNullOrEmpty(userGuid))
            {
                return BadRequest();
            }
            var redis = ConnectionMultiplexer.Connect("redis_image:6379");
            var db = redis.GetDatabase();
            var x = db.StringGet("position_" + positionGuid);
            if (x.IsNullOrEmpty)
            {
                return this.NotFound($"Pozycja z id {positionGuid} nie została znaleziona");
            }
            var y = db.StringGet("file_" + fileGuid);
            if (y.IsNullOrEmpty)
            {
                return this.NotFound($"Plik z id {fileGuid} nie został znaleziony");
            }
            var z = db.StringGet("user_" + userGuid);
            if (z.IsNullOrEmpty)
            {
                return this.NotFound($"Użytkownik z id {userGuid} nie został znaleziony");
            }
            var position = JsonConvert.DeserializeObject<BibliographicPosition>(x.ToString());
            var file = JsonConvert.DeserializeObject<File>(y.ToString());
            file.FileBytes = null;
            int index = position.Files.FindIndex(item => item.Guid == fileGuid);
            if (index < 0)
            {
                return this.NotFound($"Pozycja z id {positionGuid} nie ma podłączonego pliku o id {fileGuid}");
            }
            else
            {
                position.Files.RemoveAll(z => z.Guid == fileGuid);
            }
            db.StringSet("position_" + positionGuid, JsonConvert.SerializeObject(position));
            foreach (File f in position.Files)
            {
                this.CreateLinksForFile(f, userGuid);
            }
            return Ok(this.CreateLinksForPosition(position, userGuid));
        }
        private BibliographicPosition CreateLinksForPosition(BibliographicPosition position, string userGuid)
        {
            position.Links = new List<Link>();
            var idObj = new { guid = position.Guid };

            position.Links.Add(
                new Link(Request.Scheme + "://" + Request.Host + "/rest/" + nameof(this.GetPositions) + "/" + userGuid,
                "get-positions",
                "GET"));

            position.Links.Add(
                new Link(Request.Scheme + "://" + Request.Host + "/rest/" + nameof(this.GetPosition) + "/" + userGuid + "/" + position.Guid,
                "get-position",
                "GET"));

            position.Links.Add(
                new Link(Request.Scheme + "://" + Request.Host + "/rest/" + nameof(this.PostPosition),
                "post-position",
                "POST"));

            position.Links.Add(
                new Link(Request.Scheme + "://" + Request.Host + "/rest/" + nameof(this.ConnectFile) + "/" + userGuid + "/" + position.Guid + "?fileGuid=",
                "connect-file",
                "PATCH"));

            position.Links.Add(
                new Link(Request.Scheme + "://" + Request.Host + "/rest/" + nameof(this.DisconnectFile) + "/" + userGuid + "/" + position.Guid + "?fileGuid=",
                "disconnect-file",
                "PATCH"));

            position.Links.Add(
                new Link(Request.Scheme + "://" + Request.Host + "/rest/" + nameof(this.DeletePosition) + "/" + userGuid + "/" + position.Guid,
                "delete-position",
                "DELETE"));

            position.Links.Add(
                new Link(Request.Scheme + "://" + Request.Host + "/rest/" + nameof(this.GetFiles) + "/" + userGuid,
                "get-files",
                "GET"));

            return position;
        }
        private File CreateLinksForFile(File file, string userGuid)
        {
            file.Links = new List<Link>();

            file.Links.Add(
                new Link(Request.Scheme + "://" + Request.Host + "/rest/" + nameof(this.GetFiles) + "/" + userGuid,
                "get-files",
                "GET"));

            file.Links.Add(
                new Link("http://localhost:8080/api/Api/DownloadPDF?userGuid=" + userGuid + "&guid=" + file.Guid,
                "get-file",
                "GET"));

            file.Links.Add(
                new Link(Request.Scheme + "://" + Request.Host + "/rest/" + nameof(this.AddFile),
                "add-file",
                "POST"));

            file.Links.Add(
                new Link(Request.Scheme + "://" + Request.Host + "/rest/" + nameof(this.DeleteFile) + "/" + userGuid + "/" + file.Guid,
                "delete-file",
                "DELETE"));

            return file;
        }
    }
}