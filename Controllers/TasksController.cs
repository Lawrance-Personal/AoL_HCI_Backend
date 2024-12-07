using AoL_HCI_Backend.Contracts;
using AoL_HCI_Backend.Models;
using AoL_HCI_Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace AoL_HCI_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController(MongoDBServices database, IAuthenticationServices authentication) : ControllerBase
    {
        private readonly MongoDBServices _database = database;
        private readonly IAuthenticationServices _authentication = authentication;

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ReturnDataRecord<Tasks>>> Create([FromHeader(Name = "Authorization")] string token, [FromHeader(Name = "Refresh-Token")] string refreshToken, CreateTaskRecord createTask){
            Response.Headers.Append("Access-Control-Allow-Origin", "*");
            var tokenArr = token.Split(" ");
            AuthToken newToken = JwtValidator.ToAuthToken(tokenArr[1], refreshToken);
            if(JwtValidator.TokenIsExpired(tokenArr[1])){
                var refreshedToken = await _authentication.RefreshToken(refreshToken);
                if (refreshedToken != null)
                {
                    newToken = refreshedToken;
                    if (newToken.IdToken is null) return Unauthorized("Token Expired");
                }
                else
                {
                    return Unauthorized("Token Expired");
                }
            }
            Tasks task = createTask.ToModel();
            await _database.GetCollection<Tasks>("Tasks").InsertOneAsync(task);
            _database.SyncStatusTask(task);
            return CreatedAtRoute(new {id = task.Id}, new ReturnDataRecord<Tasks>(task, newToken));
        }
        
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<ReturnListRecord<Tasks>>> Get([FromHeader(Name = "Authorization")] string token, [FromHeader(Name = "Refresh-Token")] string refreshToken, [FromQuery] string? id = null, [FromQuery] string? divisionId = null, [FromQuery] string? jobId = null, [FromQuery] string? dueDate = null, [FromQuery] int? status = null){
            Response.Headers.Append("Access-Control-Allow-Origin", "*");
            var tokenArr = token.Split(" ");
            AuthToken newToken = JwtValidator.ToAuthToken(tokenArr[1], refreshToken);
            if(JwtValidator.TokenIsExpired(tokenArr[1])){
                var refreshedToken = await _authentication.RefreshToken(refreshToken);
                if (refreshedToken!= null)
                {
                    newToken = refreshedToken;
                    if (newToken.IdToken is null) return Unauthorized("Token Expired");
                }
                else
                {
                    return Unauthorized("Token Expired");
                }
            }
            var filterBuilder = Builders<Tasks>.Filter;
            var filter = filterBuilder.Empty;
            if(!string.IsNullOrEmpty(id)) filter &= filterBuilder.Eq(u => u.Id, id);
            if(!string.IsNullOrEmpty(divisionId)) filter &= filterBuilder.Eq(u => u.DivisionId, divisionId);
            if(!string.IsNullOrEmpty(jobId)) filter &= filterBuilder.Eq(u => u.JobId, jobId);
            if(!string.IsNullOrEmpty(dueDate)){
                var dueDateUnix = long.Parse(dueDate);
                filter &= filterBuilder.Lt(u => long.Parse(u.DueDate), dueDateUnix);
            }
            if(status != null) filter &= filterBuilder.Eq(u => u.Status, status);
            return Ok(new ReturnListRecord<Tasks>(await _database.GetCollection<Tasks>("Tasks").Find(filter).ToListAsync(), newToken));
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<ReturnDataRecord<Tasks>>> Update([FromHeader(Name = "Authorization")] string token, [FromHeader(Name = "Refresh-Token")] string refreshToken, string id, UpdateTaskRecord updateTask){
            Response.Headers.Append("Access-Control-Allow-Origin", "*");
            var tokenArr = token.Split(" ");
            AuthToken newToken = JwtValidator.ToAuthToken(tokenArr[1], refreshToken);
            if(JwtValidator.TokenIsExpired(tokenArr[1])){
                var refreshedToken = await _authentication.RefreshToken(refreshToken);
                if (refreshedToken!= null)
                {
                    newToken = refreshedToken;
                    if (newToken.IdToken is null) return Unauthorized("Token Expired");
                }
                else
                {
                    return Unauthorized("Token Expired");
                }
            }
            var task = await _database.GetCollection<Tasks>("Tasks").Find(u => u.Id == id).FirstOrDefaultAsync();
            if(task is null) return NotFound("Task not found");
            task = updateTask.ReplaceModel(task);
            await _database.GetCollection<Tasks>("Tasks").ReplaceOneAsync(u => u.Id == id, task);
            return new ReturnDataRecord<Tasks>(task, newToken);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult<ReturnDataRecord<Tasks>>> Delete([FromHeader(Name = "Authorization")] string token, [FromHeader(Name = "Refresh-Token")] string refreshToken, string id){
            Response.Headers.Append("Access-Control-Allow-Origin", "*");
            var tokenArr = token.Split(" ");
            AuthToken newToken = JwtValidator.ToAuthToken(tokenArr[1], refreshToken);
            if(JwtValidator.TokenIsExpired(tokenArr[1])){
                var refreshedToken = await _authentication.RefreshToken(refreshToken);
                if (refreshedToken!= null)
                {
                    newToken = refreshedToken;
                    if (newToken.IdToken is null) return Unauthorized("Token Expired");
                }
                else
                {
                    return Unauthorized("Token Expired");
                }
            }
            var task = await _database.GetCollection<Tasks>("Tasks").Find(u => u.Id == id).FirstOrDefaultAsync();
            if(task is null) return NotFound("Task not found");
            await _database.GetCollection<Tasks>("Tasks").DeleteOneAsync(u => u.Id == id);
            _database.SyncStatusTask(task);
            return new ReturnDataRecord<Tasks>(task, newToken);
        }
    }
}
